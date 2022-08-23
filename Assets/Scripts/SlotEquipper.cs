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
        return player.CanSlot(e, slotIndex);
    }

    public bool Matches(Equip e)
    {
        return player.SlotMatches(e, slotIndex);
    }

    public bool Add(Equip e)
    {
        if (!CanSlot(e)) return false;
        
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