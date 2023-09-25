using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    [SerializeField] private PlayerSO Player;

    [SerializeField] private int ActNum = 0;
    [SerializeField] private int StageNum = 0;

    [SerializeField] private List<StageSO> StageList;

    [SerializeField] private List<Events> _Events;

    [SerializeField] private List<StageSO> RandomStages;
    private List<StageSO> usedRandomStages = new List<StageSO>();

    private void Start()
    {
        ActAndStageInit();
        var C_Stage = StageList[StageNum++];
        PrintManager.Instance.SetStage(C_Stage.StageSummary, C_Stage.StageSprite, C_Stage.OptionCount, C_Stage.Options);
        UIManager.Instance.SetButtonEvent(SelectEvent(C_Stage), StageNum);
    }

    public void NextStage()
    {
        var C_Stage = StageList[StageNum++];
        Debug.Log(C_Stage.isRandomStage);
        if(!C_Stage.isRandomStage)
        {
            PrintManager.Instance.SetStage(C_Stage.StageSummary, C_Stage.StageSprite, C_Stage.OptionCount, C_Stage.Options);
            UIManager.Instance.SetButtonEvent(SelectEvent(C_Stage), StageNum);
        }
        else
        {
            int index = Random.Range(0, RandomStages.Count);
            while(usedRandomStages.Contains(RandomStages[index]))
            {
               Debug.Log("while");
               if(usedRandomStages.Count >= RandomStages.Count - 1) break;
               index = Random.Range(0, RandomStages.Count);
            }
            usedRandomStages.Add(RandomStages[index]);     
            var randomStage = RandomStages[index];
            if(randomStage == null) return;
            PrintManager.Instance.SetStage(randomStage.StageSummary, randomStage.StageSprite, randomStage.OptionCount, randomStage.Options);
            UIManager.Instance.SetButtonEvent(SelectEvent(randomStage), StageNum);
        }
    }

    void ActAndStageInit()
    {
        ActNum = StageNum = 0;
        Player.FirstInit();
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
                EventKinds.Battle => _Events[1],
                EventKinds.Skip => _Events[2],
                EventKinds.Random => RandomEvents(),
                _ => _Events[0],
            };

            if (EventVar == null) break;

            Events_.Add(EventVar);
        }

        return Events_;
    }

    Events RandomEvents()
    {
        int index = Random.Range(0,2);

        if(_Events[index] == null)
        {
            Debug.LogError("Event not Available");
            return null;
        }
        return _Events[index];
    }
}
