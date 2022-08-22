using Equipment;
using UnityEngine;

public class SlotEquipper : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Character player;
    [SerializeField] private Inventory inventory;

    public bool Add(Equip e)
    {
        if (!player.CanSlot(e, slotIndex)) return false;
        
        player.Remove(e);
        
        var prev = player.Remove(slotIndex);
        if (prev != null)
        {
            inventory.Return(prev);
        }

        player.Add(e, slotIndex);
        
        return true;
    }
}