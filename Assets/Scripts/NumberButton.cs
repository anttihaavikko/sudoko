using AnttiStarterKit.Animations;
using UnityEngine;
using UnityEngine.UI;

public class NumberButton : MonoBehaviour
{
    [SerializeField] private Image filling;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private ButtonStyle buttonStyle;

    public void Select()
    {
        buttonStyle.enabled = false;
        filling.color = activeColor;
        Tweener.ScaleToBounceOut(transform, Vector3.one * 1.2f, 0.2f);
    }

    public void DeSelect()
    {
        filling.color = Color.white;
        Tweener.ScaleToQuad(transform, Vector3.one, 0.2f);
        buttonStyle.enabled = true;
    }
}