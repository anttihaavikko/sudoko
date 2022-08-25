using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using Unity.VisualScripting;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Shaker shaker;
    [SerializeField] private Appearer disabler;
    [SerializeField] private TMP_Text disableMark;

    private int solution;
    private Board board;
    private int index;
    private bool isDisabled;
    
    public bool IsRevealed { get; private set; }

    public int Index => index;

    public void Setup(Board b, int i, int number, bool revealed)
    {
        board = b;
        label.text = "";
        solution = number;
        IsRevealed = revealed;
        index = i;
        
        if (revealed)
        {
            Reveal(solution);
        }
    }

    public void Fill()
    {
        if (!board || IsRevealed || isDisabled) return;
        board.TryFill(this, index);
    }

    public void Reveal(int value)
    {
        IsRevealed = true;
        label.color = Color.black;
        label.text = value.ToString();
    }

    public void IndicateWrong(int value)
    {
        label.color = Color.red;
        label.text = value.ToString();
        shaker.Shake();
        this.StartCoroutine(() => label.text = "", 0.5f);
    }

    public void Clear(bool shake = false)
    {
        label.text = "";
        IsRevealed = false;

        if (board)
        {
            board.ClearSolutionCell(index);
        }

        if (shake)
        {
            shaker.Shake();
        }

        isDisabled = false;
        disabler.Hide();
    }

    public void DisableTile(string mark)
    {
        isDisabled = true;
        disabler.Show();
        disableMark.text = mark;
        IsRevealed = true;

        if (board)
        {
            board.FillCell(index);
        }
    }

    public void Shake()
    {
        shaker.Shake();
    }
}