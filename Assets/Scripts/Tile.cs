using TMPro;
using UnityEngine;

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
        if (IsRevealed) return;
        board.TryFill(this, index);
    }

    public void Reveal(int value)
    {
        IsRevealed = true;
        label.text = value.ToString();
    }
}