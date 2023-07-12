using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int ActNum = 0;
    [SerializeField] private int StageNum = 0;

    [SerializeField] private List<StageSO> StageList;

    private void Start()
    {
        ActAndStageInit();
        var C_Stage = StageList[0];
        PrintManager.Instance.SetStage(C_Stage.StageSummary, C_Stage.StageSprite, C_Stage.OptionCount, C_Stage.Options);
    }

    void ActAndStageInit()
    {
        ActNum = StageNum = 0;
    }
}
