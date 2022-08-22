using System;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

namespace Equipment
{
    [Serializable]
    public class Blueprint
    {
        public Sprite sprite, trim;
        public ColorCollection colors, trimColors;
        public EquipmentSlot slot;
    }
}