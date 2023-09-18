using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private int ButtonNum;

    public void Clicked()
    {
        if(ButtonNum == 0)
        {
            UpgradeBattleManager.Instance.PartAttackPannelSetActive(true);
        }
        else UpgradeBattleManager.Instance.PlayerAction(ButtonNum);
    }
}
