using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private int ButtonNum;

    public void Clicked()
    {
        BattleManager.Instance.PlayerAction(ButtonNum);
    }
}
