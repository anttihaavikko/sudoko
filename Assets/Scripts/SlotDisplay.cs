using System.Collections.Generic;
using Equipment;
using UnityEngine;

public class SlotDisplay : MonoBehaviour
{
    [SerializeField] private List<SoulSlot> slots;

    public void Show(Equip e)
    {
        var amount = e.SlotCount;
        var i = 0;
        
        slots.ForEach(s =>
        {
            s.gameObject.SetActive(i < amount);
            s.Fill(e.GetSoulColor(i));
            i++;
        });
    }
}