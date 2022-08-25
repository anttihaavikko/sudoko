using System;
using AnttiStarterKit.Game;
using UnityEngine;

public class ScoreLoader : MonoBehaviour
{
    private void Start()
    {
        var sm = StateManager.Instance;
        GetComponent<ScoreDisplay>().Set(sm.Score, sm.Multiplier);
    }

    public static void Save(ScoreDisplay display)
    {
        var sm = StateManager.Instance;
        sm.Score = display.Total;
        sm.Multiplier = display.Multi;
    }
}