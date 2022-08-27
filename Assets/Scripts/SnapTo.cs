using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class SnapTo : MonoBehaviour
{
    [SerializeField] private Camera cam;
    
    public Transform target;

    private Transform t;

    private void Start()
    {
        t = transform;
        Snap();
    }

    private void Update()
    {
        Snap();
    }

    void Snap()
    {
        t.position = cam.ScreenToWorldPoint(target.position).WhereZ(0);
    }
}
