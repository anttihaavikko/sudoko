using System;
using System.Collections.Generic;
using System.Linq;

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
}