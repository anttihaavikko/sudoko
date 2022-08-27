using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
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

        var p = player.transform.position;
        AudioManager.Instance.PlayEffectFromCollection(9, p, 1f);
        AudioManager.Instance.PlayEffectFromCollection(10, p, 1f);
        AudioManager.Instance.PlayEffectFromCollection(1, p, 0.5f);
    }
}