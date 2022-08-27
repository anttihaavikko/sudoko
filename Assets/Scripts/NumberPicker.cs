using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using UnityEngine;

public class NumberPicker : MonoBehaviour
{
    [SerializeField] private List<NumberButton> buttons;

    public int Number { get; private set; }

    private NumberButton current;

    private void Start()
    {
        SelectNumber(StateManager.Instance.SelectedNumber);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Z))
        {
            SelectNumber(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.X))
        {
            SelectNumber(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.C))
        {
            SelectNumber(3);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.A))
        {
            SelectNumber(4);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.S))
        {
            SelectNumber(5);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.D))
        {
            SelectNumber(6);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Q))
        {
            SelectNumber(7);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.W))
        {
            SelectNumber(8);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.E))
        {
            SelectNumber(9);
        }
    }

    public void SelectNumber(int number)
    {
        if (current)
        {
            current.DeSelect();
        }
        
        current = buttons[number - 1];
        current.Select();
        Number = number;
        StateManager.Instance.SelectedNumber = Number;
    }
}