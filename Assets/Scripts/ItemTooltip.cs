using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;

public class ItemTooltip : Manager<ItemTooltip>
{
    [SerializeField] private TMP_Text content;
    [SerializeField] private GameObject node;

    public void Show(Vector3 pos, string text)
    {
        transform.position = pos;
        transform.SetAsLastSibling();
        content.text = text;
        node.SetActive(true);
    }

    public void Hide()
    {
        node.SetActive(false);
    }
}