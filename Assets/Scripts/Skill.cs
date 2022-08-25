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

    private int first, second;

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
            .Replace("(", "<color=red>")
            .Replace(")", "</color>")
            .Replace("[1]", TextUtils.TextWith(first.ToString(), Color.red))
            .Replace("[2]", TextUtils.TextWith(second.ToString(), Color.red));
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