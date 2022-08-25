using UnityEngine;

namespace Equipment
{
    public class EquipmentVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite, trim;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private GameObject hair;
        [SerializeField] private float chance = 0.5f;
        [SerializeField] private bool canMask;

        private bool free = true;
        private Equip equip;

        public EquipmentSlot Slot => slot;
        public bool IsFree => free;

        public bool Spawns => Random.value < chance;

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

            if (hair && slot == EquipmentSlot.Hat)
            {
                hair.SetActive(false);
            }

            if (canMask)
            {
                sprite.maskInteraction = e.masked ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None;
                trim.maskInteraction = e.masked ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None;   
            }
        }

        public void Hide()
        {
            equip = null;
            sprite.sprite = null;
            trim.sprite = null;
            free = true;
            
            if (hair && slot == EquipmentSlot.Hat)
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