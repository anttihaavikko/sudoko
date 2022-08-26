using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomText : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TMP_Text>().text = Namer.GenerateName(Random.Range(0, 5));
    }
}