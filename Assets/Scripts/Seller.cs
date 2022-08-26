using AnttiStarterKit.Game;
using Equipment;
using UnityEngine;

public class Seller : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private ScoreDisplay goldDisplay;

    public void Sell(Equip e)
    {
        player.Remove(e);
        player.RecalculateStats();
        goldDisplay.Add(e.GetPrice());
        StateManager.Instance.Gold = goldDisplay.Total;
    }
}