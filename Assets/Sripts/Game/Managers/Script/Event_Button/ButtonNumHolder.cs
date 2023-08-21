using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonNumHolder : MonoBehaviour
{
    [SerializeField] private int ButtonNum;
 
    void Start()
    {
        var Btn = transform.GetComponent<Button>();
        Btn.onClick.RemoveAllListeners();
        Btn.onClick.AddListener(ClickButton);
    }

    void ClickButton()
    {
        UIManager.Instance.RewardButtonClick(ButtonNum);
    }
}
