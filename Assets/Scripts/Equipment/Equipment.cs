using System;
using UnityEngine;

namespace Equipment
{
    [Serializable]
    public class Equipment
    {
        public Sprite sprite, trim;
        public Color color, trimColor;
        public EquipmentSlot slot;
        public bool noFlip;

        public Equipment(Blueprint blueprint)
        {
            sprite = blueprint.sprite;
            trim = blueprint.trim;
            color = blueprint.colors.Random();
            trimColor = blueprint.trimColors.Random();
            slot = blueprint.slot;
        }
    }
}