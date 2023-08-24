using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoSingleton<BattleManager>
{
    private int NowStage;
    [SerializeField] private PlayerSO PSO;
    [SerializeField] private List<PlayerSO> EnemySOs;

    [SerializeField] private TextMeshProUGUI PlayerNameText;
    [SerializeField] private TextMeshProUGUI EnemyNameText;

    [SerializeField] private Image Player_HP;
    [SerializeField] private Image Enemy_HP;
    [SerializeField] private TextMeshProUGUI Player_HP_Text;
    [SerializeField] private TextMeshProUGUI Enemy_HP_Text;

    [SerializeField] private List<TextMeshProUGUI> Player_Skill_Texts;
    [SerializeField] private List<Button> Player_Skill_Button;
    [SerializeField] private List<Image> Player_Skill_Image;

    public void BattleSet(int StageNum)
    {
        NowStage = StageNum;
        //���� �÷��̾��� �̸� �ٲ��ֱ�
        PlayerNameText.text = PSO.Name;
        EnemyNameText.text = EnemySOs[NowStage].Name;
        //���� HP�� �÷��̾��� HPǥ��
        UpdateHP();
    }

    /// <summary>
    /// ����� �÷��̾�� ���� hp�� ���Ʈ ���ִ� �Լ�
    /// </summary>
    void UpdateHP()
    {
        Player_HP.fillAmount = (float)PSO._CurrentHP / (float)PSO._MaxHP;
        Player_HP_Text.text = $"{PSO._CurrentHP}/{PSO._MaxHP}";

        Enemy_HP.fillAmount = (float)EnemySOs[NowStage]._CurrentHP / (float)EnemySOs[NowStage]._MaxHP;
        Enemy_HP_Text.text = $"{EnemySOs[NowStage]._CurrentHP}/{EnemySOs[NowStage]._MaxHP}";
    }

    void UpdateSkill()
    {
        for(int num = 0; num < PSO.Skills.Count; num++)
        {

        }
    }
}
