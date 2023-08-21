using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoSingleton<BattleManager>
{
    [SerializeField] private PlayerSO PSO;
    [SerializeField] private List<PlayerSO> EnemySOs;

    [SerializeField] private TextMeshProUGUI PlayerNameText;
    [SerializeField] private TextMeshProUGUI EnemyNameText;
    public void BattleStart(int StageNum)
    {
        PlayerNameText.text = PSO.Name;
        EnemyNameText.text = EnemySOs[StageNum].Name;
    }
}
