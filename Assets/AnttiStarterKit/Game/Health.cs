using System;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AnttiStarterKit.Game
{
    [Serializable]
    public class Health
    {
        [SerializeField] private int cur = 10, max = 10;
        [SerializeField] private TMP_Text display;
        [SerializeField] private Transform bar;
        
        [SerializeField] private UnityEvent onDeath;
        [SerializeField] private UnityEvent onDamage;

        public Action<HealthValue> changed;

        public int Current => cur;
        public int Max => max;
        public float Ratio => 1f * cur / max;

        public void Init()
        {
            UpdateDisplays();
        }

        public void Init(int hp)
        {
            cur = hp;
            UpdateDisplays();
        }

        public void TakeDamage(int amount, GameObject source = null)
        {
            cur = Mathf.Max(cur - amount, 0);
            changed?.Invoke(Get());
            onDamage?.Invoke();

            if (cur <= 0)
            {
                onDeath?.Invoke();
            }

            UpdateDisplays();
        }

        public void Heal(int amount)
        {
            cur = Mathf.Min(cur + amount, max);
            changed?.Invoke(Get());
            UpdateDisplays();
        }

        public void SetMax(int amount)
        {
            max = amount;
        }

        public void Cap()
        {
            if (cur > max)
            {
                cur = max;
            }
        }

        public void AddMax(int amount, bool heals)
        {
            max += amount;
            if (heals)
            {
                cur = Mathf.Min(cur + amount, max);
            }
            UpdateDisplays();
        }

        private void UpdateDisplays()
        {
            if (display)
            {
                display.text = $"{cur}/{max}";
            }

            if (bar)
            {
                Tweener.ScaleToBounceOut(bar, new Vector3(1f * cur / max, 1, 1), 0.2f);
            }
        }

        public HealthValue Get()
        {
            return new HealthValue(cur, max);
        }
    }

    public struct HealthValue
    {
        public int current;
        public int max;
        public float ratio;

        public HealthValue(int current, int max)
        {
            this.current = current;
            this.max = max;
            this.ratio = 1f * current / max;
        }
    }
}