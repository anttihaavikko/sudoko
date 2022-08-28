using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using Leaderboards;
using Map;
using UnityEngine;

public class Restarter : MonoBehaviour
{
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private ScoreManager scoreManager;
    
    public void Restart()
    {
        Clear();
        SceneChanger.Instance.ChangeScene("Map");
    }

    public void Back()
    {
        Clear();
        SceneChanger.Instance.ChangeScene("Start");
    }

    private static void Clear()
    {
        AudioManager.Instance.TargetPitch = 1f;
        MapState.Instance.Clear();
        StateManager.Instance.Clear();
    }

    public void SubmitAndQuit()
    {
        scoreManager.SubmitScore(PlayerPrefs.GetString("PlayerName"), scoreDisplay.Total, StateManager.Instance.Level, PlayerPrefs.GetString("PlayerId"));
        Back();
    }
}