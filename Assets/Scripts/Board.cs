using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using Equipment;
using Leaderboards;
using Map;
using Sudoku;
using Sudoku.Model;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private NumberPicker numberPicker;
    [SerializeField] private Character player;
    [SerializeField] private Transform enemyPos;
    [SerializeField] private List<EnemyList> enemies;
    [SerializeField] private Drop dropPrefab;
    [SerializeField] private TMP_Text enemyDescription;
    [SerializeField] private GameObject enemyTooltip;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Appearer continueButton, startButton, uiHider;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private Looter looter;
    [SerializeField] private GameObject gameOverStuff;
    [SerializeField] private GameObject bossCam;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject numberPanel;
    [SerializeField] private List<Character> enemyList;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Appearer tutorial;

    private Character enemy;
    private readonly TileGrid<Tile> grid = new(9, 9);

    private SudokuBoard sudoku;
    private bool ending;
    private bool fightStarted;
    private bool tutorialVisible;

    private Camera cam;

    public bool ShowingTutorial => tutorialVisible;

    private void Start()
    {
        CreateGrid();
        SpawnEnemy();

        player.Board = this;
        
        Invoke(nameof(ShowTutorial), 3f);
    }

    private void ShowTutorial()
    {
        if (!StateManager.Instance.TutorialShown)
        {
            tutorialVisible = true;
            tutorial.Show();
            StateManager.Instance.TutorialShown = true;
        }
    }
    
    public void StartFight()
    {
        if (fightStarted) return;

        ShowTutorial();
        
        startButton.Hide();
        fightStarted = true;
        Generate();
        enemy.StartTimer();
        
        Invoke(nameof(StartFills), 0.5f);
        Invoke(nameof(FillOnTimer), 30f);
    }

    private void StartFills()
    {
        RandomFills(player.GetSkillCount(SkillType.StartWithFill));
    }

    private void RandomFills(int fills)
    {
        var cells = grid.All().Where(c => !c.IsRevealed).OrderBy(_ => Random.value).Take(fills);
        StartCoroutine(FillCells(cells, Vector3.zero));
    }

    private void FillOnTimer()
    {
        var fills = player.GetSkillCount(SkillType.AutoFillOnTimer);
        if (fills <= 0) return;
        RandomFills(fills);
        Invoke(nameof(FillOnTimer), 30f);
    }

    private void Update()
    {
        if (DevKey.Down(KeyCode.Space))
        {
            if(!fightStarted) StartFight();
            Attack(player, enemy, 10);
        }

        if (DevKey.Down(KeyCode.N))
        {
            Win();
        }

        if (DevKey.Down(KeyCode.B))
        {
            MapState.Instance.x = 6;
            SceneChanger.Instance.ChangeScene("Main");
        }
    }

    private void SpawnEnemy()
    {
        var enemyIndex = StateManager.Instance.ExtraBoss ? 6 : MapState.Instance.x;
        StateManager.Instance.ExtraBoss = false;
        var prefab = enemies[enemyIndex].Random();
        enemy = Instantiate(prefab, enemyPos);
        enemy.Index = enemyList.IndexOf(prefab);
        enemy.transform.localPosition = Vector3.zero;
        enemy.Board = this;
        enemy.Mirror();
        enemy.showDescription = ShowDescription;
        
        looter.SetSource(enemy);

        if (enemy.IsBoss)
        {
            Invoke(nameof(ActivateBossCam), 0.1f);
        }
    }

    private void ActivateBossCam()
    {
        bossCam.SetActive(true);
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
                var tile = Instantiate(tilePrefab, gridContainer);
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

    public void TryFill(Tile tile, int index, int val = -1)
    {
        if (!fightStarted || !enemy.IsAlive() || !player.IsAlive()) return;

        var value = val > -1 ? val : numberPicker.Number;
        var cell = sudoku.GetCell(index);
        
        var x = cell.Position.Column - 1;
        var y = cell.Position.Row - 1;
        
        if (sudoku.Solver.IsValidValueForTheCell(value, cell))
        {
            if (tutorialVisible)
            {
                tutorialVisible = false;
                tutorial.HideWithDelay(0.5f);
            }
            
            tile.Reveal(value);

            scoreDisplay.Add(value * GetDefScoreMulti());

            cell.Value = value;

            var heals = player.GetSkillCount(SkillType.HealOnX, value);
            if (heals > 0)
            {
                this.StartCoroutine(() => player.Heal(value), 0.5f);
            }

            if (player.HasSkill(SkillType.ClearNeighboursOnX, value))
            {
                var neighbours = grid.GetNeighbours(x, y);
                StartCoroutine(ClearCells(neighbours, Vector3.zero));
            }

            CheckForFullBoard();
            
            if (enemy.Interrupts(value, player))
            {
                return;
            }

            var boost = player.GetSkillCount(SkillType.MoreDamageOnX, value);
            var mod = Mathf.Pow(1.5f, boost);
            var hits = player.GetSkillCount(SkillType.ExtraHitOnX, value) + 1;

            for (var i = 0; i < hits; i++)
            {
                var rounded = Mathf.RoundToInt(value * mod);
                this.StartCoroutine(() => Attack(player, enemy, rounded), i * 0.2f);
            }

            if (player.HasSkill(SkillType.FillNeighboursOnX, value))
            {
                var neighbours = grid.GetNeighbours(x, y).Where(c => !c.IsRevealed);
                StartCoroutine(FillCells(neighbours, Vector3.zero));
            }

            return;
        }

        tile.IndicateWrong(value);
        
        Attack(enemy, player, value);
    }

    private void CheckForFullBoard()
    {
        if (sudoku.IsBoardFilled())
        {
            var bigDamage = player.GetSkillCount(SkillType.BigDamageOnClear);
            if (bigDamage > 0)
            {
                var attack = player.GetAttack() + 1;
                var amount = bigDamage * 20 * attack;
                this.StartCoroutine(() => Attack(player, enemy, amount, false), 0.8f);
            }

            Invoke(nameof(Clear), 0.75f);
            Invoke(nameof(Generate), 1.5f);
        }
    }

    private int GetDefScoreMulti()
    {
        var skills = player.GetSkillCount(SkillType.BiggerMultiOnLowDef);
        var diff = 5 - player.GetDefense();
        return Mathf.Max(1, 1 + diff * skills);
    }

    public float GetSlowMod()
    {
        return Mathf.Pow(1.25f, player.GetSkillCount(SkillType.SlowEnemies));
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
            Lose();
        }
    }

    private IEnumerator EndWalk()
    {
        // AudioManager.Instance.Highpass();
        
        scoreDisplay.Add(enemy.Score * (StateManager.Instance.Level + 1));

        looter.GenerateDrops();
        
        yield return new WaitForSeconds(0.5f);
        
        scoreDisplay.AddMulti();

        // uiHider.Show();
        
        yield return new WaitForSeconds(0.5f);
        
        if (!player.IsAlive()) yield break;

        StateManager.Instance.Health = player.CurrentHealth;
        
        if (enemy.IsBoss)
        {
            bossCam.SetActive(false);
            yield return new WaitForSeconds(4f);
        }
        
        if (player.HasSkill(SkillType.HealAfterCombat))
        {
            player.ShowHp();
            yield return new WaitForSeconds(0.2f);
            player.Heal(9999);
            yield return new WaitForSeconds(0.7f);
        }
        
        numberPanel.SetActive(false);
        gridContainer.gameObject.SetActive(false);
        inventoryPanel.SetActive(true);
        // uiHider.HideWithDelay();

        yield return looter.LootDrops();
        
        continueButton.Show();
        
        ScoreLoader.Save(scoreDisplay);
    }

    public void Continue()
    {
        continueButton.Hide();
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        player.WalkTo(20, 0, false);
        // AudioManager.Instance.Highpass(false);
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
        this.StartCoroutine(() => AudioManager.Instance.PlayEffectAt(10, Vector3.zero), 0.2f);
        AudioManager.Instance.TargetPitch = 0f;
        scoreManager.SubmitScore(PlayerPrefs.GetString("PlayerName"), scoreDisplay.Total, StateManager.Instance.Level, PlayerPrefs.GetString("PlayerId"));
        gameOverStuff.SetActive(true);
    }

    private void Attack(Character attacker, Character target, int damage, bool boosted = true)
    {
        attacker.Attack(target, damage, boosted);
        Invoke(nameof(CheckEnd), 0.5f);
    }

    public void EnemyAttack(int damage)
    {
        if (!player.IsAlive()) return;
        enemy.Attack(player, damage);
        Invoke(nameof(CheckEnd), 0.8f);
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

    public void ClearSolutionCell(int index)
    {
        sudoku.GetCell(index).Value = -1;
    }

    public void ClearRandomCell()
    {
        grid.All().Where(c => c.IsRevealed).ToList().Random().Clear(true);
    }

    public void ClearRandomRow()
    {
        var row = Random.Range(0, 9);
        var cells = Enumerable.Range(0, 9).Select(i => grid.Get(i, row));
        StartCoroutine(ClearCells(cells, enemy.transform.position));
    }
    
    public void ClearRandomColumn()
    {
        var column = Random.Range(0, 9);
        var cells = Enumerable.Range(0, 9).Select(i => grid.Get(column, i));
        StartCoroutine(ClearCells(cells, enemy.transform.position));
    }

    public void ClearRandomSection()
    {
        var cells = new List<Tile>();
        
        var x = Random.Range(0, 3) * 3;
        var y = Random.Range(0, 3) * 3;

        for (var ix = 0; ix < 3; ix++)
        {
            for (var iy = 0; iy < 3; iy++)
            {
                cells.Add(grid.Get(x + ix, y + iy));
            }   
        }
        
        StartCoroutine(ClearCells(cells, enemy.transform.position));
    }

    private IEnumerator ClearCells(IEnumerable<Tile> cells, Vector3 origin)
    {
        var ordered = cells.OrderBy(c => Vector3.Distance(c.transform.position, origin)).ToList();
        foreach (var cell in ordered)
        {
            cell.Clear(true);
            yield return new WaitForSeconds(0.03f);
        }
    }
    
    private IEnumerator FillCells(IEnumerable<Tile> cells, Vector3 origin)
    {
        var ordered = cells.OrderBy(c => Vector3.Distance(c.transform.position, origin)).ToList();
        foreach (var tile in ordered)
        {
            var cell = sudoku.GetCell(tile.Index);
            var val = Enumerable.Range(1, 9).FirstOrDefault(i => sudoku.Solver.IsValidValueForTheCell(i, cell));
            if (val > 0)
            {
                TryFill(tile, tile.Index, val);
                tile.Shake();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void FillCell(int index)
    {
        var cell = sudoku.GetCell(index);
        var val = Enumerable.Range(1, 9).FirstOrDefault(i => sudoku.Solver.IsValidValueForTheCell(i, cell));
        if (val > 0)
        {
            cell.Value = val;
            CheckForFullBoard();
        }
    }

    public void DisableRandomCell()
    {
        grid.All().Where(c => !c.IsRevealed).ToList().Random().DisableTile("X");
        CheckForFullBoard();
    }

    public void HideSolvedCell()
    {
        grid.All().Where(c => c.IsRevealed).ToList().Random().DisableTile("?");
    }

    public void DecreaseMulti()
    {
        scoreDisplay.DecreaseMulti();
    }
}