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

    private Vector3 origin;
    private EffectCamera cam;

    private void Start()
    {
        cam = Camera.main.GetComponent<EffectCamera>();
        origin = transform.position;
        
        health.Init();
        
        if (transform.localScale.x < 0f)
        {
            mirrorables.ForEach(m => m.Mirror());   
        }

        healthDisplay.SetParent(null, true);
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public void Damage(int amount)
    {
        cam.BaseEffect(0.2f);
        flasher.Flash();
        health.TakeDamage(amount);
    }

    public void Attack(Character target, int amount)
    {
        var t = transform;
        Tweener.MoveToBounceOut(t, origin + Vector3.right * 1.5f * t.localScale.x, 0.2f);
        this.StartCoroutine(() => target.Damage(amount), 0.15f);
        this.StartCoroutine(() => Tweener.MoveToQuad(t, origin, 0.2f), 0.25f);
    }

    public bool IsAlive()
    {
        return health.Current > 0;
    }
}