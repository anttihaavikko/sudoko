using System;
using AnttiStarterKit.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class MapState : Manager<MapState>
    {
        public int x = 0;
        public int y = 0;

        private int seed = -1;

        public int World { get; private set; }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void ReSeed()
        {
            seed = Random.Range(0, int.MaxValue);
        }

        public void NextWorld()
        {
            x = 0;
            y = 0;
            ReSeed();
            World++;
        }

        public void Seed()
        {
            if (seed < 0)
            {
                ReSeed();
            }
            
            Random.InitState(seed);
        }

        public void Clear()
        {
            x = 0;
            y = 0;
            ReSeed();
        }
    }
}