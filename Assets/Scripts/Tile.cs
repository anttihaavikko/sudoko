using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private int solution;
    private Board board;
    private int index;
    
    public bool IsRevealed { get; private set; }

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
        if (!board || IsRevealed) return;
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
        this.StartCoroutine(() => label.text = "", 0.5f);
    }

    public void Clear()
    {
        label.text = "";
    }
}