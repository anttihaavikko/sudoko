using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using Equipment;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private LayerMask dropMask;
    [SerializeField] private Image image, trim;
    [SerializeField] private GameObject newIndicator;
    [SerializeField] private SlotDisplay slotDisplay;

    private bool dragging;
    private Transform parent;
    private Vector3 start;
    private Transform rootElement;
    private Equip equip;

    public void Setup(Transform root, Equip e, bool isNew = false)
    {
        rootElement = root;
        
        image.sprite = e.grounded;
        image.transform.Mirror(e.flipped);
        image.color = e.color;
        trim.sprite = e.trim;
        trim.transform.Mirror(e.flipped);
        trim.color = e.trim ? e.trimColor : Color.clear;

        equip = e;
        
        newIndicator.SetActive(isNew);
        
        slotDisplay.Show(e);
    }

    public void UpdateSlots()
    {
        slotDisplay.Show(equip);
    }

    private void Update()
    {
        if (dragging)
        {
            transform.position = Input.mousePosition.WhereZ(0);
        }
    }

    public void Hover()
    {
        if (dragging) return;
        Inventory.Instance.MarkSlot(equip);
        newIndicator.SetActive(false);
        ItemTooltip.Instance.Show(transform.position, equip);
    }

    public void HoverOut()
    {
        Inventory.Instance.UnMarkSlots();
        ItemTooltip.Instance.Hide();
    }

    public void MouseDown()
    {
        ItemTooltip.Instance.Hide();
        
        var t = transform;
        start = t.position;
        parent = t.parent;
        transform.SetParent(rootElement, true);
        dragging = true;
        
        t.SetAsLastSibling();
    }

    public bool Has(Equip e)
    {
        return equip == e;
    }

    public void MouseUp()
    {
        var t = transform;
        dragging = false;

        var hit = Physics2D.OverlapCircle(Input.mousePosition.WhereZ(0), 1f, dropMask);
        if (hit)
        {
            var inventory = hit.GetComponent<Inventory>();
            var equipper = hit.GetComponent<SlotEquipper>();
            var seller = hit.GetComponent<Seller>();

            if (seller)
            {
                seller.Sell(equip);
                gameObject.SetActive(false);
                return;
            }

            if (inventory)
            {
                inventory.Strip(equip);
                t.SetParent(inventory.Container, false);
                return;
            }
            
            if (equipper)
            {
                if (equipper.Add(equip))
                {
                    Inventory.Instance.RecalculateStats();
                    Tweener.MoveToBounceOut(t, hit.transform.position, 0.1f);
                    return;   
                }
            }
        }
        
        // Tweener.MoveToBounceOut(t, start, 0.1f);
        t.position = start;
        t.SetParent(parent, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverOut();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MouseDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MouseUp();
    }
}