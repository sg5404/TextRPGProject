using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "SO/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public string Name;
    public string Summary;
    [SerializeField] private int MaxHP;
    [SerializeField] private int CurrentHP;
    [SerializeField] private int DEF;
    [SerializeField] private int CRI_PER;
    [SerializeField] private int CRI_DMG;
    [SerializeField] private int AVOID_PER;
    [SerializeField] private int HIT_RATE;

    public int _MaxHP => MaxHP;
    public int _CurrentHP => CurrentHP;
    public int _DEF => DEF;
    public int _CRI_PER => CRI_PER;
    public int _CRI_DMG => CRI_DMG;
    public int _AVOID_PER => AVOID_PER;
    public int _HIT_RATE => HIT_RATE;

    void AddStats(Stat stat)
    {
        MaxHP += stat.HP;
        CurrentHP += stat.HP;
        DEF += stat.DEF;
        CRI_PER += stat.CRI_PER;
        CRI_DMG += stat.CRI_DMG;
        AVOID_PER += stat.AVOID_PER;
        HIT_RATE += stat.HIT_RATE;
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
