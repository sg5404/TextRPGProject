using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoSingleton<BattleManager>
{
    private int NowStage;

    private int EnemyDamage;

    private int PlayerDamage;
    private int PlayerShield;

    private string EnemySkillName;
    private int EnemyAttackAmount;

    private bool isPlayerAction = false;
    private bool isTurnStart = false;
    private bool IsCriticaled = false;

    private List<Skill> EnemySkills = new List<Skill>();
    private List<Skill> PlayerSkills = new List<Skill>();

    [SerializeField] private TextMeshProUGUI TextPrefab;
    [SerializeField] private Transform BattleContent;

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
        EnemySOs[NowStage].FirstInit();
        //적의 HP와 플레이어의 HP표시
        UpdateHP();
        UpdateSkill();
        SetEnemySkills();
        //BattleStart();
        StartCoroutine(_BattleStart());
    }

    /// <summary>
    /// 변경된 플레이어와 적의 hp를 업데이트 해주는 함수
    /// </summary>
    void UpdateHP()
    {
        Player_HP.fillAmount = (float)PSO._CurrentHP / (float)PSO._CurrentMaxHP;
        Player_HP_Text.text = $"{PSO._CurrentHP}/{PSO._CurrentMaxHP}";

        Enemy_HP.fillAmount = (float)EnemySOs[NowStage]._CurrentHP / (float)EnemySOs[NowStage]._CurrentMaxHP;
        Enemy_HP_Text.text = $"{EnemySOs[NowStage]._CurrentHP}/{EnemySOs[NowStage]._CurrentMaxHP}";
    }

    void UpdateSkill()
    {
        PlayerSkills.Clear();

        for (int num = 0; num < PSO.Skills.Count; num++)
        {
            Player_Skill_Button[num].interactable = false;
            Player_Skill_Texts[num].text = "";
            Player_Skill_Image[num].gameObject.SetActive(false);

            if (PSO.Skills[num].SkillName != "")
            {
                Player_Skill_Button[num].interactable = true;
                Player_Skill_Texts[num].text = PSO.Skills[num].SkillName;
                Player_Skill_Image[num].gameObject.SetActive(true);
                Player_Skill_Image[num].sprite = PSO.Skills[num].SkillImage;

                PlayerSkills.Add(PSO.Skills[num]);
            }
        }
    }

    void SetEnemySkills()
    {
        EnemySkills.Clear();

        for (int num = 0; num < EnemySOs[NowStage].Skills.Count; num++)
            EnemySkills.Add(EnemySOs[NowStage].Skills[num]);
    }

    //void BattleStart()
    //{
    //    isTurnStart = true;

    //    //출력하고 70씩 포지션 내려주면 될듯?
    //    while (true)
    //    {
    //        if (isTurnStart) EnemyAttack();

    //        if (!isPlayerAction) continue;

    //        DamageCalculatel(); //데미지 계산
    //    }
    //}

    IEnumerator _BattleStart()
    {
        isTurnStart = true;
        isPlayerAction = true;

        while (true)
        {
            if (isTurnStart) EnemyAttack();

            yield return new WaitForSeconds(0.1f);

            if (isPlayerAction) continue;

            StartCoroutine(DamageCalculatel());
            yield return new WaitForSeconds(4f);
        }
    }

    void EnemyAttack()
    {
        isTurnStart = false;

        int SkillNum = Random.Range(0, 4);
        var AttackText = Instantiate(TextPrefab);
        AttackText.transform.SetParent(BattleContent);
        AttackText.text = EnemySkills[SkillNum].SkillSummary;
        TextDown();

        EnemyDamage = 0;
        EnemyDamage = (EnemySkills[SkillNum].SkillDamagePercent * EnemySOs[NowStage]._CurrentATK) / 100;
        EnemySkillName = EnemySkills[SkillNum].SkillName;
        EnemyAttackAmount = EnemySkills[SkillNum].SkillAttackAcount;

        isPlayerAction = true;

        for (int num = 0; num < PlayerSkills.Count; num++)
        {
            Player_Skill_Button[num].interactable = true;
        }

        StartCoroutine(TextDown_());
    }

    private IEnumerator TextDown_()
    {
        yield return new WaitForSeconds(0.5f);
        TextDown();
    }

    ////여기에 치명타 판정해야할듯?
    //public void PlayerAction(int SkillNum)
    //{
    //    for(int num = 0; num < Player_Skill_Button.Count; num++)
    //    {
    //        Player_Skill_Button[num].interactable = false;
    //    }

    //    var ActionText = Instantiate(TextPrefab);
    //    ActionText.transform.SetParent(BattleContent);
    //    ActionText.text = $"당신은 {PlayerSkills[SkillNum].SkillName}을 사용했다.";
    //    TextDown();
    //    TextDown();

    //    PlayerDamage = 0;
    //    PlayerShield = 0;

    //    if (PlayerSkills[SkillNum].S_Type == SkillType.Attack)
    //    {
    //        PlayerDamage = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentATK) / 100;
    //        float temp = 0;
    //        if (IsCritical(PSO._CurrentCRI_PER))
    //        {
    //            temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
    //            PlayerDamage = (int)temp;
    //            IsCriticaled = true;
    //        }
    //    }
    //    else PlayerShield = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentDEF) / 100;

    //    isPlayerAction = false;
    //}

    /// <summary>
    /// 플레이어가 조작하는 행동들을 실행하는 함수
    /// </summary>
    /// <param name="SkillNum"></param>
    public void PlayerAction(int SkillNum)
    {
        for (int num = 0; num < Player_Skill_Button.Count; num++)
        {
            Player_Skill_Button[num].interactable = false;
        }

        var ActionText = Instantiate(TextPrefab);
        ActionText.transform.SetParent(BattleContent);
        ActionText.text = $"당신은 {PlayerSkills[SkillNum].SkillName}을 사용했다.";
        TextDown();
        TextDown();

        BattleInit();

        UnityAction Action = PlayerSkills[SkillNum].S_Type switch
        {
            SkillType.Attack => () => Attack(SkillNum),
            SkillType.Shield => () => Shield(SkillNum),
            _ => () => Attack(SkillNum),
        };
        Action();

        //else PlayerShield = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentDEF) / 100;

        isPlayerAction = false;
    }

    void BattleInit()
    {
        PlayerDamage = 0;
        PlayerShield = 0;
    }

    /// <summary>
    /// 플레이어가 공격을 사용했을때 발동해주는 함수
    /// </summary>
    void Attack(int SkillNum)
    {
        float temp = 0;
        //기본 데미지 저장
        PlayerDamage = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentATK) / 100;
        Debug.Log(PlayerDamage);

        //크리티컬 판정
        if (IsCritical(PSO._CurrentCRI_PER))
        {
            temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
            PlayerDamage = (int)temp;
            IsCriticaled = true;
        }
    }

    /// <summary>
    /// 플레이어가 쉴드를 사용했을때 발동해주는 함수
    /// </summary>
    /// <param name="SkillNum"></param>
    void Shield(int SkillNum)
    {
        //쉴드량 저장
        PlayerShield = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentDEF) / 100;
    }

    IEnumerator DamageCalculatel()
    {
        TextDown();
        yield return new WaitForSeconds(0.5f);

        int RealDamage = 0;
        int DecreaseDamage = 0;
        int DefenseDamage = 0;

        yield return new WaitForSeconds(0.5f);
        if (PlayerDamage > 0)
        {
            RealDamage = PlayerDamage - EnemySOs[NowStage]._CurrentDEF;
            RealDamage = Mathf.Clamp(RealDamage, 0, 1000);

            var ResultText = Instantiate(TextPrefab);
            ResultText.transform.SetParent(BattleContent);
            ResultText.text = "";
            if(IsCriticaled)
            {
                IsCriticaled = false;
                ResultText.text += "치명타! "; 
            }
            ResultText.text += $"공격으로 {RealDamage} 피해를 입혔다.";
            TextDown();
            TextDown();

            Hit(EnemySOs[NowStage], RealDamage);
        }
        else
        {
            var ResultText = Instantiate(TextPrefab);
            ResultText.transform.SetParent(BattleContent);
            ResultText.text = $"수비로 쉴드를{PlayerShield} 얻었다.";
            TextDown();
            TextDown();
        }

        yield return new WaitForSeconds(0.5f);

        for (int num = 0; num < EnemyAttackAmount; num++)
        {
            DecreaseDamage = Min(EnemyDamage, PSO._CurrentDEF);
            DefenseDamage = Min(PlayerShield, (EnemyDamage - PSO._CurrentDEF));
            DefenseDamage = Mathf.Clamp(DefenseDamage, 0, 1000);
            RealDamage = EnemyDamage - PSO._CurrentDEF - PlayerShield;
            PlayerShield -= (EnemyDamage - PSO._CurrentDEF);
            RealDamage = Mathf.Clamp(RealDamage, 0, 1000);

            var EnemyText1 = Instantiate(TextPrefab);
            EnemyText1.transform.SetParent(BattleContent);
            EnemyText1.text = $"적의 {EnemySkillName}에 {RealDamage} 피해를 입었다.";
            TextDown();

            //yield return new WaitForSeconds(0.5f);

            var EnemyText2 = Instantiate(TextPrefab);
            EnemyText2.transform.SetParent(BattleContent);
            EnemyText2.text = $"({DecreaseDamage}감소됨) ({DefenseDamage}방어됨)";
            TextDown();

            Hit(PSO, RealDamage);

            yield return new WaitForSeconds(0.5f);
            TextDown();
        }

        isTurnStart = true;
    }

    void Hit(PlayerSO Target, int DMG)
    {
        Target.Hit(DMG);
        UpdateHP();
    }

    int Min(int num1, int num2)
    {
        return num1 > num2 ? num2 : num1;
    }

    void TextDown()
    {
        BattleContent.position = new Vector3(BattleContent.position.x, BattleContent.position.y + 70f, BattleContent.position.z);
    }

    bool IsCritical(int CRI_PER)
    {
        int Rnum = Random.Range(0, 100);
        return Rnum < CRI_PER ? true : false;
    }
}
