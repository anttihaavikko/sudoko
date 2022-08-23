using Equipment;
using UnityEngine;
using UnityEngine.UI;

public class SlotEquipper : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Character player;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Image rim;

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
        rim.color = state ? 
            new Color(1f, 1f, 1f, 0.75f) :
            new Color(1f, 1f, 1f, 0.25f);
    }
}