using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardButtonSetting : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Summary;
    public Image StatImage;

    private Stat rewardStat = new Stat();
    public Stat RewardStat => rewardStat;

    public void Setting(StatSO _statSO)
    {
        Name.text = _statSO.Name;
        Summary.text = _statSO.Summary;
        StatImage.sprite = _statSO.StatImage;

        rewardStat.HP = _statSO._HP;
        rewardStat.DEF = _statSO._DEF;
        rewardStat.ATK = _statSO._ATK;
        rewardStat.CRI_PER = _statSO._CRI_PER;
        rewardStat.CRI_DMG = _statSO._CRI_DMG;
        rewardStat.AVOID_PER = _statSO._AVOID_PER;
        rewardStat.HIT_RATE = _statSO._HIT_RATE;
    }
}
