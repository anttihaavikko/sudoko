using System;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

namespace Equipment
{
    [Serializable]
    public class Blueprint
    {
        public string title;
        public Sprite sprite, trim, grounded;
        public bool canFlip;
        public ColorCollection colors, trimColors;
        public EquipmentSlot slot;
        public float groundAngle;
        public bool masked;
        public float groundOffset;
        public bool isSword;
    }
}