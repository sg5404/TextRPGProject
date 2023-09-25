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

    [SerializeField] private TextMeshProUGUI HP_TEXT;

    private List<StatSO> CurrentStats = new List<StatSO>();
    private List<StatSO> CurrentWeaponStats = new List<StatSO>();

    private List<StatSO> UsedStats = new List<StatSO>();
    private List<StatSO> UsedWeaponStats = new List<StatSO>();

    private int nowStage = 0;

    public void SetButtonEvent(List<Events> events, int StageNum)
    {   
        for(int i = 0; i < events.Count; i++)
        {
            SelectButtons[i].onClick.RemoveAllListeners();
            SelectButtons[i].onClick.AddListener(events[i].EventStart);
        }

        ActProgressBar.fillAmount = (float)StageNum / 15f;
        ActProgressText.text = $"{StageNum}/15";
        nowStage = StageNum;
    }

    public void RewardPannelSetActive(bool isActive)
    {
        BackgroundPannel.gameObject.SetActive(isActive);
        RewardPannel.gameObject.SetActive(isActive);    

        if(!isActive) return;
        if(isFirstReward) SetRewardButton(WeaponStats, ref CurrentWeaponStats, UsedWeaponStats);
        else SetRewardButton(Stats, ref CurrentStats, UsedStats);
        HP_TEXT.text = $"{playerSO._CurrentHP} / {playerSO._CurrentMaxHP}";
    }

    public void BattlePannelSetActive(bool isActive)
    {
        BackgroundPannel.gameObject.SetActive(isActive);
        BattlePannel.gameObject.SetActive(isActive);
        UpgradeBattleManager.Instance.BattleSet(nowStage - 1);
        HP_TEXT.text = $"{playerSO._CurrentHP/playerSO._CurrentMaxHP}";
    }

    public void SetRewardButton(List<StatSO> _stats, ref List<StatSO> _currentStats, List<StatSO> _usedStats)
    {
        if(_usedStats.Count >= _stats.Count - 3) 
        {
            Debug.LogError("더 이상 스탯 설정 불가, 스탯 추가 필요");
            return;
        }

        for(int i = 0; i < RewardButtons.Count; i++)
        {
            int statIndex = Random.Range(0, _stats.Count);
            while(_usedStats.Contains(_stats[statIndex]) || _currentStats.Contains(_stats[statIndex]))
            {
               if(_usedStats.Count >= _stats.Count - 1) break;
               statIndex = Random.Range(0, _stats.Count);
            }
            _currentStats.Add(_stats[statIndex]);
            RewardButtons[i]?.GetComponent<RewardButtonSetting>().Setting(_stats[statIndex]);
        }
    }

    public void RewardButtonClick(int num)
    {
        //받아온 num 으로 몇번째에 들어가 있는 강화를 선택할지 고를 수 있음
        //디버그용
        Debug.Log(num);
        RewardPannelSetActive(false);
        playerSO.AddStats(RewardButtons[num]?.GetComponent<RewardButtonSetting>().RewardStat);
        if(!isFirstReward && CurrentStats.Count > 2)
        {
            UsedStats.Add(CurrentStats[num]);
            CurrentStats.Clear();
        }
        //다음스테이지로 넘어가기
        StageManager.Instance.NextStage();
        FirstReward();
    }

    public void FirstReward()
    {
        isFirstReward = false;
    }
}
