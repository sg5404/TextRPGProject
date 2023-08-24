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
        //�޾ƿ� num ���� ���°�� �� �ִ� ��ȭ�� �������� �� �� ����
        //����׿�
        Debug.Log(num);

        RewardPannelSetActive(false);

        //�������������� �Ѿ��
        StageManager.Instance.NextStage();
    }
}
