using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Shaker shaker;
    [SerializeField] private Appearer disabler;
    [SerializeField] private TMP_Text disableMark;
    [SerializeField] private SpriteRenderer filling;
    [SerializeField] private Color hoverColor;

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
        AudioManager.Instance.PlayEffectFromCollection(9, transform.position, 1.5f);
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
        
        AudioManager.Instance.PlayEffectAt(3, transform.position, 1.25f);
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

    public void HoverIn()
    {
        filling.color = hoverColor;
    }

    public void HoverOut()
    {
        filling.color = Color.white;
    }
}