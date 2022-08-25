using AnttiStarterKit.Game;
using Map;
using UnityEngine;

public class Restarter : MonoBehaviour
{
    public void Restart()
    {
        MapState.Instance.Clear();
        StateManager.Instance.Clear();
        SceneChanger.Instance.ChangeScene("Map");
    }
}