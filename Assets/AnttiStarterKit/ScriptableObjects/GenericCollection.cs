using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

namespace AnttiStarterKit.ScriptableObjects
{
    public class GenericCollection<T> : ScriptableObject
    {
        [SerializeField] private List<T> items;

        public int Count => items.Count;

        public T Random()
        {
            return !items.Any() ? default : items.Random();
        }

        public IEnumerable<T> Random(int amount)
        {
            return !items.Any() ? new List<T>() : items.OrderBy(_ => UnityEngine.Random.value).Take(amount);
        }

        public T Get(int index)
        {
            return items[index];
        }

        public List<T> ToList()
        {
            return items;
        }
    }
}