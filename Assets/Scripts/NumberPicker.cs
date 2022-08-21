using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;

public class NumberPicker : MonoBehaviour
{
    [SerializeField] private List<Transform> buttons;
    [SerializeField] private Transform marker;

    public int Number { get; private set; }

    private void Start()
    {
        Number = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectNumber(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectNumber(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectNumber(3);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectNumber(4);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectNumber(5);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectNumber(6);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectNumber(7);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectNumber(8);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectNumber(9);
        }
    }

    public void SelectNumber(int number)
    {
        Tweener.MoveToBounceOut(marker, buttons[number - 1].position, 0.2f);
        Number = number;
    }
}