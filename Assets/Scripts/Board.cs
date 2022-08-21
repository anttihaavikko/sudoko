using System;
using System.Linq;
using AnttiStarterKit.Extensions;
using Sudoku;
using Sudoku.Model;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private NumberPicker numberPicker;

    private readonly TileGrid<Tile> grid = new(9, 9);

    private SudokuBoard sudoku;
    
    private void Start()
    {
        sudoku = new SudokuBoard();
        
        sudoku.Clear();
        sudoku.Solver.SolveThePuzzle();

        var toRemove = 10;
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
                var tile = Instantiate(tilePrefab, transform);
                tile.transform.position = new Vector3(x - 4.1f + GetGap(x), -y + 4.1f - GetGap(y), 0);
                grid.Set(tile, x, y);

                var index = x + y * 9;
                var cell = sudoku.GetCell(index);
                var val = cell.Value;
                tile.Setup(this, index, val, val > 0);
            }
        }
    }

    public void TryFill(Tile tile, int index)
    {
        var value = numberPicker.Number;
        var cell = sudoku.GetCell(index);
        if (sudoku.Solver.IsValidValueForTheCell(value, cell))
        {
            tile.Reveal(value);
        }
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