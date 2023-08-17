using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Transform RewardPannel;
    [SerializeField] private Transform BackgroundPannel;

    [SerializeField] private List<Button> SelectButtons;

    [SerializeField] private List<Button> RewardButtons;

    public void SetButtonEvent(List<Events> events)
    {   
        for(int i = 0; i < events.Count; i++)
        {
            SelectButtons[i].onClick.RemoveAllListeners();
            SelectButtons[i].onClick.AddListener(events[i].EventStart);
        }
    }

    public void RewardPannelSetActive(bool isActive)
    {
        BackgroundPannel.gameObject.SetActive(isActive);
        RewardPannel.gameObject.SetActive(isActive);    
    }

    public void RewardButtonClick(int num)
    {
        //받아온 num 으로 몇번째에 들어가 있는 강화를 선택할지 고를 수 있음
        //디버그용
        Debug.Log(num);

        RewardPannelSetActive(false);

        //다음스테이지로 넘어가기(만들어야 함)
        StageManager.Instance.NextStage();
    }
}
