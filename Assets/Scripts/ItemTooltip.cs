using System;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using Equipment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : Manager<ItemTooltip>
{
    [SerializeField] private TMP_Text content;
    [SerializeField] private GameObject node;
    [SerializeField] private GameObject slotContainer;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private SlotDisplay slotDisplay;
    [SerializeField] private Transform mirrorContainer;

    private void Start()
    {
        node.SetActive(false);
    }

    public void Show(Vector3 pos, Equip e)
    {
        var hasSlots = e.SlotCount > 0;
        slotContainer.SetActive(hasSlots);

        if (hasSlots)
        {
            slotDisplay.Show(e);
        }

        transform.position = pos;
        transform.SetAsLastSibling();
        content.text = e.GetDescription();
        node.SetActive(true);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        
        var mirrored = transform.position.x > Screen.width * 0.6f ? -1 : 1;
        transform.localScale = node.transform.localScale = mirrorContainer.localScale = Vector3.one.WhereX(mirrored); 
    }

    public void Hide()
    {
        node.SetActive(false);
    }
}