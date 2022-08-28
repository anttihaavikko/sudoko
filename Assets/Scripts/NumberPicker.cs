using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using UnityEngine;

public class NumberPicker : MonoBehaviour
{
    [SerializeField] private List<NumberButton> buttons;
    [SerializeField] private Appearer tutorial;
    [SerializeField] private Board board;

    public int Number { get; private set; }

    private NumberButton current;
    private bool tutorialVisible;

    private void Start()
    {
        Select(StateManager.Instance.SelectedNumber);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Z))
        {
            SelectNumberFromKeyBoard(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.X))
        {
            SelectNumberFromKeyBoard(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.C))
        {
            SelectNumberFromKeyBoard(3);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.A))
        {
            SelectNumberFromKeyBoard(4);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.S))
        {
            SelectNumberFromKeyBoard(5);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.D))
        {
            SelectNumberFromKeyBoard(6);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Q))
        {
            SelectNumberFromKeyBoard(7);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.W))
        {
            SelectNumberFromKeyBoard(8);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.E))
        {
            SelectNumberFromKeyBoard(9);
        }
    }

    private void SelectNumberFromKeyBoard(int number)
    {
        Select(number);

        if (tutorialVisible)
        {
            tutorial.HideWithDelay(0.5f);
        }
    }

    public void SelectNumber(int number)
    {
        Select(number);

        if (!board.ShowingTutorial && !StateManager.Instance.KeyboardUsed)
        {
            tutorial.ShowAfter(1f);
            tutorialVisible = true;
            StateManager.Instance.KeyboardUsed = true;
        }
    }
    
    private void Select(int number)
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