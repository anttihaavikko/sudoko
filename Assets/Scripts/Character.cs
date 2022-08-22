using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
using Equipment;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    [SerializeField] private string title;
    [SerializeField] private Stats stats;
    [SerializeField] private List<Skill> startsWith;
    
    [SerializeField] private Flasher flasher;
    [SerializeField] private Health health;
    [SerializeField] private List<Transform> mirrorables;
    [SerializeField] private Transform healthDisplay, moveDisplay;
    [SerializeField] private GameObject root, shadow;
    [SerializeField] private bool isPlayer;
    [SerializeField] private Transform moveBar;
    [SerializeField] private float scale = 1f;
    [SerializeField] private List<EquipmentVisuals> equipmentVisuals;
    [SerializeField] private EquipmentList equipmentList;
    [SerializeField] private Transform hitPos, center;
    [SerializeField] private Transform groundMask;

    private Animator anim;

    private Vector3 origin;
    private EffectCamera cam;
    private readonly List<Equip> drops = new();
    private readonly List<Skill> skills = new();

    private float moveTimer;

    public int CurrentHealth => health.Current;
    
    public Board Board { get; set; }

    public Action<string> showDescription;
    
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int AttackAnim = Animator.StringToHash("attack");
    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int SkillTrigger = Animator.StringToHash("skill");
    private readonly List<Equip> inventory = new();

    private void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main.GetComponent<EffectCamera>();
        origin = transform.position;
        
        health.Init();

        healthDisplay.SetParent(null, true);
        moveDisplay.SetParent(null, true);

        Stagger();

        if (isPlayer)
        {
            transform.position += Vector3.left * 7;
            WalkTo(origin.x, true);
        }

        Scale();

        Gear();
        
        startsWith.ForEach(s => skills.Add(s.Copy()));
        
        showDescription?.Invoke(GetDescription());
    }

    private void Scale()
    {
        if (!isPlayer)
        {
            groundMask.SetParent(null, true);
        }

        transform.localScale *= scale;
        
        if (!isPlayer)
        {
            groundMask.SetParent(transform, true);
        }
    }

    private string GetDescription()
    {
        var sb = new StringBuilder();
        sb.Append(TextUtils.TextWith(title, 30));
        sb.Append(TextUtils.TextWith("\n\n", 10));
        sb.Append($"Attack: {stats.attack}\n");
        sb.Append($"Speed: 1 / {stats.speed} s\n");
        if (stats.defence > 0)
        {
            sb.Append($"Defence: {stats.defence}\n");
        }
        skills.ForEach(s => sb.Append($"\n{s.GetDescription()}"));
        return sb.ToString();
    }

    public void SkillEffect()
    {
        anim.SetTrigger(SkillTrigger);
    }

    private void Gear()
    {
        var gear = StateManager.Instance.Gear;
        
        if (isPlayer)
        {
            inventory.Clear();
            inventory.AddRange(StateManager.Instance.Inventory);
            
            equipmentVisuals.ForEach(v =>
            {
                v.Hide();
                var match = gear.FirstOrDefault(g => g != null && g.slot == v.Slot);
                if (match == default) return;
                gear.Remove(match);
                v.Show(match);
            });
            
            return;
        }

        GearEnemy();
    }

    private void GearEnemy()
    {
        equipmentVisuals.ForEach(v =>
        {
            v.Hide();
            if (!(Random.value < 0.5f)) return;
            var e = equipmentList.Random(v.Slot);
            v.Show(e);
            drops.Add(e);
        });
    }

    public List<Equip> GetDrops()
    {
        return drops;
    }

    public void Mirror()
    {
        transform.Mirror();
        mirrorables.ForEach(m => m.Mirror());
    }

    private void Update()
    {
        if (!isPlayer && IsAlive())
        {
            moveTimer -= Time.deltaTime;

            var amount = 1f - moveTimer / stats.speed;
            moveBar.localScale = moveBar.localScale.WhereY(amount);

            if (moveTimer <= 0)
            {
                moveTimer = stats.speed;
                Board.EnemyAttack(0);
            }
        }
    }

    private void Stagger()
    {
        moveTimer = stats.speed;
        moveBar.localScale = moveBar.localScale.WhereY(0);
    }

    public void Remove(Equip e)
    {
        var equippedSlot = equipmentVisuals.FirstOrDefault(v => v.Has(e));
        if (equippedSlot)
        {
            Remove(equippedSlot);
        }
    }

    public Equip Remove(int slotIndex)
    {
        if (equipmentVisuals[slotIndex].IsFree) return null;
        var e = equipmentVisuals[slotIndex].GetEquip();
        Remove(equipmentVisuals[slotIndex]);
        return e;
    }

    private void Remove(EquipmentVisuals slot)
    {
        inventory.Add(slot.GetEquip());
        slot.Hide();
        UpdateState();
    }

    public int Add(Equip e, int inSlot = -1)
    {
        inventory.Remove(e);
        
        if (inSlot < 0)
        {
            var equippedSlot = equipmentVisuals.FirstOrDefault(v => v.Has(e));
            if (equippedSlot)
            {
                return equipmentVisuals.IndexOf(equippedSlot);
            }   
        }

        EffectManager.AddTextPopup(e.sprite.name.ToUpper(), transform.position + Vector3.up * 3);
        
        var slot = inSlot >= 0 ? 
            equipmentVisuals[inSlot] : 
            equipmentVisuals.FirstOrDefault(v => v.Slot == e.slot && v.IsFree);
        
        if (slot)
        {
            slot.Show(e);
            UpdateState();
            return equipmentVisuals.IndexOf(slot);
        }

        UpdateState();
        inventory.Add(e);

        return -1;
    }

    private void UpdateState()
    {
        var equips = equipmentVisuals.Select(v => v.GetEquip()).Where(e => e != default);
        StateManager.Instance.UpdateEquips(equips, inventory);
    }

    public float WalkTo(float pos, bool showHpAfter)
    {
        healthDisplay.gameObject.SetActive(false);
        
        anim.SetBool(Walking, true);
        var t = transform;
        var p = origin.WhereX(pos);
        var walkTime = Mathf.Max(Mathf.Abs(p.x - t.position.x) * 0.25f, 0.2f);

        Tweener.MoveToQuad(t, p, walkTime);
        this.StartCoroutine(() =>
        {
            anim.SetBool(Walking, false);
            healthDisplay.gameObject.SetActive(showHpAfter);
        }, walkTime - 0.1f);

        return walkTime;
    }

    public void Die()
    {
        EffectManager.AddEffects(new []{ 1, 2 }, center.position);
        
        cam.BaseEffect(0.4f);
        root.SetActive(false);
        shadow.SetActive(false);
        this.StartCoroutine(() =>
        {
            healthDisplay.gameObject.SetActive(false);
            moveDisplay.gameObject.SetActive(false);
        }, 0.3f);
    }

    public bool Interrupts(int value)
    {
        var heals = skills.Where(s => s.Matches(SkillType.HealInsteadOnX, value)).ToList();

        if (heals.Any())
        {
            Heal(heals.Count * value);
        }
        
        return heals.Any();
    }

    public void Heal(int amount)
    {
        EffectManager.AddEffect(3, center.position);
        SkillEffect();
        health.Heal(amount);
        EffectManager.AddTextPopup(amount.ToString(), hitPos.position.RandomOffset(0.2f));
    }

    public void Damage(int amount)
    {
        var reduced = Mathf.Max(0, amount - stats.defence);
        
        var p = hitPos.position;
        anim.SetTrigger(Hurt);
        flasher.Flash();

        if (reduced <= 0)
        {
            cam.BaseEffect(0.1f);
            EffectManager.AddTextPopup("MISS", p.RandomOffset(0.2f));
            return;
        }
        
        cam.BaseEffect(0.2f);
        health.TakeDamage(reduced);
        Stagger();
        
        EffectManager.AddTextPopup(reduced.ToString(), p.RandomOffset(0.2f));
        EffectManager.AddEffect(0, p.RandomOffset(0.2f));
    }

    public void Attack(Character target, int amount)
    {
        var t = transform;
        anim.SetTrigger(AttackAnim);
        Tweener.MoveToBounceOut(t, origin + Vector3.right * 1.5f * t.localScale.x, 0.2f);
        this.StartCoroutine(() => target.Damage(amount + stats.attack), 0.15f);
        this.StartCoroutine(() => Tweener.MoveToQuad(t, origin, 0.2f), 0.25f);
    }

    public bool IsAlive()
    {
        return health.Current > 0;
    }

    public void SetHealth(int hp)
    {
        health.Init(hp);
    }

    public List<Equip> GetEquips()
    {
        return equipmentVisuals.Select(v => v.GetEquip()).Where(e => e != null).ToList();
    }

    public List<Equip> GetInventory()
    {
        return inventory.ToList();
    }

    public bool CanSlot(Equip e, int slotIndex)
    {
        return equipmentVisuals[slotIndex].Slot == e.slot && !equipmentVisuals[slotIndex].Has(e); 
    }
}