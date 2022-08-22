using UnityEngine;

namespace Equipment
{
    public class EquipmentVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite, trim;
        [SerializeField] private EquipmentSlot slot;

        public EquipmentSlot Slot => slot;

        public void Show(Equipment e)
        {
            sprite.sprite = e.sprite;
            sprite.color = e.color;
            trim.sprite = e.trim;
            trim.color = e.trimColor;
        }

        public void Hide()
        {
            sprite.sprite = null;
            trim.sprite = null;
        }
    }
}