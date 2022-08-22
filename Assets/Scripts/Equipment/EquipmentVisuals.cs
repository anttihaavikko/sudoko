using UnityEngine;

namespace Equipment
{
    public class EquipmentVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite, trim;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private GameObject hair;

        private bool free = true;

        public EquipmentSlot Slot => slot;
        public bool IsFree => free;

        public void Show(Equip e, bool useGrounded = false)
        {
            sprite.sprite = useGrounded ? e.grounded : e.sprite;
            sprite.color = e.color;
            trim.sprite = e.trim;
            trim.color = e.trimColor;
            free = false;

            if (slot == EquipmentSlot.Hat)
            {
                hair.SetActive(false);
            }
        }

        public void Hide()
        {
            sprite.sprite = null;
            trim.sprite = null;
            free = true;
            
            if (slot == EquipmentSlot.Hat)
            {
                hair.SetActive(true);
            }
        }
    }
}