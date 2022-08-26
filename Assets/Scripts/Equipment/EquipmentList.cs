using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "Equipment", order = 0)]
    public class EquipmentList : ScriptableObject
    {
        [SerializeField] private List<Blueprint> hats;
    
        public Equip Random()
        {
            return new Equip(hats.Random());
        }

        public Equip Random(EquipmentSlot slot)
        {
            return new Equip(hats.Where(h => h.slot == slot).ToList().Random());
        }

        public int RandomIndex(EquipmentSlot slot)
        {
            var bp = hats.Where(h => h.slot == slot).ToList().Random();
            return hats.IndexOf(bp);
        }

        public Equip Get(int index)
        {
            return new Equip(hats[index]);
        }
    }
}