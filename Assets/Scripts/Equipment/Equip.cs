using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnttiStarterKit.Utils;
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
        public bool flipped;
        public float groundAngle;
        public bool masked;
        public float groundOffset;

        private List<Skill> skills = new();
        private int slotCount;
        private int goldAmount;

        private List<Equip> slots = new();

        public int SlotCount => slotCount;

        public bool HasFreeSlot => slots.Count < slotCount;

        public bool IsSoul => slot == EquipmentSlot.Soul;

        public int Gold => goldAmount;

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
            flipped = blueprint.canFlip && Random.value < 0.5f;
            masked = blueprint.masked;
            groundOffset = blueprint.groundOffset;

            slotCount = GetRandomSlotCount();

            if (slot == EquipmentSlot.Gold)
            {
                goldAmount = Random.Range(100, 300);
            } 
        }

        public void Slot(Equip e)
        {
            if (HasFreeSlot)
            {
                slots.Add(e);
            }
        }

        public bool IsConsumable()
        {
            return slot is EquipmentSlot.Gold or EquipmentSlot.Potion;
        }

        public void AddSkill(Skill s)
        {
            if (IsConsumable()) return;
            skills.Add(s.Copy());
        }

        public string GetName()
        {
            return !skills.Any() ? title : skills.First().Decorate(title);
        }

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append($"<size=30>{GetName()}</size>");
            sb.Append("<size=5>\n\n</size>");
            skills.ForEach(s => sb.Append($"{s.GetDescription()}<size=5>\n\n</size>"));
            
            slots.SelectMany(s => s.skills).ToList().ForEach(s => sb.Append($"{s.GetDescription()}<size=5>\n\n</size>"));
            
            if (slot == EquipmentSlot.Soul)
            {
                const string text = "Souls need to be inserted to a piece of equipment with a free socket. They can not be removed afterwards.";
                sb.Append(TextUtils.TextWith(text, Color.gray, 15));
            }
            
            return sb.ToString();
        }

        public IEnumerable<Skill> GetSkills()
        {
            return skills.Concat(slots.SelectMany(s => s.skills));
        }

        private int GetRandomSlotCount()
        {
            if (slot == EquipmentSlot.Soul) return 0;
            
            return Random.value switch
            {
                < 0.1f => 3,
                < 0.25f => 2,
                < 0.5f => 1,
                _ => 0
            };
        }

        public Color GetSoulColor(int i)
        {
            return slots.Count <= i ? Color.clear : slots[i].color;
        }

        public void AddGold(int amount)
        {
            goldAmount += amount;
        }
    }
}