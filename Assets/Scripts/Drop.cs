using Equipment;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] private EquipmentVisuals visuals;

    private Equip equip;

    public Equip Equipment => equip;

    public void Setup(Equip e)
    {
        equip = e;
        visuals.Show(e, true);
        visuals.transform.Rotate(new Vector3(0, 0, e.groundAngle));
    }
}