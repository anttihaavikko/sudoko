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
    [SerializeField] private Transform hitPos;
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

    private void SkillEffect()
    {
        anim.SetTrigger(SkillTrigger);
    }

    private void Gear()
    {
        var gear = StateManager.Instance.Gear;
        
        if (isPlayer)
        {
            equipmentVisuals.ForEach(v =>
            {
                v.Hide();
                var match = gear.FirstOrDefault(g => g.slot == v.Slot);
                if (match == default) return;
                v.Show(match);
                gear.Remove(match);
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

    public void Add(Equip e)
    {
        EffectManager.AddTextPopup(e.sprite.name.ToUpper(), transform.position + Vector3.up * 3);
        
        var slot = equipmentVisuals.FirstOrDefault(v => v.Slot == e.slot && v.IsFree);
        if (slot)
        {
            slot.Show(e);
            StateManager.Instance.AddGear(e);
        }
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
}