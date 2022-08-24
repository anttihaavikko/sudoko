using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text attack, defense;
    
    public void Set(Stats stats)
    {
        attack.text = stats.attack.ToString();
        defense.text = stats.defence.ToString();
    }
}