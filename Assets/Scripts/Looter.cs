using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using Equipment;
using UnityEngine;
using Random = UnityEngine.Random;

public class Looter : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Character player;
    [SerializeField] private Drop dropPrefab;
    [SerializeField] private SkillSet weaponSkills, armorSkills, soulSkills;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float spawnOffset;

    private Lootable lootSource;
    private readonly List<Drop> dropItems = new();
    private float lastPos;

    private void Start()
    {
        Invoke(nameof(AfterStart), 0.1f);
    }

    public void SetSource(Lootable source)
    {
        lootSource = source;
    }

    private void AfterStart()
    {
        inventoryPanel.SetActive(true);
    }

    public void Continue()
    {
        player.WalkTo(lastPos + 10, false);
        Invoke(nameof(NextLevel), 1.5f);
    }

    private void NextLevel()
    {
        StateManager.Instance.NextLevel();
    }

    public void GenerateDrops()
    {
        var drops = lootSource.GetDrops().OrderBy(_ => Random.value).ToList();
        var p = lootSource.transform.position;

        var offset = 0;
        
        drops.ForEach(d =>
        {
            var speed = Random.Range(0.8f, 1.2f);
            var drop = Instantiate(dropPrefab, lootSource.SpawnPoint, Quaternion.identity);
            drop.GetComponent<Animator>().speed = speed;
            var set = GetSkillSetFor(d.slot);
            d.AddSkill(set.Random());
            drop.Setup(d);
            dropItems.Add(drop);
            var targetPos = p + (spawnOffset + 2f * offset) * Vector3.right;
            Tweener.MoveToQuad(drop.transform, targetPos, 0.5f * speed);
            var rot = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            lastPos = targetPos.x;
            offset++;
        });
    }
    
    private SkillSet GetSkillSetFor(EquipmentSlot slot)
    {
        if (slot == EquipmentSlot.Soul) return soulSkills;
        return slot == EquipmentSlot.Weapon ? weaponSkills : armorSkills;
    }

    public IEnumerator LootDrops()
    {
        player.ReattachHpDisplay();
        var p = lootSource.transform.position;
        var offset = 0;
        var start = p.x;
        foreach (var d in dropItems)
        {
            var duration = player.WalkTo(start + offset * 2f + spawnOffset, true, false);
            offset++;
            yield return new WaitForSeconds(duration);

            d.gameObject.SetActive(false);
            inventory.Add(d.Equipment, true);
            
            player.RecalculateStats();

            yield return new WaitForSeconds(0.6f);
        }
    }
}