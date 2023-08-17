using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int ActNum = 0;
    [SerializeField] private int StageNum = 0;

    [SerializeField] private List<StageSO> StageList;

    [SerializeField] private List<Events> _Events;

    private void Start()
    {
        ActAndStageInit();
        var C_Stage = StageList[0];
        PrintManager.Instance.SetStage(C_Stage.StageSummary, C_Stage.StageSprite, C_Stage.OptionCount, C_Stage.Options);
        UIManager.Instance.SetButtonEvent(SelectEvent(C_Stage));
    }

    void ActAndStageInit()
    {
        ActNum = StageNum = 0;
    }

    List<Events> SelectEvent(StageSO stage)
    {
        List<Events> Events_ = new List<Events>();

        for (int i = 0; i < 2; i++)
        {
            EventKinds event_k = stage.Options[i].EventKind;

            var EventVar = event_k switch
            {
                EventKinds.None => null,
                EventKinds.Reward => _Events[0],
                _ => _Events[0],
            };

            if (EventVar == null) break;

            Events_.Add(EventVar);
        }

        return Events_;
    }
}
