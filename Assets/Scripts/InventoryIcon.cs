using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using Equipment;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField] private LayerMask dropMask;
    [SerializeField] private Image image, trim;
    [SerializeField] private GameObject newIndicator;

    private bool dragging;
    private Transform parent;
    private Vector3 start;
    private Transform rootElement;
    private Equip equip;

    public void Setup(Transform root, Equip e, bool isNew = false)
    {
        rootElement = root;
        
        image.sprite = e.grounded;
        image.color = e.color;
        trim.sprite = e.trim;
        trim.color = e.trim ? e.trimColor : Color.clear;

        equip = e;
        
        newIndicator.SetActive(isNew);
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
        newIndicator.SetActive(false);
        ItemTooltip.Instance.Show(transform.position, equip.GetDescription());
    }

    public void HoverOut()
    {
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
                    Tweener.MoveToBounceOut(t, hit.transform.position, 0.1f);
                    return;   
                }
            }
        }
        
        // Tweener.MoveToBounceOut(t, start, 0.1f);
        t.position = start;
        t.SetParent(parent, true);
    }
}