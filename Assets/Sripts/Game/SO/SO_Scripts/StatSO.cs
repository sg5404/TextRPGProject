using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatSO", menuName = "SO/StatSO")]
public class StatSO : ScriptableObject
{
    public string Name;
    public string Summary;
    public Sprite StatImage;

    [SerializeField] private int HP;
    [SerializeField] private int DMG;
    [SerializeField] private int DEF;
    [SerializeField] private int CRI_PER;
    [SerializeField] private int CRI_DMG;
    [SerializeField] private int AVOID_PER;
    [SerializeField] private int HIT_RATE;

    public int _HP => HP;
    public int _DMG => DMG;
    public int _DEF => DEF;
    public int _CRI_PER => CRI_PER;
    public int _CRI_DMG => CRI_DMG;
    public int _AVOID_PER => AVOID_PER;
    public int _HIT_RATE => HIT_RATE;

}
