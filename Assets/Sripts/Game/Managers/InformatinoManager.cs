using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformatinoManager : MonoBehaviour
{
    [SerializeField] private Button InfoButton;
    [SerializeField] private Transform InfoPannel;
    [SerializeField] private Transform BackGroundPannel;
    [SerializeField] private Button OKButton;

    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private TextMeshProUGUI ATK_Text;
    [SerializeField] private TextMeshProUGUI DEF_Text;
    [SerializeField] private TextMeshProUGUI CRI_Text;
    [SerializeField] private TextMeshProUGUI AVOID_Text;

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
        
        if (isActive == true)
        {
            StatSet();
        }
    }

    void StatSet()
    {
        ATK_Text.text = playerSO._CurrentATK.ToString();
        DEF_Text.text = playerSO._CurrentDEF.ToString();
        CRI_Text.text = playerSO._CurrentCRI_PER.ToString() + "%";
        AVOID_Text.text = playerSO._CurrentAVOID_PER.ToString() + "%";
    }
}
