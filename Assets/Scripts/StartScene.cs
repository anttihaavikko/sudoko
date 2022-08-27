using System;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartScene : MonoBehaviour
{
    [SerializeField] private Character demo;

    private float direction = 1;
    private Transform t;
    private float scale;

    private void Start()
    {
        t = demo.transform;
        scale = t.localScale.x;
        t.position = t.position.WhereX(-20f);
        Invoke(nameof(WalkToRandomPos), 0.25f);
    }

    private void WalkToRandomPos()
    {
        var delay = demo.WalkTo(Random.Range(-5f, 5f), 0, false);
        Invoke(nameof(WalkOff), delay + 1f);
    }

    private void WalkOff()
    {
        var delay = demo.WalkTo(20 * direction, 0, false);
        Invoke(nameof(Flip), delay + 1f);
        Invoke(nameof(WalkToRandomPos), delay + 1f);
    }

    private void Flip()
    {
        demo.GearEnemy();
        direction = -direction;
        t.localScale = t.localScale.WhereX(direction * scale);
    }

    public void NameOrMap()
    {
        var scene = PlayerPrefs.HasKey("PlayerName") ? "Map" : "Name";
        SceneChanger.Instance.ChangeScene(scene);
    }
}