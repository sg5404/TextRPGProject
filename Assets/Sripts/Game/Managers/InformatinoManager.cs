using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformatinoManager : MonoBehaviour
{
    [SerializeField] private Button InfoButton;
    [SerializeField] private Transform InfoPannel;
    [SerializeField] private Transform BackGroundPannel;
    [SerializeField] private Button OKButton;

    private void Start()
    {
        ButtonSet();
    }

    void ButtonSet()
    {
        InfoButton.onClick.RemoveAllListeners();
        OKButton.onClick.RemoveAllListeners();

        InfoButton.onClick.AddListener(() => InfoPannelSetActive(true));
        OKButton.onClick.AddListener(() => InfoPannelSetActive(false));
    }

    void InfoPannelSetActive(bool isActive)
    {
        InfoPannel.gameObject.SetActive(isActive);
        BackGroundPannel.gameObject.SetActive(isActive);
    }
}
