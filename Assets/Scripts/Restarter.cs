using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using Map;
using UnityEngine;

public class Restarter : MonoBehaviour
{
    public void Restart()
    {
        AudioManager.Instance.TargetPitch = 1f;
        MapState.Instance.Clear();
        StateManager.Instance.Clear();
        SceneChanger.Instance.ChangeScene("Map");
    }
}