using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using Equipment;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Manager<Inventory>
{
    [SerializeField] private Character player;
    [SerializeField] private Transform container, root;
    [SerializeField] private InventoryIcon iconPrefab;
    [SerializeField] private List<SlotEquipper> slots;
    [SerializeField] private EquipmentList equipmentList;
    [SerializeField] private SkillSet soulSkills;
    [SerializeField] private ScoreDisplay goldDisplay;

    public Transform Container => container;

    private readonly List<InventoryIcon> icons = new();
    private RectTransform goldDisplayContainer;

    private void Update()
    {
        if (DevKey.Down(KeyCode.G))
        {
            var soul = equipmentList.Random(EquipmentSlot.Soul);
            soul.AddSkill(soulSkills.Random().Copy());
            Add(soul);
        }
        
        if (DevKey.Down(KeyCode.H))
        {
            var soul = equipmentList.Random(EquipmentSlot.Hat);
            soul.AddSkill(soulSkills.Random().Copy());
            Add(soul);
        }
    }

    public void MarkSlot(Equip e)
    {
        var marked = slots.Where(s => s.Matches(e)).ToList();
        marked.ForEach(s => s.Mark(true));
    }

    private void Start()
    {
        player.GetEquips().ForEach(e => Add(e));
        player.GetInventory().ForEach(AddToInventory);

        if (goldDisplay)
        {
            goldDisplay.Set(StateManager.Instance.Gold);
            goldDisplayContainer = goldDisplay.GetComponent<RectTransform>();
            ResizeGoldPanel();
        }
    }

    private void ResizeGoldPanel()
    {
        if (goldDisplayContainer)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(goldDisplayContainer);
        }
    }

    public void UpdateSlotsFor(Equip e)
    {
        var icon = icons.FirstOrDefault(i => i.Has(e));
        if (icon)
        {
            icon.UpdateSlots();
        }
    }

    public void Strip(Equip e)
    {
        player.Remove(e);
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        player.RecalculateStats();
    }

    public void Return(Equip e)
    {
        var icon = icons.FirstOrDefault(i => i.Has(e));
        if (icon)
        {
            icon.transform.SetParent(container, false);
        }
    }

    public void Hide(Equip e)
    {
        var icon = icons.FirstOrDefault(i => i.Has(e));
        if (icon)
        {
            icon.gameObject.SetActive(false);
        }
    }

    public void Add(Equip e, bool isNew = false)
    {
        var index = player.Add(e);
        var icon = Instantiate(iconPrefab, container);
        icons.Add(icon);
        icon.Setup(root, e, isNew);
        icon.transform.SetAsFirstSibling();

        if (index < 0) return;
        
        var t = icon.transform;
        t.SetParent(root, true);
        t.position = slots[index].transform.position;
    }
    
    public void AddToInventory(Equip e)
    {
        var icon = Instantiate(iconPrefab, container);
        icons.Add(icon);
        icon.Setup(root, e);
    }

    public void UnMarkSlots()
    {
        slots.ForEach(s => s.Mark(false));
    }

    public void AddGold(int amount)
    {
        goldDisplay.Add(amount);
        StateManager.Instance.Gold = goldDisplay.Total;
        ResizeGoldPanel();
    }
}