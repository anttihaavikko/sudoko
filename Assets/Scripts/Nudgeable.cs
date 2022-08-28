using System;
using System.Runtime.CompilerServices;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

public class Nudgeable : MonoBehaviour
{
    [SerializeField] private float amount = 5f;
    [SerializeField] private bool returns = true;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float moveAmount = 0.1f;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private SoundCollection sound;

    private Transform t;
    private Quaternion startRotation;
    private Vector3 startPos;
    private bool initialized;

    private void Initialize()
    {
        t = transform;
        startRotation = t.rotation;
        startPos = t.position;
    }

    public void Nudge(float from)
    {
        if (!initialized)
        {
            Initialize();
        }
        
        var direction = from < t.position.x ? -1 : 1;
        var target = startRotation * Quaternion.Euler(0, 0, direction * amount);
        Tweener.RotateToBounceOut(t, target, duration);

        if (moveAmount > 0)
        {
            Tweener.MoveToQuad(t, t.position.WhereX(startPos.x - direction * moveAmount), duration);   
        }

        if (returns)
        {
            Invoke(nameof(Return), duration);
        }

        if (particles)
        {
            particles.Play();
        }

        if (sound)
        {
            AudioManager.Instance.PlayEffectFromCollection(sound, t.position);
        }
    }

    private void Return()
    {
        Tweener.RotateToQuad(transform, startRotation, duration * 2f);
        
        if (moveAmount > 0)
        {
            Tweener.MoveToQuad(t, t.position.WhereX(startPos.x), duration);   
        }
    }
}