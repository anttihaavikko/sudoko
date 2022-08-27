using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using Map;
using UnityEngine;

public class Restarter : MonoBehaviour
{
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
}