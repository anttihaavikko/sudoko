using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;

namespace AnttiStarterKit.Game
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text valueField, additionField, multiField;
        [SerializeField] private Appearer additionAppearer;

        [SerializeField] private float minSpeed = 0.3f;
        [SerializeField] private float maxSpeed = 3f;
        [SerializeField] private float additionShowTime = 2.5f;

        [SerializeField] private bool separateThousands = true;

        private Pulsater multiPulsate;

        private int value;
        private float shownValue;
        private int addition;
        private int multiplier = 1;

        public int Total => value;
        public int Multi => multiplier;

        private void Start()
        {
            if (multiField)
            {
                multiPulsate = multiField.GetComponent<Pulsater>();   
            }
        }

        public void Set(int amount, int multi = 1)
        {
            value = amount;
            multiplier = multi;
            shownValue = amount;
            valueField.text = Format(value);
            ShowMulti();
        }

        private string Format(int number)
        {
            return separateThousands ? number.AsScore() : number.ToString();
        }

        private void ShowMulti()
        {
            if (!multiField) return;
            
            multiField.text = $"x{multiplier}";

            if (multiPulsate)
            {
                multiPulsate.Pulsate();
            }
        }

        private void Update()
        {
            if (Mathf.Abs(shownValue - value) < 0.1f) return;
            var speed = Mathf.Max(Mathf.Abs(value - shownValue) * Time.deltaTime * maxSpeed, minSpeed);
            shownValue = Mathf.MoveTowards(shownValue, value, speed);
            valueField.text = Format(Mathf.RoundToInt(shownValue));
        }

        private string GetAdditionAsText()
        {
            var number = separateThousands ? addition.AsScore() : addition.ToString();
            return addition > 0 ? $"+{number}" : number;
        }

        public void Add(int amount)
        {
            var amt = amount * multiplier;
        
            value += amt;
            addition += amt;

            additionField.text = GetAdditionAsText();
            additionAppearer.Show();
            
            CancelInvoke(nameof(ClearAddition));
            Invoke(nameof(ClearAddition), additionShowTime);
        }

        public void AddMulti()
        {
            multiplier++;
            ShowMulti();
        }

        public void ResetMulti()
        {
            multiplier = 1;
            ShowMulti();
        }

        public void DecreaseMulti()
        {
            if (multiplier > 1)
            {
                multiplier--;
                ShowMulti();
            }
        }

        private void ClearAddition()
        {
            additionAppearer.Hide();
            addition = 0;
        }
    }
}