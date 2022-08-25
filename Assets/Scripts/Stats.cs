using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stats
{
    public int attack;
    public int defence;
    public int speed;

    public void Add(List<Stats> adds)
    {
        attack += adds.Sum(a => a.attack);
        defence += adds.Sum(a => a.defence);
        speed += adds.Sum(a => a.speed);
    }

    public Stats Copy()
    {
        return new Stats
        {
            attack = attack,
            defence = defence,
            speed = speed
        };
    }

    public void Cap()
    {
        attack = Mathf.Max(0, attack);
        defence = Mathf.Max(0, defence);
    }
}