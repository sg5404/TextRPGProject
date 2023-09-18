using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartHolder : MonoBehaviour
{
    [SerializeField] private int ButtonNum;

    public void Clicked()
    {
        UpgradeBattleManager.Instance.PartAttackPannelSetActive(false);
        UpgradeBattleManager.Instance.SetAttackPart((PartKind)ButtonNum);
        UpgradeBattleManager.Instance.PlayerAction(0);
    }
}
