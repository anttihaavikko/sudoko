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
    
        public Equipment Random()
        {
            return new Equipment(hats.Random());
        }

        public Equipment Random(EquipmentSlot slot)
        {
            return new Equipment(hats.Where(h => h.slot == slot).ToList().Random());
        }
    }
}