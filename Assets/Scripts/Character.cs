using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Visuals;
using UnityEngine.Rendering.UI;

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
    

    private Animator anim;

    private Vector3 origin;
    private EffectCamera cam;

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

    public void WalkTo(float pos, bool showHpAfter)
    {
        healthDisplay.gameObject.SetActive(false);
        
        anim.SetBool(Walking, true);
        var t = transform;
        var p = origin.WhereX(pos);
        var walkTime = 12f / Vector3.Distance(p, t.position);

        Tweener.MoveToQuad(t, p, walkTime);
        this.StartCoroutine(() =>
        {
            anim.SetBool(Walking, false);
            healthDisplay.gameObject.SetActive(showHpAfter);
        }, walkTime - 0.1f);
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