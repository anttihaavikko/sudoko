using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Visuals;
using Equipment;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    [SerializeField] private Flasher flasher;
    [SerializeField] private Health health;
    [SerializeField] private List<Transform> mirrorables;
    [SerializeField] private Transform healthDisplay, moveDisplay;
    [SerializeField] private GameObject root, shadow;
    [SerializeField] private bool isPlayer;
    [SerializeField] private Transform moveBar;
    [SerializeField] private float moveDelay = 1f;
    [SerializeField] private float scale = 1f;
    [SerializeField] private List<EquipmentVisuals> equipmentVisuals;
    [SerializeField] private EquipmentList equipmentList;
    [SerializeField] private Transform hitPos;

    private Animator anim;

    private Vector3 origin;
    private EffectCamera cam;
    private readonly List<Equip> drops = new();

    private float moveTimer;

    public int CurrentHealth => health.Current;
    
    public Board Board { get; set; }
    
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int AttackAnim = Animator.StringToHash("attack");
    private static readonly int Hurt = Animator.StringToHash("hurt");

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

        transform.localScale *= scale;

        Gear();
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

            var amount = 1f - moveTimer / moveDelay;
            moveBar.localScale = moveBar.localScale.WhereY(amount);

            if (moveTimer <= 0)
            {
                moveTimer = moveDelay;
                Board.EnemyAttack(1);
            }
        }
    }

    private void Stagger()
    {
        moveTimer = moveDelay;
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

    public void Damage(int amount)
    {
        anim.SetTrigger(Hurt);
        cam.BaseEffect(0.2f);
        flasher.Flash();
        health.TakeDamage(amount);
        Stagger();

        var p = hitPos.position;
        EffectManager.AddTextPopup(amount.ToString(), p.RandomOffset(0.2f));
        EffectManager.AddEffect(0, p.RandomOffset(0.2f));
    }

    public void Attack(Character target, int amount)
    {
        var t = transform;
        anim.SetTrigger(AttackAnim);
        Tweener.MoveToBounceOut(t, origin + Vector3.right * 1.5f * t.localScale.x, 0.2f);
        this.StartCoroutine(() => target.Damage(amount), 0.15f);
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