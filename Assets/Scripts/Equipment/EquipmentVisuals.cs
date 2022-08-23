using UnityEngine;

namespace Equipment
{
    public class EquipmentVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite, trim;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private GameObject hair;

        private bool free = true;
        private Equip equip;

        public EquipmentSlot Slot => slot;
        public bool IsFree => free;

        public void Show(Equip e, bool useGrounded = false)
        {
            equip = e;
            sprite.sprite = useGrounded ? e.grounded : e.sprite;
            sprite.color = e.color;
            sprite.flipX = e.flipped;
            trim.sprite = e.trim;
            trim.color = e.trimColor;
            trim.flipX = e.flipped;
            free = false;

            if (slot == EquipmentSlot.Hat)
            {
                hair.SetActive(false);
            }
        }

        public void Hide()
        {
            equip = null;
            sprite.sprite = null;
            trim.sprite = null;
            free = true;
            
            if (slot == EquipmentSlot.Hat)
            {
                hair.SetActive(true);
            }
        }

        public Equip GetEquip()
        {
            return equip;
        }

        public bool Has(Equip e)
        {
            return equip == e;
        }
    }
}