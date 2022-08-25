using System;
using System.Collections;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using Equipment;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chest : Lootable
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Looter looter;
    [SerializeField] private EquipmentList equipmentList;
    [SerializeField] private Character player;
    [SerializeField] private Appearer continueButton;
    
    private Animator anim;

    private static readonly int Open = Animator.StringToHash("open");

    private void Start()
    {
        anim = GetComponent<Animator>();
        
        looter.SetSource(this);
        
        GenerateLoot();

        StartCoroutine(OpenAndLoot());
    }

    private void GenerateLoot()
    {
        var type = GetLootType();

        for (var i = 0; i < Random.Range(1, 4); i++)
        {
            drops.Add(equipmentList.Random(type));
        }
    }

    private EquipmentSlot GetLootType()
    {
        return EnumUtils.ToList<EquipmentSlot>()
            .Where(s => s != EquipmentSlot.None && s != EquipmentSlot.Pants && s != EquipmentSlot.Shirt)
            .ToList()
            .Random();
    }

    private IEnumerator OpenAndLoot()
    {
        yield return new WaitForSeconds(2f);
        player.AttackAnimation();
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger(Open);
        yield return new WaitForSeconds(0.2f);
        looter.GenerateDrops();
        yield return new WaitForSeconds(1f);
        yield return looter.LootDrops();
        continueButton.Show();
    }
}