using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using Sudoku;
using Sudoku.Model;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private NumberPicker numberPicker;
    [SerializeField] private Character player;
    [SerializeField] private Transform enemyPos;
    [SerializeField] private List<Character> enemies;

    private Character enemy;
    private readonly TileGrid<Tile> grid = new(9, 9);

    private SudokuBoard sudoku;
    
    private void Start()
    {
        CreateGrid();
        Generate();

        player.SetHealth(StateManager.Instance.Health);

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        enemy = Instantiate(enemies.Random(), enemyPos);
        enemy.transform.localPosition = Vector3.zero;
        enemy.Board = this;
        enemy.Mirror();
    }

    private void CreateGrid()
    {
        for (var x = 0; x < 9; x++)
        {
            for (var y = 0; y < 9; y++)
            {
                var tile = Instantiate(tilePrefab, transform);
                tile.transform.position = new Vector3(x - 4.1f + GetGap(x), -y + 4.1f - GetGap(y), 0);
                grid.Set(tile, x, y);
            }
        }
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
        if (!enemy.IsAlive()) return;
        
        var value = numberPicker.Number;
        var cell = sudoku.GetCell(index);
        
        if (sudoku.Solver.IsValidValueForTheCell(value, cell))
        {
            tile.Reveal(value);
            Attack(player, enemy, value);
            cell.Value = value;

            Invoke(nameof(CheckWin), 1f);

            if (sudoku.IsBoardFilled())
            {
                Generate();
            }
            
            return;
        }
        
        Attack(enemy, player, value);

        if (!player.IsAlive())
        {
            Invoke(nameof(Lose), 1f);
        }
    }

    private void CheckWin()
    {
        if (!enemy.IsAlive())
        {
            player.WalkTo(7);
            Invoke(nameof(Win), 1f);
        }
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
    }

    public void EnemyAttack(int damage)
    {
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