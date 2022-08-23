using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
using Equipment;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    [SerializeField] private string title;
    [SerializeField] private Stats stats;
    [SerializeField] private SkillSet startsWith;
    
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

    private bool fightStarted;

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

        Stagger(true);

        if (isPlayer)
        {
            transform.position += Vector3.left * 7;
            WalkTo(origin.x, true);
        }

        Scale();

        Gear();

        if (startsWith)
        {
            startsWith.Random(2).ToList().ForEach(s => skills.Add(s.Copy()));   
        }

        if (isPlayer)
        {
            var eq = GetEquips();
            SetHealth(StateManager.Instance.Health);
            skills.Clear();
            eq.ForEach(e => skills.AddRange(e.GetSkills()));
        }

        UpdateStats();
        
        showDescription?.Invoke(GetDescription());
    }

    private void UpdateStats()
    {
        var adds = skills.Select(s => s.GetStats()).ToList();
        stats.Add(adds);

        var hpAddition = skills.Sum(s => s.GetHp());
        health.AddMax(hpAddition, !isPlayer);
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
            sb.Append($"Defense: {stats.defence}\n");
        }
        skills.ForEach(s => sb.Append($"<size=10>\n</size>{s.GetDescription()}<size=10>\n</size>"));
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
        
        drops.Add(equipmentList.Random(EquipmentSlot.Soul));
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
        if (!fightStarted || isPlayer || !IsAlive()) return;
        
        moveTimer -= Time.deltaTime;

        var amount = 1f - moveTimer / stats.speed;
        moveBar.localScale = moveBar.localScale.WhereY(amount);

        if (moveTimer <= 0)
        {
            moveTimer = stats.speed;
            Board.EnemyAttack(0);
        }
    }

    private void Stagger(bool forced = false)
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

        EffectManager.AddTextPopup(e.GetName().ToUpper(), transform.position + Vector3.up * 3);
        
        var slot = inSlot >= 0 ? 
            equipmentVisuals[inSlot] : 
            equipmentVisuals.FirstOrDefault(v => v.Slot == e.slot && v.IsFree);
        
        if (slot)
        {
            slot.Show(e);
            UpdateState();
            return equipmentVisuals.IndexOf(slot);
        }
        
        inventory.Add(e);
        UpdateState();

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

    public bool Interrupts(int value, Character target)
    {
        var heals = GetSkillCount(SkillType.HealInsteadOnX, value);
        var counters = GetSkillCount(SkillType.AttackInsteadOnX, value);

        if (heals > 0)
        {
            Heal(heals * value);
        }
        
        if (counters > 0)
        {
            Attack(target, counters * value);
        }
        
        return heals + counters > 0;
    }

    public void Heal(int amount)
    {
        EffectManager.AddEffect(3, center.position);
        SkillEffect();
        health.Heal(amount);
        EffectManager.AddTextPopup(amount.ToString(), hitPos.position.RandomOffset(0.2f));
    }

    public void Damage(int amount, bool critical = false)
    {
        var reduced = critical ? amount : Mathf.Max(0, amount - stats.defence);
        
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

    public void Attack(Character target, int amount, bool boosted = true)
    {
        var critical = HasSkill(SkillType.IgnoresDefence);
        var t = transform;
        anim.SetTrigger(AttackAnim);
        Tweener.MoveToBounceOut(t, origin + Vector3.right * 1.5f * t.localScale.x, 0.2f);
        this.StartCoroutine(() =>
        {
            target.Damage(boosted ? amount + stats.attack : amount, critical);
            AttackTriggers();
        }, 0.15f);
        this.StartCoroutine(() => Tweener.MoveToQuad(t, origin, 0.2f), 0.25f);

        var thorns = target.GetSkillCount(SkillType.Thorns);
        
        if (thorns > 0)
        {
            this.StartCoroutine(() => target.Attack(this, thorns, false), 0.4f);
        }
    }

    public bool HasSkill(SkillType skill)
    {
        return skills.Any(s => s.Matches(skill));
    }

    public List<Skill> GetSkills(SkillType skill)
    {
        return skills.Where(s => s.Matches(skill)).ToList();
    }
    
    public List<Skill> GetSkills(SkillType skill, int number)
    {
        return skills.Where(s => s.Matches(skill, number)).ToList();
    }

    public int GetSkillCount(SkillType skill)
    {
        return skills.Count(s => s.Matches(skill));
    }
    
    public int GetSkillCount(SkillType skill, int number)
    {
        return skills.Count(s => s.Matches(skill, number));
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

    public bool SlotMatches(Equip e, int slotIndex)
    {
        return equipmentVisuals[slotIndex].Slot == e.slot;
    }

    public bool CanSlot(Equip e, int slotIndex)
    {
        return equipmentVisuals[slotIndex].Slot == e.slot && !equipmentVisuals[slotIndex].Has(e); 
    }
    
    public bool CanSlotSoul(int slotIndex)
    {
        var e = equipmentVisuals[slotIndex].GetEquip();
        return e != default && e.HasFreeSlot;
    }

    public void StartTimer()
    {
        fightStarted = true;
    }

    public void Slot(Equip e, int slotIndex)
    {
        inventory.Remove(e);
        var target = equipmentVisuals[slotIndex].GetEquip();
        target.Slot(e);
        UpdateState();
        Inventory.Instance.UpdateSlotsFor(target);
    }

    public void AttackTriggers()
    {
        GetSkills(SkillType.ClearCellOnAttack).ForEach(s => Board.ClearRandomCell());
        GetSkills(SkillType.ClearRowOnAttack).ForEach(s => Board.ClearRandomRow());
        GetSkills(SkillType.ClearColOnAttack).ForEach(s => Board.ClearRandomColumn());
        GetSkills(SkillType.ClearSectionOnAttack).ForEach(s => Board.ClearRandomSection());
        
        GetSkills(SkillType.DisableCellOnAttack).ForEach(s => Board.DisableRandomCell());
        GetSkills(SkillType.HideSolvedCellOnAttack).ForEach(s => Board.HideSolvedCell());
    }
}