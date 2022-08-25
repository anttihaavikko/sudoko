using System;
using System.Collections.Generic;
using Equipment;
using UnityEngine;

public abstract class Lootable : MonoBehaviour
{
    [SerializeField] protected Transform spawnPos;
    
    protected readonly List<Equip> drops = new();

    public Vector3 SpawnPoint => spawnPos.position;

    private void Awake()
    {
        if (!spawnPos)
        {
            spawnPos = transform;
        }
    }

    public List<Equip> GetDrops()
    {
        return drops;
    }
}