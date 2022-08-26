using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> attack, defense;
    
    public void Set(Stats stats)
    {
        attack.ForEach(a => a.text = stats.attack.ToString());
        defense.ForEach(d => d.text = stats.defence.ToString());
    }
}