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
    }

    public void BattlePannelSetActive(bool isActive)
    {
        BackgroundPannel.gameObject.SetActive(isActive);
        BattlePannel.gameObject.SetActive(isActive);
        BattleManager.Instance.BattleSet(nowStage - 1);
    }

    public void RewardButtonClick(int num)
    {
        //받아온 num 으로 몇번째에 들어가 있는 강화를 선택할지 고를 수 있음
        //디버그용
        Debug.Log(num);

        RewardPannelSetActive(false);

        //다음스테이지로 넘어가기
        StageManager.Instance.NextStage();
    }
}
