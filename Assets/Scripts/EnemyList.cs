using System;
using System.Collections.Generic;
using AnttiStarterKit.Extensions;
using UnityEngine;

[Serializable]
public class EnemyList
{
    [SerializeField] private List<Character> enemies;

    public Character Random()
    {
        return enemies.Random();
    }
}