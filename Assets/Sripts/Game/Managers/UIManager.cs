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
        //�޾ƿ� num ���� ���°�� �� �ִ� ��ȭ�� �������� �� �� ����
        //����׿�
        Debug.Log(num);

        RewardPannelSetActive(false);

        //�������������� �Ѿ��(������ ��)
        StageManager.Instance.NextStage();
    }
}
