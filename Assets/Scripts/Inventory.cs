using System;
using System.Collections.Generic;
using System.Linq;
using Equipment;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private Transform container, root;
    [SerializeField] private InventoryIcon iconPrefab;
    [SerializeField] private List<Transform> slots;

    public Transform Container => container;

    private List<InventoryIcon> icons = new();

    private void Start()
    {
        player.GetEquips().ForEach(e => Add(e));
        player.GetInventory().ForEach(AddToInventory);
    }

    public void Strip(Equip e)
    {
        player.Remove(e);
    }

    public void Return(Equip e)
    {
        var icon = icons.FirstOrDefault(i => i.Has(e));
        if (icon)
        {
            icon.transform.SetParent(container, false);
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
        t.position = slots[index].position;
    }
    
    public void AddToInventory(Equip e)
    {
        var icon = Instantiate(iconPrefab, container);
        icons.Add(icon);
        icon.Setup(root, e);
    }
}