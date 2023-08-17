using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Transform RewardPannel;

    [SerializeField] private List<Button> SelectButtons;


    public void SetButtonEvent(List<Events> events)
    {   
        for(int i = 0; i < events.Count; i++)
        {

        }
    }

    public void RewardPannelSetActive(bool isActive)
    {
        RewardPannel.gameObject.SetActive(isActive);    
    }
}
