using System;
using UnityEngine;

namespace Equipment
{
    [Serializable]
    public class Equip
    {
        public Sprite sprite, trim, grounded;
        public Color color, trimColor;
        public EquipmentSlot slot;
        public bool noFlip;
        public float groundAngle;

        public Equip(Blueprint blueprint)
        {
            sprite = blueprint.sprite;
            trim = blueprint.trim;
            grounded = blueprint.grounded ? blueprint.grounded : blueprint.sprite;
            color = blueprint.colors.Random();
            trimColor = blueprint.trimColors.Random();
            slot = blueprint.slot;
            groundAngle = blueprint.groundAngle;
        }
    }
}