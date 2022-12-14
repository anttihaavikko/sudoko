using System;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AnttiStarterKit.Animations
{
    public class Appearer : MonoBehaviour
    {
        [SerializeField] private SoundCollection soundCollection;
        
        public float appearAfter = -1f;
        public float hideDelay;
        public bool silent;
        public GameObject visuals;
        public bool inScreenSpace;
        public Camera cam;
        
        public TMP_Text text;
        private Vector3 size;
        private bool visible;

        private void Awake()
        {
            var t = transform;
            size = t.localScale;
            t.localScale = Vector3.zero;
            
            if(visuals) visuals.SetActive(false);

            if (appearAfter >= 0)
                Invoke(nameof(Show), appearAfter);
        }

        public void ShowAfter()
        {
            Invoke(nameof(Show), appearAfter);
        }

        public void ShowAfter(float delay)
        {
            Invoke(nameof(Show), delay);
        }

        public void Show()
        {
            visible = true;
            CancelInvoke(nameof(Hide));
            CancelInvoke(nameof(MakeInactive));
            DoSound();

            if(visuals) visuals.SetActive(true);
            Tweener.Instance.ScaleTo(transform, size, 0.3f, 0f, TweenEasings.BounceEaseOut);
        }

        public void Hide()
        {
            visible = false;
            CancelInvoke(nameof(Show));
            DoSound();

            Tweener.Instance.ScaleTo(transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
        
            if(visuals) Invoke(nameof(MakeInactive), 0.2f);
        }

        private void MakeInactive()
        {
            visuals.SetActive(false);
        }

        private void DoSound()
        {
            if (silent) return;

            var p = transform.position;
            var pos = inScreenSpace && cam ? cam.ScreenToWorldPoint(p) : p;

            if (soundCollection)
            {
                AudioManager.Instance.PlayEffectFromCollection(soundCollection, pos);
            }
        }

        public void HideWithDelay()
        {
            Invoke(nameof(Hide), hideDelay);
        }
        
        public void HideWithDelay(float delay)
        {
            Invoke(nameof(Hide), delay);
        }

        public void ShowWithText(string t, float delay)
        {
            if (text)
                text.text = t;

            Invoke(nameof(Show), delay);
        }

        public void Toggle()
        {
            if (visible)
            {
                Hide();
                return;
            }

            Show();
        }
    }
}
