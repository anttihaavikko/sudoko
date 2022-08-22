using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Managers;
using Equipment;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : Manager<StateManager>
{
    public int Level { get; private set; }
    public int Health { get; set; } = 100;

    private List<Equip> gear = new();

    public List<Equip> Gear => gear.ToList();
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        Level++;
        SceneChanger.Instance.ChangeScene("Main");
    }

    public void AddGear(Equip e)
    {
        gear.Add(e);
    }
}