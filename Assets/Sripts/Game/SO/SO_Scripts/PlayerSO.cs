using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "SO/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public string Name;
    public string Summary;
    [SerializeField] private int MaxHP;
    [SerializeField] private int CurrentMaxHP;
    [SerializeField] private int CurrentHP;
    [SerializeField] private int ATK;
    [SerializeField] private int CurrentATK;
    [SerializeField] private int DEF;
    [SerializeField] private int CurrentDEF;
    [SerializeField] private int CRI_PER;
    [SerializeField] private int CurrentCRI_PER;
    [SerializeField] private int CRI_DMG;
    [SerializeField] private int CurrentCRI_DMG;
    [SerializeField] private int AVOID_PER;
    [SerializeField] private int CurrentAVOID_PER;
    [SerializeField] private int HIT_RATE;
    [SerializeField] private int CurrentHIT_RATE;

    public List<Skill> Skills;

    public int _MaxHP => MaxHP;
    public int _CurrentMaxHP => CurrentMaxHP;
    public int _CurrentHP => CurrentHP;
    public int _ATK => ATK;
    public int _CurrentATK => CurrentATK;
    public int _DEF => DEF;
    public int _CurrentDEF => CurrentDEF;
    public int _CRI_PER => CRI_PER;
    public int _CurrentCRI_PER => CurrentCRI_PER;
    public int _CRI_DMG => CRI_DMG;
    public int _CurrentCRI_DMG => CurrentCRI_DMG;
    public int _AVOID_PER => AVOID_PER;
    public int _CurrentAVOID_PER => CurrentAVOID_PER;
    public int _HIT_RATE => HIT_RATE;
    public int _CurrentHIT_RATE => CurrentHIT_RATE;

    public void AddStats(Stat stat)
    {
        CurrentMaxHP += stat.HP;
        CurrentHP += stat.HP;
        CurrentATK += stat.ATK;
        CurrentDEF += stat.DEF;
        CurrentCRI_PER += stat.CRI_PER;
        CurrentCRI_DMG += stat.CRI_DMG;
        CurrentAVOID_PER += stat.AVOID_PER;
        CurrentHIT_RATE += stat.HIT_RATE;
    }

    public void FirstInit()
    {
        CurrentMaxHP = MaxHP;
        CurrentHP = CurrentMaxHP;
        CurrentATK = ATK;
        CurrentDEF = DEF;
        CurrentCRI_PER = CRI_PER;
        CurrentCRI_DMG = CRI_DMG;
        CurrentAVOID_PER = AVOID_PER;
        CurrentHIT_RATE = HIT_RATE;
    }

    public void Hit(int DMG)
    {
        CurrentHP -= DMG;
    }

    public void FullHeal()
    {
        CurrentHP = CurrentMaxHP;
    }
}

[System.Serializable]
public class Stat
{
    public int HP;
    public int ATK;
    public int DEF;
    public int CRI_PER;
    public int CRI_DMG;
    public int AVOID_PER;
    public int HIT_RATE;
}

[System.Serializable]
public class Skill
{
    public string SkillName; //스킬 이름
    public SkillType S_Type;
    public Sprite SkillImage;
    public string SkillSummary; //스킬 설명
    public int SkillDamagePercent; //공격력 계수
    public int SkillAttackAcount; //공격 횟수
}

public enum SkillType
{
    Attack,
    Shield,
}