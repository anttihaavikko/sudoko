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
    public int Health { get; set; } = 20;
    public int Gold { get; set; } = 100;

    private readonly List<Equip> gear = new();
    private readonly List<Equip> inventory = new();

    public List<Equip> Gear => gear.ToList();
    public List<Equip> Inventory => inventory.ToList();

    public int Score { get; set; }
    public int Multiplier { get; set; } = 1;
    public bool ExtraBoss { get; set; }
    public int SelectedNumber { get; set; } = 1;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        Level++;
        SceneChanger.Instance.ChangeScene("Map");
    }

    public void UpdateEquips(IEnumerable<Equip> equips, IEnumerable<Equip> inv)
    {
        gear.Clear();
        inventory.Clear();
        gear.AddRange(equips);
        inventory.AddRange(inv);
    }

    public void Clear()
    {
        Level = 0;
        Score = 0;
        Multiplier = 1;
        Gold = 100;
        gear.Clear();
        inventory.Clear();
    }
}