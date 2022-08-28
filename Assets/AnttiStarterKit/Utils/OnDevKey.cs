using System;
using UnityEngine;
using UnityEngine.Events;

namespace AnttiStarterKit.Utils
{
    public class OnDevKey : MonoBehaviour
    {
        [SerializeField] private KeyCode key;
        [SerializeField] private UnityEvent action;
        [SerializeField] private bool buildAlso;

        private void Update()
        {
            if (DevKey.Down(key) || buildAlso)
            {
                action?.Invoke();
            }
        }
    }
}