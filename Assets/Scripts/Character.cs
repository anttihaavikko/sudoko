using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
using Equipment;
using Map;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;

public class Character : Lootable
{
    [SerializeField] private string title;
    [TextArea][SerializeField] private string description;
    [SerializeField] private bool isBoss;
    [SerializeField] private int score = 100;
    [SerializeField] private Stats stats;
    [SerializeField] private int skillPicks;
    [SerializeField] private SkillSet startsWith;

    [SerializeField] private SoundCollection hurtSounds;

    [SerializeField] private Flasher flasher;
    [SerializeField] private StatsDisplay statsDisplay;
    [SerializeField] private Health health;
    [SerializeField] private List<Transform> mirrorables;
    [SerializeField] private Transform healthDisplay, moveDisplay;
    [SerializeField] private GameObject root, shadow;
    [SerializeField] private bool isPlayer;
    [SerializeField] private Transform moveBar;
    [SerializeField] private float scale = 1f;
    [SerializeField] private List<EquipmentVisuals> equipmentVisuals;
    [SerializeField] private EquipmentList equipmentList;
    [SerializeField] private Transform hitPos, center;
    [SerializeField] private Transform groundMask;
    
    [SerializeField] private Color healColor;
    [SerializeField] private MobList mobList;
    [SerializeField] private GameObject timeDisplay;

    [SerializeField] private Material ghostMaterial;
    [SerializeField] private List<LineRenderer> limbs;
    [SerializeField] private GameObject ghostParticles;
    [SerializeField] private SpriteRenderer ghostFlattener;

    [SerializeField] private List<SpriteRenderer> transparentOnGhost;
    [SerializeField] private List<SpriteRenderer> keepVisibleOnGhost;
    [SerializeField] private Material defaultSpriteMaterial;

    private readonly List<Character> ghosts = new();

    private Animator anim;

    private Vector3 origin;
    private EffectCamera cam;
    private readonly List<Skill> skills = new();

    private float moveTimer;
    private bool fightStarted;
    private int guards;
    private float ghostDistance = 0.9f;
    private float ghostGap = 0.9f;
    private bool isGhost;

    public int CurrentHealth => health.Current;
    public int Score => score;
    public bool IsBoss => isBoss;

    public Board Board { get; set; }
    public int Index { get; set; }

    public Action<string> showDescription;
    
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int AttackAnim = Animator.StringToHash("attack");
    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int SkillTrigger = Animator.StringToHash("skill");
    private static readonly int BossAttack = Animator.StringToHash("bossAttack");
    
    private readonly List<Equip> inventory = new();

    private Stats baseStats;
    private int baseHealth;

    private void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main.GetComponent<EffectCamera>();
        origin = transform.position;
        
        health.Init();

        healthDisplay.SetParent(null, true);
        moveDisplay.SetParent(null, true);

        Stagger();
        Scale(scale);
        Gear();

        if (startsWith)
        {
            startsWith.Random(skillPicks + MapState.Instance.World).ToList().ForEach(s => skills.Add(s.Copy()));   
        }

        if (isPlayer)
        {
            RedoSkills();
        }

        if (!isPlayer)
        {
            stats.Add(new List<Stats>{ stats }, MapState.Instance.World + 1);
        }

        baseStats = stats.Copy();
        baseHealth = health.Max;

        UpdateStats();
        
        showDescription?.Invoke(GetDescription());

        guards = GetSkillCount(SkillType.MultiGuard);
        
        if (isPlayer)
        {
            // flasher.AllSprites.ForEach(s =>
            // {
            //     s.gameObject.layer = 9;
            //     if (s.GetComponent<SpriteMask>()) return;
            //     var mask = s.AddComponent<SpriteMask>();
            //     mask.sprite = s.sprite;
            // });
            //
            // ghostFlattener.material = defaultSpriteMaterial;
            // var go = ghostFlattener.gameObject;
            // go.SetActive(true);
            // go.layer = 8;
            
            transform.position += Vector3.left * 7;
            SpawnGhosts();
            Invoke(nameof(StartWalk), 0.1f);
        }
    }

    private void StartWalk()
    {
        WalkTo(origin.x, 0, true);
    }

    public void RecalculateStats()
    {
        RedoSkills();
        UpdateStats();
    }

    public void RedoSkills()
    {
        var eq = GetEquips();
        SetHealth(StateManager.Instance.Health);
        skills.Clear();
        eq.ForEach(e => skills.AddRange(e.GetSkills()));
    }

    public void UpdateStats()
    {
        stats = baseStats.Copy();
        
        var adds = skills.Select(s => s.GetStats()).ToList();
        stats.Add(adds);

        var balanceAdds = GetSkillCount(SkillType.BalancedStats);
        if (balanceAdds > 0)
        {
            stats.Add(new List<Stats>{ new() { attack = balanceAdds * 5, defence = balanceAdds * 5 }});
        }

        health.SetMax(baseHealth);
        var hpAddition = skills.Sum(s => s.GetHp());
        health.AddMax(hpAddition, !isPlayer);
        health.Cap();

        stats.Cap();
        
        statsDisplay.Set(stats);
    }

    private void Scale(float mod)
    {
        if (!isPlayer)
        {
            groundMask.SetParent(null, true);
        }

        transform.localScale *= mod;
        
        if (!isPlayer)
        {
            groundMask.SetParent(transform, true);
        }
    }

    private string GetDescription()
    {
        var sb = new StringBuilder();
        sb.Append(TextUtils.TextWith(title, 30));
        sb.Append(TextUtils.TextWith("\n", 13));
        
        sb.Append(TextUtils.TextWith(description, Color.gray, 15));
        
        // sb.Append($"Attack: {stats.attack}\n");
        // sb.Append($"Speed: 1 / {stats.speed} s\n");
        // if (stats.defence > 0)
        // {
        //     sb.Append($"Defense: {stats.defence}\n");
        // }
        skills.ForEach(s => sb.Append($"<size=10>\n\n</size>{s.GetDescription()}"));
        return sb.ToString();
    }

    public void SkillEffect()
    {
        anim.SetTrigger(SkillTrigger);
    }

    private void Gear()
    {
        if (isGhost) return;
        
        var gear = StateManager.Instance.Gear;
        
        if (isPlayer)
        {
            inventory.Clear();
            inventory.AddRange(StateManager.Instance.Inventory);
            
            equipmentVisuals.ForEach(v =>
            {
                v.Hide();
                var match = gear.FirstOrDefault(g => g != null && g.slot == v.Slot);
                if (match == default) return;
                gear.Remove(match);
                v.Show(match);
            });
            
            return;
        }

        GearEnemy();
    }

    public void GearEnemy()
    {
        var indexList = new List<int>();
        
        equipmentVisuals.ForEach(v =>
        {
            v.Hide();
            if (!v.Spawns) return;
            var index = equipmentList.RandomIndex(v.Slot);
            indexList.Add(index);
            var e = equipmentList.Get(index);
            AddExtraSkills(e);
            v.Show(e);
            drops.Add(e);
        });

        var soul = equipmentList.Random(EquipmentSlot.Soul);
        AddExtraSkills(soul);
        soul.SetGhost(Index, drops);
        drops.Add(soul);

        if (drops.Count < 5)
        {
            drops.Add(equipmentList.Random(EquipmentSlot.Gold));
        }
        
        if (drops.Count < 5 && Random.value < GetPotionChance())
        {
            drops.Add(equipmentList.Random(EquipmentSlot.Potion));
        }
    }

    private void AddExtraSkills(Equip e)
    {
        if (!isBoss) return;
        
        for (var i = 0; i < MapState.Instance.World + 1; i++)
        {
            e.AddExtraSkill();
        }
    }

    public void Mirror()
    {
        transform.Mirror();
        mirrorables.ForEach(m => m.Mirror());
    }

    private void Update()
    {
        if (!fightStarted || isPlayer || !IsAlive()) return;

        var mod = Board ? Board.GetSlowMod() : 1f;
        moveTimer -= Time.deltaTime / mod;

        var amount = 1f - moveTimer / stats.speed;
        moveBar.localScale = moveBar.localScale.WhereY(amount);

        if (moveTimer <= 0)
        {
            moveTimer = stats.speed;
            Board.EnemyAttack(0);
        }
    }

    private void Stagger()
    {
        moveTimer = stats.speed;
        moveBar.localScale = moveBar.localScale.WhereY(0);
    }

    public void Remove(Equip e)
    {
        var equippedSlot = equipmentVisuals.FirstOrDefault(v => v.Has(e));
        if (equippedSlot)
        {
            Remove(equippedSlot);
        }
        
        inventory.Remove(e);
    }

    public Equip Remove(int slotIndex)
    {
        if (equipmentVisuals[slotIndex].IsFree) return null;
        var e = equipmentVisuals[slotIndex].GetEquip();
        Remove(equipmentVisuals[slotIndex]);
        return e;
    }

    private void Remove(EquipmentVisuals slot)
    {
        inventory.Add(slot.GetEquip());
        slot.Hide();
        UpdateState();
    }

    public void Shout(string message)
    {
        EffectManager.AddTextPopup(message, transform.position + Vector3.up * 3);
    } 

    public int Add(Equip e, int inSlot = -1)
    {
        inventory.Remove(e);
        
        if (inSlot < 0)
        {
            var equippedSlot = equipmentVisuals.FirstOrDefault(v => v.Has(e));
            if (equippedSlot)
            {
                return equipmentVisuals.IndexOf(equippedSlot);
            }   
        }

        EffectManager.AddTextPopup(e.GetName().ToUpper(), transform.position + Vector3.up * 3);
        
        var slot = inSlot >= 0 ? 
            equipmentVisuals[inSlot] : 
            equipmentVisuals.FirstOrDefault(v => v.Slot == e.slot && v.IsFree);
        
        if (slot)
        {
            slot.Show(e);
            UpdateState();
            return equipmentVisuals.IndexOf(slot);
        }
        
        inventory.Add(e);
        UpdateState();

        return -1;
    }

    public void UpdateState()
    {
        var equips = equipmentVisuals.Select(v => v.GetEquip()).Where(e => e != default);
        StateManager.Instance.UpdateEquips(equips, inventory);
    }

    public float WalkTo(float pos, float delay, bool showHpAfter, bool hideHpBefore = true)
    {
        var index = 1;
        ghosts.ForEach(g =>
        {
            var i = index;
            g.StartCoroutine(() => g.WalkTo(pos - i * (ghostDistance + ghostGap), 0, false), 0.1f * index);
            index++;
        });
        
        if (hideHpBefore)
        {
            healthDisplay.gameObject.SetActive(false);   
        }

        anim.SetBool(Walking, true);
        var t = transform;
        var p = origin.WhereX(pos);
        var walkTime = Mathf.Max(Mathf.Abs(p.x - t.position.x) * 0.25f, 0.2f);
        
        StopAllCoroutines();

        Tweener.MoveToQuad(t, p, walkTime);
        this.StartCoroutine(() =>
        {
            anim.SetBool(Walking, false);
            healthDisplay.gameObject.SetActive(showHpAfter);
        }, walkTime - 0.1f);

        return walkTime;
    }

    public void Die()
    {
        var cp = center.position;
        
        AudioManager.Instance.PlayEffectAt(4, cp, 1.5f);
        AudioManager.Instance.PlayEffectAt(5, cp, 2f);
        AudioManager.Instance.PlayEffectAt(6, cp, 1.5f);
        AudioManager.Instance.PlayEffectAt(7, cp, 1.5f);
        
        AudioManager.Instance.PlayEffectFromCollection(7, cp, 3f);
        
        EffectManager.AddEffects(new []{ 1, 2, 4, 5 }, cp.WhereZ(0));
        
        cam.BaseEffect(0.4f);
        root.SetActive(false);
        shadow.SetActive(false);
        this.StartCoroutine(() =>
        {
            healthDisplay.gameObject.SetActive(false);
            moveDisplay.gameObject.SetActive(false);
        }, 0.3f);
    }

    public bool Interrupts(int value, Character target)
    {
        var heals = GetSkillCount(SkillType.HealInsteadOnX, value);
        var counters = GetSkillCount(SkillType.AttackInsteadOnX, value);

        if (heals > 0)
        {
            Heal(heals * value);
        }
        
        if (counters > 0)
        {
            Attack(target, counters * value);
        }
        
        return heals + counters > 0;
    }

    public void Heal(int amount)
    {
        var cp = center.position;
        AudioManager.Instance.PlayEffectAt(8, cp, 0.8f);
        EffectManager.AddEffect(3, cp);
        SkillEffect();
        health.Heal(amount);
        var pop = EffectManager.AddTextPopup(amount.ToString(), hitPos.position.RandomOffset(0.2f));
        pop.SetColor(healColor);
        StateManager.Instance.Health = health.Current;
    }

    public void Damage(int amount, int madeWith, bool critical = false)
    {
        var reduced = critical ? amount : Mathf.Max(0, amount - stats.defence);

        var p = hitPos.position;
        
        // AudioManager.Instance.PlayEffectAt(0, p);
        AudioManager.Instance.PlayEffectFromCollection(4, p, 0.5f);
        AudioManager.Instance.PlayEffectFromCollection(6, p, 2.5f);

        anim.SetTrigger(Hurt);
        flasher.Flash();

        if (reduced <= 0)
        {
            cam.BaseEffect(0.1f);
            EffectManager.AddTextPopup("MISS", p.RandomOffset(0.2f));
            AudioManager.Instance.PlayEffectAt(2, p, 0.4f);   
            return;
        }
        
        if (hurtSounds)
        {
            AudioManager.Instance.PlayEffectFromCollection(hurtSounds, p);
        }
        
        cam.BaseEffect(0.2f);
        health.TakeDamage(reduced);

        if (isPlayer)
        {
            DecreaseMulti();
        }

        if (!HasSkill(SkillType.NoStagger) && !HasSkill(SkillType.StaggerOnlyOnX, madeWith))
        {
            Stagger();   
        }

        EffectManager.AddTextPopup(reduced.ToString(), p.RandomOffset(0.2f));
        EffectManager.AddEffect(0, p.RandomOffset(0.2f));
    }

    private void DecreaseMulti()
    {
        if (guards > 0)
        {
            guards--;
            return;
        }

        Board.DecreaseMulti();
    }

    public void Attack(Character target, int amount, bool boosted = true)
    {
        var critical = HasSkill(SkillType.IgnoresDefence);
        var t = transform;
        AttackAnimation();
        var distanceMod = isBoss ? 0.25f : 1f; 
        Tweener.MoveToBounceOut(t, origin + Vector3.right * 1.5f * t.localScale.x * distanceMod, 0.2f);

        var hasSword = GetEquips().Any(e => e.isSword);

        if (hasSword)
        {
            AudioManager.Instance.PlayEffectAt(1, t.position);   
        }
        
        AudioManager.Instance.PlayEffectFromCollection(9, t.position, 1.2f);

        this.StartCoroutine(() =>
        {
            if (hasSword)
            {
                AudioManager.Instance.PlayEffectFromCollection(5, hitPos.position, 0.5f);
            }
            
            target.Damage(boosted ? amount + stats.attack : amount, amount, critical);
            AttackTriggers();
        }, 0.15f);
        this.StartCoroutine(() => Tweener.MoveToQuad(t, origin, 0.2f), 0.25f);

        var thorns = target.GetSkillCount(SkillType.Thorns);
        
        if (thorns > 0 && boosted)
        {
            this.StartCoroutine(() => target.Attack(this, thorns, false), 0.4f);
        }
    }

    public void AttackAnimation()
    {
        anim.SetTrigger(isBoss ? BossAttack : AttackAnim);
    }

    public bool HasSkill(SkillType skill)
    {
        return skills.Any(s => s.Matches(skill));
    }
    
    public bool HasSkill(SkillType skill, int number)
    {
        return skills.Any(s => s.Matches(skill, number));
    }

    public List<Skill> GetSkills(SkillType skill)
    {
        return skills.Where(s => s.Matches(skill)).ToList();
    }
    
    public List<Skill> GetSkills(SkillType skill, int number)
    {
        return skills.Where(s => s.Matches(skill, number)).ToList();
    }

    public int GetSkillCount(SkillType skill)
    {
        return skills.Count(s => s.Matches(skill));
    }
    
    public int GetSkillCount(SkillType skill, int number)
    {
        return skills.Count(s => s.Matches(skill, number));
    }

    public bool IsAlive()
    {
        return health.Current > 0;
    }

    public void SetHealth(int hp)
    {
        health.Init(hp);
    }

    public List<Equip> GetEquips()
    {
        return equipmentVisuals.Select(v => v.GetEquip()).Where(e => e != null).ToList();
    }

    public List<Equip> GetInventory()
    {
        return inventory.ToList();
    }

    public bool SlotMatches(Equip e, int slotIndex)
    {
        return equipmentVisuals[slotIndex].Slot == e.slot;
    }

    public bool CanSlot(Equip e, int slotIndex)
    {
        return equipmentVisuals[slotIndex].Slot == e.slot && !equipmentVisuals[slotIndex].Has(e); 
    }
    
    public bool CanSlotSoul(int slotIndex)
    {
        var e = equipmentVisuals[slotIndex].GetEquip();
        return e != default && e.HasFreeSlot;
    }

    public void StartTimer()
    {
        fightStarted = true;
    }

    public void Slot(Equip e, int slotIndex)
    {
        inventory.Remove(e);
        var target = equipmentVisuals[slotIndex].GetEquip();
        target.Slot(e);
        UpdateState();
        Inventory.Instance.UpdateSlotsFor(target);
    }

    public void AttackTriggers()
    {
        GetSkills(SkillType.ClearCellOnAttack).ForEach(s => Board.ClearRandomCell());
        GetSkills(SkillType.ClearRowOnAttack).ForEach(s => Board.ClearRandomRow());
        GetSkills(SkillType.ClearColOnAttack).ForEach(s => Board.ClearRandomColumn());
        GetSkills(SkillType.ClearSectionOnAttack).ForEach(s => Board.ClearRandomSection());
        
        GetSkills(SkillType.DisableCellOnAttack).ForEach(s => Board.DisableRandomCell());
        GetSkills(SkillType.HideSolvedCellOnAttack).ForEach(s => Board.HideSolvedCell());
    }

    public void ReattachHpDisplay()
    {
        healthDisplay.SetParent(transform, true);
    }

    public void ShowHp()
    {
        healthDisplay.gameObject.SetActive(true);
    }

    public int GetAttack()
    {
        return stats.attack;
    }

    public int GetDefense()
    {
        return stats.defence;
    }

    private List<Equip> GetSouls()
    {
        return GetEquips().SelectMany(e => e.SlottedSouls).ToList();
    }

    private void ShowEquip(Equip e)
    {
        var slot = equipmentVisuals.First(v => v.Slot == e.slot && v.IsFree);
        slot.Show(e);
    }

    private void SpawnGhosts()
    {
        var souls = GetSouls();
        var offset = 1;
        souls.ForEach(s =>
        {
            var index = s.GhostIndex < 0 ? 10 : s.GhostIndex;
            var ghost = Instantiate(mobList.Get(index), transform.position + Vector3.left * (offset * ghostDistance + ghostGap), Quaternion.identity);
            ghost.Ghostify(s.GhostEquips, new Color(s.color.r, s.color.g, s.color.b, 0.6f));
            ghosts.Add(ghost);
            offset++;
        });
    }

    private void Ghostify(List<Equip> equips, Color color)
    {
        equipmentVisuals.ForEach(v => v.Hide());
        isGhost = true;
        healthDisplay.gameObject.SetActive(false);
        timeDisplay.SetActive(false);
        shadow.SetActive(false);
        
        equips.ForEach(ShowEquip);

        // flasher.Colorize(color, true);
        // flasher.ChangeMaterialForAll(ghostMaterial);
        // transparentOnGhost.ForEach(s => s.color = Color.clear);

        flasher.AllSprites.ForEach(s =>
        {
            s.enabled = false;
            if (s.GetComponent<SpriteMask>()) return;
            var mask = s.AddComponent<SpriteMask>();
            mask.sprite = s.sprite;
        });
        
        keepVisibleOnGhost.ForEach(s =>
        {
            s.enabled = true;
            s.color = new Color(1, 1, 1, 0.75f);
        });
        
        ghostFlattener.gameObject.SetActive(true);
        ghostFlattener.color = color;

        Scale(0.5f);
        
        limbs.ForEach(l =>
        {
            l.widthMultiplier = 0.5f;
            l.startColor = l.endColor = color;
            l.material = ghostMaterial;
        });
        
        ghostParticles.SetActive(true);
    }

    public float GetPotionChance()
    {
        return 0.2f * (1 + GetSkillCount(SkillType.MorePots));
    }
}