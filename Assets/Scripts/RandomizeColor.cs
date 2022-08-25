using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    [SerializeField] private ColorCollection colors;
    
    private void Start()
    {
        GetComponent<SpriteRenderer>().color = colors.Random();
    }
}