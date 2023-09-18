using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Image ActProgressBar;
    [SerializeField] private TextMeshProUGUI ActProgressText;

    [SerializeField] private Transform BattlePannel;
    [SerializeField] private Transform RewardPannel;
    [SerializeField] private Transform BackgroundPannel;

    [SerializeField] private List<Button> SelectButtons;

    [SerializeField] private List<Button> RewardButtons;

    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private List<StatSO> Stats;
    [SerializeField] private List<StatSO> WeaponStats;
    [SerializeField] private bool isFirstReward = true;
    private List<int> CurrentStats = new List<int>();
    private List<int> CurrentWeaponStats = new List<int>();

    private int nowStage = 0;

    public void SetButtonEvent(List<Events> events, int StageNum)
    {   
        for(int i = 0; i < events.Count; i++)
        {
            SelectButtons[i].onClick.RemoveAllListeners();
            SelectButtons[i].onClick.AddListener(events[i].EventStart);
        }

        ActProgressBar.fillAmount = (float)StageNum / 12f;
        ActProgressText.text = $"{StageNum}/12";
        nowStage = StageNum;
    }

    public void RewardPannelSetActive(bool isActive)
    {
        BackgroundPannel.gameObject.SetActive(isActive);
        RewardPannel.gameObject.SetActive(isActive);    

        if(!isActive) return;
        if(isFirstReward) SetRewardButton(WeaponStats, CurrentWeaponStats);
        else SetRewardButton(Stats, CurrentStats);
    }

    public void BattlePannelSetActive(bool isActive)
    {
        BackgroundPannel.gameObject.SetActive(isActive);
        BattlePannel.gameObject.SetActive(isActive);
        UpgradeBattleManager.Instance.BattleSet(nowStage - 1);
    }

    public void SetRewardButton(List<StatSO> _stats, List<int> _currentStats)
    {
        if(_currentStats.Count >= _stats.Count - 3) 
        {
            Debug.LogError("더 이상 스탯 설정 불가");
            return;
        }

        for(int i = 0; i < RewardButtons.Count; i++)
        {
            int statIndex = Random.Range(0, _stats.Count - 1);
            while(_currentStats.Contains(statIndex))
            {
               if(_currentStats.Count == _stats.Count - 1) break;
               statIndex = Random.Range(0, _stats.Count - 1);
            }                        
            RewardButtons[i]?.GetComponent<RewardButtonSetting>().Setting(_stats[statIndex]);
            _currentStats.Add(statIndex);
        }
        isFirstReward = false;
    }

    public void RewardButtonClick(int num)
    {
        //받아온 num 으로 몇번째에 들어가 있는 강화를 선택할지 고를 수 있음
        //디버그용
        Debug.Log(num);
        RewardPannelSetActive(false);
        playerSO.AddStats(RewardButtons[num]?.GetComponent<RewardButtonSetting>().RewardStat);
        //다음스테이지로 넘어가기
        StageManager.Instance.NextStage();
        FirstReward();
    }

    public void FirstReward()
    {
        isFirstReward = false;
    }
}
