using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Equipment
{
    [Serializable]
    public class Equip
    {
        public string title;
        public Sprite sprite, trim, grounded;
        public Color color, trimColor;
        public EquipmentSlot slot;
        public bool noFlip;
        public float groundAngle;

        private List<Skill> skills = new();
        private int slotCount;

        public int SlotCount => slotCount;

        public Equip(Blueprint blueprint)
        {
            title = blueprint.title;
            sprite = blueprint.sprite;
            trim = blueprint.trim;
            grounded = blueprint.grounded ? blueprint.grounded : blueprint.sprite;
            color = blueprint.colors.Random();
            trimColor = blueprint.trimColors.Random();
            slot = blueprint.slot;
            groundAngle = blueprint.groundAngle;

            slotCount = GetRandomSlotCount();
        }

        public void AddSkill(Skill s)
        {
            skills.Add(s.Copy());
        }

        public string GetName()
        {
            return skills.First().Decorate(title);
        }

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append($"<size=30>{GetName()}</size>");
            sb.Append("<size=5>\n\n</size>");
            skills.ForEach(s => sb.Append($"{s.GetDescription()}<size=5>\n\n</size>"));
            return sb.ToString();
        }

        public IEnumerable<Skill> GetSkills()
        {
            return skills;
        }

        private int GetRandomSlotCount()
        {
            return Random.value switch
            {
                < 0.1f => 3,
                < 0.25f => 2,
                < 0.5f => 1,
                _ => 0
            };
        }
    }
}