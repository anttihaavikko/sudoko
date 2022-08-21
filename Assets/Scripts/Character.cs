using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Visuals;

public class Character : MonoBehaviour
{
    [SerializeField] private Flasher flasher;
    [SerializeField] private Health health;
    [SerializeField] private List<Transform> mirrorables;
    [SerializeField] private Transform healthDisplay;
    [SerializeField] private GameObject root, shadow;
    [SerializeField] private bool isPlayer;

    private Animator anim;

    private Vector3 origin;
    private EffectCamera cam;
    
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int AttackAnim = Animator.StringToHash("attack");
    private static readonly int Hurt = Animator.StringToHash("hurt");

    private void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main.GetComponent<EffectCamera>();
        origin = transform.position;
        
        health.Init();
        
        if (transform.localScale.x < 0f)
        {
            mirrorables.ForEach(m => m.Mirror());   
        }

        healthDisplay.SetParent(null, true);


        if (isPlayer)
        {
            healthDisplay.gameObject.SetActive(false);
            anim.SetBool(Walking, true);
            var t = transform;
            var walkTime = 1.75f;
            t.position += Vector3.left * 7;
            Tweener.MoveToQuad(t, origin, walkTime);
            this.StartCoroutine(() =>
            {
                anim.SetBool(Walking, false);
                healthDisplay.gameObject.SetActive(true);
            }, walkTime - 0.1f);
        }
    }

    public void Die()
    {
        cam.BaseEffect(0.4f);
        root.SetActive(false);
        shadow.SetActive(false);
        this.StartCoroutine(() => healthDisplay.gameObject.SetActive(false), 0.3f);
    }

    public void Damage(int amount)
    {
        anim.SetTrigger(Hurt);
        cam.BaseEffect(0.2f);
        flasher.Flash();
        health.TakeDamage(amount);
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
}