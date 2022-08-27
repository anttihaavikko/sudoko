using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Managers;
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
    [SerializeField] private Appearer continueButton;

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
        continueButton.Hide();
        player.WalkTo(lastPos + 10, 0, false);
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
        
        AudioManager.Instance.PlayEffectFromCollection(9, lootSource.SpawnPoint, 1f);
        
        drops.ForEach(d =>
        {
            var speed = Random.Range(0.8f, 1.2f);
            var drop = Instantiate(dropPrefab, lootSource.SpawnPoint, Quaternion.identity);
            drop.GetComponent<Animator>().speed = speed;
            var set = GetSkillSetFor(d.slot);
            for (var i = 0; i < d.SkillCount; i++)
            {
                d.AddSkill(set.Random());
            }
            drop.Setup(d);
            dropItems.Add(drop);
            var targetPos = p + (spawnOffset + 2f * offset) * Vector3.right + Vector3.up * d.groundOffset;
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
            var pos = player.transform.position;
            var duration = player.WalkTo(start + offset * 2f + spawnOffset, 0, true, false);
            offset++;
            yield return new WaitForSeconds(duration);

            d.gameObject.SetActive(false);

            if (!d.Equipment.IsConsumable())
            {
                inventory.Add(d.Equipment, true);
                AudioManager.Instance.PlayEffectFromCollection(7, pos, 0.4f);
                AudioManager.Instance.PlayEffectFromCollection(4, pos, 0.3f);
                AudioManager.Instance.PlayEffectFromCollection(6, pos, 1.5f);
            }

            if (d.Equipment.slot == EquipmentSlot.Potion)
            {
                player.Shout("POTION");
                AudioManager.Instance.PlayEffectFromCollection(8, pos, 1.5f);
                AudioManager.Instance.PlayEffectFromCollection(6, pos, 1.5f);
                yield return new WaitForSeconds(0.2f);
                player.Heal(20);
                yield return new WaitForSeconds(0.5f);
            }

            if (d.Equipment.slot == EquipmentSlot.Gold)
            {
                AudioManager.Instance.PlayEffectFromCollection(1, pos);
                AudioManager.Instance.PlayEffectFromCollection(6, pos, 1.5f);
                player.Shout($"+{d.Equipment.Gold} GOLD");
                inventory.AddGold(d.Equipment.Gold);
            }
            
            player.RecalculateStats();

            yield return new WaitForSeconds(0.6f);
        }
    }
}