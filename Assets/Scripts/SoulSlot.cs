using UnityEngine;
using UnityEngine.UI;

public class SoulSlot : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Fill()
    {
        image.color = Color.white;
    }
}