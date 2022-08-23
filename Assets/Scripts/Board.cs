using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using Equipment;
using Sudoku;
using Sudoku.Model;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private NumberPicker numberPicker;
    [SerializeField] private Character player;
    [SerializeField] private Transform enemyPos;
    [SerializeField] private List<Character> enemies;
    [SerializeField] private Drop dropPrefab;
    [SerializeField] private TMP_Text enemyDescription;
    [SerializeField] private GameObject enemyTooltip;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Appearer continueButton, startButton;
    [SerializeField] private SkillSet weaponSkills, armorSkills;

    private Character enemy;
    private readonly TileGrid<Tile> grid = new(9, 9);

    private SudokuBoard sudoku;
    private bool ending;
    private bool fightStarted;

    private void Start()
    {
        CreateGrid();
        SpawnEnemy();
    }

    public void StartFight()
    {
        if (fightStarted) return;
        
        startButton.Hide();
        fightStarted = true;
        Generate();
        enemy.StartTimer();
    }

    private void Update()
    {
        if (DevKey.Down(KeyCode.Space))
        {
            Attack(player, enemy, 5);
        }
    }

    private void SpawnEnemy()
    {
        enemy = Instantiate(enemies.Random(), enemyPos);
        enemy.transform.localPosition = Vector3.zero;
        enemy.Board = this;
        enemy.Mirror();
        enemy.showDescription = ShowDescription;
    }

    private void ShowDescription(string desc)
    {
        enemyDescription.text = desc;
    }

    private void CreateGrid()
    {
        for (var x = 0; x < 9; x++)
        {
            for (var y = 0; y < 9; y++)
            {
                var tile = Instantiate(tilePrefab, transform);
                tile.Clear();
                tile.transform.position = new Vector3(x - 4.1f + GetGap(x), -y + 4.1f - GetGap(y), 0);
                grid.Set(tile, x, y);
            }
        }
    }

    private void Clear()
    {
        grid.All().ToList().ForEach(tile => tile.Clear());
    }

    private void Generate()
    {
        sudoku = new SudokuBoard();

        sudoku.Clear();
        sudoku.Solver.SolveThePuzzle();

        var toRemove = Mathf.Min(10 + 3 * StateManager.Instance.Level, 50);
        var tries = 0;

        while (toRemove > 0 && tries < 100)
        {
            var cell = sudoku.Cells.Where(c => c.Value > 0).ToList().Random();
            var value = cell.Value;
            cell.Value = -1;

            if (sudoku.Solver.CheckTableStateIsValid(true))
            {
                toRemove--;
                continue;
            }

            Debug.Log("Not valid");
            cell.Value = value;
            tries++;
        }

        for (var x = 0; x < 9; x++)
        {
            for (var y = 0; y < 9; y++)
            {
                var index = x + y * 9;
                var tile = grid.Get(x, y);
                var cell = sudoku.GetCell(index);
                var val = cell.Value;
                tile.Setup(this, index, val, val > 0);
            }
        }
    }

    public void TryFill(Tile tile, int index)
    {
        if (!fightStarted || !enemy.IsAlive() || !player.IsAlive()) return;

        var value = numberPicker.Number;
        var cell = sudoku.GetCell(index);
        
        if (sudoku.Solver.IsValidValueForTheCell(value, cell))
        {
            tile.Reveal(value);

            cell.Value = value;

            if (sudoku.IsBoardFilled())
            {
                Invoke(nameof(Clear), 0.75f);
                Invoke(nameof(Generate), 1.5f);
            }
            
            if (enemy.Interrupts(value))
            {
                return;
            }
            
            Attack(player, enemy, value);
            
            return;
        }

        tile.IndicateWrong(value);
        
        Attack(enemy, player, value);
    }

    private void CheckEnd()
    {
        if (ending) return;
        
        if (!enemy.IsAlive())
        {
            ending = true;
            enemyTooltip.SetActive(false);
            StartCoroutine(EndWalk());
        }

        if (!player.IsAlive())
        {
            ending = true;
            CancelInvoke(nameof(Win));
            Invoke(nameof(Lose), 1f);
        }
    }

    private IEnumerator EndWalk()
    {
        var drops = enemy.GetDrops();
        var p = enemy.transform.position;
        var dropItems = new List<Drop>();
        
        var offset = 0;
        
        drops.ForEach(d =>
        {
            var drop = Instantiate(dropPrefab, p, Quaternion.identity);
            var set = d.slot == EquipmentSlot.Weapon ? weaponSkills : armorSkills;
            d.AddSkill(set.Random());
            drop.Setup(d);
            dropItems.Add(drop);
            Tweener.MoveToQuad(drop.transform, p + 2f * offset * Vector3.right, 0.2f);
            offset++;
        });
        
        yield return new WaitForSeconds(1f);
        
        if (!player.IsAlive()) yield break;
        
        inventoryPanel.SetActive(true);
        
        offset = 0;
        var start = p.x;
        foreach (var d in dropItems)
        {
            var duration = player.WalkTo(start + offset * 2f, false);
            offset++;
            yield return new WaitForSeconds(duration);
            
            d.gameObject.SetActive(false);
            inventory.Add(d.Equipment, true);
            
            yield return new WaitForSeconds(0.6f);
        }
        
        continueButton.Show();
    }

    public void Continue()
    {
        continueButton.Hide();
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        player.WalkTo(20, false);
        yield return new WaitForSeconds(1.5f);
        Win();
    }

    private void Win()
    {
        StateManager.Instance.Health = player.CurrentHealth;
        StateManager.Instance.NextLevel();
    }

    private void Lose()
    {
    }

    private void Attack(Character attacker, Character target, int damage)
    {
        attacker.Attack(target, damage);
        Invoke(nameof(CheckEnd), 0.2f);
    }

    public void EnemyAttack(int damage)
    {
        if (!player.IsAlive()) return;
        enemy.Attack(player, damage);
    }

    private static float GetGap(int pos)
    {
        return pos switch
        {
            > 5 => 0.2f,
            > 2 => 0.1f,
            _ => 0f
        };
    }

    public int GetCurrentNumber()
    {
        return numberPicker.Number;
    }
}