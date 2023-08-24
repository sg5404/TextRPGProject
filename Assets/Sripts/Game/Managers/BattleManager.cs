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
        //적과 플레이어의 이름 바꿔주기
        PlayerNameText.text = PSO.Name;
        EnemyNameText.text = EnemySOs[NowStage].Name;
        //적의 HP와 플레이어의 HP표시
        UpdateHP();
    }

    /// <summary>
    /// 변경된 플레이어와 적의 hp를 어데이트 해주는 함수
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
