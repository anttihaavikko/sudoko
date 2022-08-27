using AnttiStarterKit.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill", order = 0)]
public class Skill : ScriptableObject
{
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private string prefix, suffix;
    [SerializeField] private SkillType type;
    [SerializeField] private Stats stats;
    [SerializeField] private int hp;
    [SerializeField] private bool interrupts;
    [SerializeField] private int price = 50;
    [SerializeField] private Color color = Color.white;

    private int first, second;

    public int Price => price;
    public Color Color => color;

    public void Debug()
    {
        UnityEngine.Debug.Log($"{name} - {first} / {second}");
    }
    
    public Skill Copy()
    {
        var cloned = Instantiate(this);
        cloned.Randomize();
        return cloned;
    }

    private void Randomize()
    {
        first = Random.Range(1, 10);
        second = Random.Range(1, 10);
    }

    public bool Matches(SkillType t, int number)
    {
        return type == t && number == first;
    }
    
    public bool Matches(SkillType t)
    {
        return type == t;
    }
    
    public bool Interrupts(int number)
    {
        return interrupts && number == first;
    }

    public string GetDescription()
    {
        return description
            .Replace("(", "<color=#FB6107>")
            .Replace(")", "</color>")
            .Replace("[1]", TextUtils.TextWith(first.ToString(), new Color(0.9843137f, 0.3803922f, 0.02745098f)))
            .Replace("[2]", TextUtils.TextWith(second.ToString(), new Color(0.9843137f, 0.3803922f, 0.02745098f)));
    }

    public Stats GetStats()
    {
        return stats;
    }

    public int GetHp()
    {
        return hp;
    }

    public string Decorate(string itemName)
    {
        return $"{prefix} {itemName} {suffix}".Trim();
    }

    public bool CanDoubleWith(Skill other)
    {
        return string.IsNullOrEmpty(prefix) != string.IsNullOrEmpty(other.prefix);
    }
}

public enum SkillType
{
    None,
    HealInsteadOnX,
    Thorns,
    IgnoresDefence,
    AttackInsteadOnX,
    ClearColOnAttack,
    ClearRowOnAttack,
    ClearSectionOnAttack,
    ClearCellOnAttack,
    DisableCellOnAttack,
    HideSolvedCellOnAttack,
    NoStagger,
    StaggerOnlyOnX,
    BigDamageOnClear,
    ClearNeighboursOnX,
    ExtraHitOnX,
    FillNeighboursOnX,
    HealAfterCombat,
    HealOnX,
    MoreDamageOnX,
    SlowEnemies,
    StartWithFill,
    MultiGuard,
    BiggerMultiOnLowDef,
    AutoFillOnTimer
}