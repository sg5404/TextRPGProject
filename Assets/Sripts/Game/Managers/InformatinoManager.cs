using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformatinoManager : MonoBehaviour
{
    [SerializeField] private Button InfoButton;
    [SerializeField] private Button InfoButton2;
    [SerializeField] private Button EnemyButton;
    [SerializeField] private Transform InfoPannel;
    [SerializeField] private Transform BackGroundPannel;
    [SerializeField] private Button OKButton;

    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private TextMeshProUGUI ATK_Text;
    [SerializeField] private TextMeshProUGUI DEF_Text;
    [SerializeField] private TextMeshProUGUI CRI_Text;
    [SerializeField] private TextMeshProUGUI AVOID_Text;
    [SerializeField] private Image CharacterImage;

    private void Start()
    {
        ButtonSet();
        
    }

    void ButtonSet()
    {
        InfoButton.onClick.RemoveAllListeners();
        InfoButton2.onClick.RemoveAllListeners();
        EnemyButton.onClick.RemoveAllListeners();
        OKButton.onClick.RemoveAllListeners();

        InfoButton.onClick.AddListener(() => InfoPannelSetActive(true));
        InfoButton2.onClick.AddListener(() => InfoPannelSetActive(true));
        EnemyButton.onClick.AddListener(() => Enemy_InfoPAnnelSetActive(true));
        OKButton.onClick.AddListener(() => InfoPannelSetActive(false));
    }

    void InfoPannelSetActive(bool isActive)
    {
        InfoPannel.gameObject.SetActive(isActive);
        BackGroundPannel.gameObject.SetActive(isActive);


        if (isActive == true)
        {
            StatSet(playerSO);
        }
    }

    void Enemy_InfoPAnnelSetActive(bool isActive)
    {
        InfoPannel.gameObject.SetActive(isActive);
        BackGroundPannel.gameObject.SetActive(isActive);

        if(isActive == true)
        {
            StatSet(UpgradeBattleManager.Instance.Current_EnemySO());
        }
    }

    void StatSet(PlayerSO SO)
    {
        CharacterImage.sprite = SO.Image;
        ATK_Text.text = SO._CurrentATK.ToString();
        DEF_Text.text = SO._CurrentDEF.ToString();
        CRI_Text.text = SO._CurrentCRI_PER.ToString() + "%";
        AVOID_Text.text = SO._CurrentAVOID_PER.ToString() + "%";
    }
}
