using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Managers;
using Equipment;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : Manager<StateManager>
{
    public int Level { get; private set; }
    public int Health { get; set; } = 100;

    private readonly List<Equip> gear = new();
    private readonly List<Equip> inventory = new();

    public List<Equip> Gear => gear.ToList();
    public List<Equip> Inventory => inventory.ToList();
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        Level++;
        SceneChanger.Instance.ChangeScene("Main");
    }

    public void UpdateEquips(IEnumerable<Equip> equips, IEnumerable<Equip> inv)
    {
        gear.Clear();
        inventory.Clear();
        gear.AddRange(equips);
        inventory.AddRange(inv);
    }
}