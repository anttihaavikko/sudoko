using System;
using UnityEngine;

public class Looter : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

    private void Start()
    {
        Invoke(nameof(AfterStart), 0.1f);
    }

    private void AfterStart()
    {
        inventoryPanel.SetActive(true);
    }

    public void Continue()
    {
        StateManager.Instance.NextLevel();
    }
}