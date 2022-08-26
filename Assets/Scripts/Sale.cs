using System;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Utils;
using Equipment;
using Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Sale : MonoBehaviour
{
    [SerializeField] private GameObject contents;
    [SerializeField] private RectTransform priceContainer;
    [SerializeField] private TMP_Text priceLabel;
    [SerializeField] private Character player;
    [SerializeField] private ScoreDisplay goldDisplay;
    [SerializeField] private Shop shop;
    [SerializeField] private EquipmentList equipmentList;
    [SerializeField] private SkillSet weaponSkills, armorSkills, soulSkills;
    [SerializeField] private InventoryIcon icon;

    private Equip equip;
    private int price;

    private void Start()
    {
        var slot = GetLootType();
        equip = equipmentList.Random(slot);

        var skills = Random.value < 0.2f ? MapState.Instance.World + 2 : 1;
        
        for (var i = 0; i < skills; i++)
        {
            equip.AddSkill(GetSkillSetFor(slot).Random());
        }
        
        icon.Setup(transform, equip);

        price = Mathf.RoundToInt(equip.GetPrice() * 1.5f);
        ShowPrice();
    }

    private void ShowPrice()
    {
        priceLabel.text = price.AsScore();
        LayoutRebuilder.ForceRebuildLayoutImmediate(priceContainer);
    }

    public void Buy()
    {
        if (goldDisplay.Total <= price)
        {
            shop.IndicateLowGold();
            return;
        }

        shop.Thank();
        contents.SetActive(false);
        goldDisplay.Add(-price);
        StateManager.Instance.Gold = goldDisplay.Total;
        Inventory.Instance.Add(equip);
        player.RecalculateStats();
    }
    
    private EquipmentSlot GetLootType()
    {
        var ignore = new[]
        {
            EquipmentSlot.None,
            EquipmentSlot.Pants,
            EquipmentSlot.Shirt,
            EquipmentSlot.Potion,
            EquipmentSlot.Gold
        };
        
        return EnumUtils.ToList<EquipmentSlot>()
            .Where(s => !ignore.Contains(s))
            .ToList()
            .Random();
    }
    
    private SkillSet GetSkillSetFor(EquipmentSlot slot)
    {
        if (slot == EquipmentSlot.Soul) return soulSkills;
        return slot == EquipmentSlot.Weapon ? weaponSkills : armorSkills;
    }
}