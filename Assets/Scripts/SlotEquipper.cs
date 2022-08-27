using System;
using AnttiStarterKit.Managers;
using Equipment;
using UnityEngine;
using UnityEngine.UI;

public class SlotEquipper : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Character player;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Image rim;
    [SerializeField] private Image icon;

    private Color baseColor;

    private void Start()
    {
        baseColor = rim.color;
    }

    public bool CanSlot(Equip e)
    {
        return e.IsSoul ? 
            player.CanSlotSoul(slotIndex) : 
            player.CanSlot(e, slotIndex);
    }

    public bool Matches(Equip e)
    {
        return e.IsSoul && player.CanSlotSoul(slotIndex) || player.SlotMatches(e, slotIndex);
    }

    public bool Add(Equip e)
    {
        if (!CanSlot(e)) return false;
        
        var pos = Vector3.zero;
        AudioManager.Instance.PlayEffectFromCollection(7, pos, 0.4f);
        AudioManager.Instance.PlayEffectFromCollection(4, pos, 0.3f);
        AudioManager.Instance.PlayEffectFromCollection(6, pos, 1.5f);

        if (e.IsSoul)
        {
            inventory.Hide(e);
            player.Slot(e, slotIndex);
            return true;
        }
        
        player.Remove(e);
        
        var prev = player.Remove(slotIndex);
        if (prev != null)
        {
            inventory.Return(prev);
        }

        player.Add(e, slotIndex);
        
        return true;
    }

    public void Mark(bool state)
    {
        rim.color = icon.color = state ? 
            Color.white :
            baseColor;
    }
}