using System;
using AnttiStarterKit.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : Manager<StateManager>
{
    public int Level { get; private set; }
    public int Health { get; set; } = 100;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        Level++;
        SceneChanger.Instance.ChangeScene("Main");
    }
}