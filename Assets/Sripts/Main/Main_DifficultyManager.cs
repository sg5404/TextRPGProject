using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DifNameAndDifSummary
{
    public string Name;
    public string Summary;
}
public class Main_DifficultyManager : MonoBehaviour
{
    [SerializeField] private Button Dif_HighButton;
    [SerializeField] private Button Dif_LowButton;

    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI SummaryText;

    [SerializeField] private List<DifNameAndDifSummary> DNAD;

    public int Dif_Num = 0;

    private void Start()
    {
        Dif_Num = 0;
        ButtonInit();
        UpdateUI();
    }

    void ButtonInit()
    {
        Dif_HighButton.onClick.RemoveAllListeners();
        Dif_LowButton.onClick.RemoveAllListeners();

        Dif_HighButton.onClick.AddListener(DifNumPlus);
        Dif_LowButton.onClick.AddListener(DifNumMinus);
    }

    void DifNumPlus()
    {
        Dif_Num++;
        Dif_Num = Mathf.Clamp(Dif_Num, 0, DNAD.Count - 1);
        UpdateUI();
    }

    void DifNumMinus()
    {
        Dif_Num--;
        Dif_Num = Mathf.Clamp(Dif_Num, 0, DNAD.Count - 1);
        UpdateUI();
    }

    void UpdateUI()
    {
        NameText.text = DNAD[Dif_Num].Name;
        SummaryText.text = DNAD[Dif_Num].Summary;

        Dif_LowButton.interactable = true;
        Dif_HighButton.interactable = true;

        if (Dif_Num == 0) Dif_LowButton.interactable = false;
        if(Dif_Num == DNAD.Count - 1) Dif_HighButton.interactable = false;
    }
}
