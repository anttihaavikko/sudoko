using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private List<Skill> skills = new();

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

        public void AddSkill(Skill s)
        {
            skills.Add(s.Copy());
        }

        public string GetName()
        {
            return skills.First().Decorate(sprite.name);
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
    }
}