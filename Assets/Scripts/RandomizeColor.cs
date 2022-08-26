using System.Collections.Generic;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    [SerializeField] private ColorCollection colors;
    [SerializeField] private List<SpriteRenderer> others;

    private void Start()
    {
        var color = colors.Random();
        GetComponent<SpriteRenderer>().color = color;
        others.ForEach(s => s.color = color);
    }
}