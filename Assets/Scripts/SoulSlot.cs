using UnityEngine;
using UnityEngine.UI;

public class SoulSlot : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Fill(Color color)
    {
        image.color = color;
    }
}