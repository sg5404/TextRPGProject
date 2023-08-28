using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        //���� �÷��̾��� �̸� �ٲ��ֱ�
        PlayerNameText.text = PSO.Name;
        EnemyNameText.text = EnemySOs[NowStage].Name;
        EnemySOs[NowStage].FullHeal();
        //���� HP�� �÷��̾��� HPǥ��
        UpdateHP();
        UpdateSkill();
        SetEnemySkills();
        //BattleStart();
        StartCoroutine(_BattleStart());
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

    //    //����ϰ� 70�� ������ �����ָ� �ɵ�?
    //    while (true)
    //    {
    //        if (isTurnStart) EnemyAttack();

    //        if (!isPlayerAction) continue;

    //        DamageCalculatel(); //������ ���
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
        EnemyDamage = (EnemySkills[SkillNum].SkillDamagePercent * EnemySOs[NowStage]._ATK) / 100;
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

    public void PlayerAction(int SkillNum)
    {
        for(int num = 0; num < Player_Skill_Button.Count; num++)
        {
            Player_Skill_Button[num].interactable = false;
        }

        var ActionText = Instantiate(TextPrefab);
        ActionText.transform.SetParent(BattleContent);
        ActionText.text = $"����� {PlayerSkills[SkillNum].SkillName}�� ����ߴ�.";
        TextDown();
        TextDown();

        PlayerDamage = 0;
        PlayerShield = 0;

        if (PlayerSkills[SkillNum].S_Type == SkillType.Attack) PlayerDamage = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._ATK) / 100;
        else PlayerShield = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._DEF) / 100;

        isPlayerAction = false;
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
            RealDamage = PlayerDamage - EnemySOs[NowStage]._DEF;
            RealDamage = Mathf.Clamp(RealDamage, 0, 1000);

            var ResultText = Instantiate(TextPrefab);
            ResultText.transform.SetParent(BattleContent);
            ResultText.text = $"�������� {RealDamage} ���ظ� ������.";
            TextDown();
            TextDown();

            Hit(EnemySOs[NowStage], RealDamage);
        }
        else
        {
            var ResultText = Instantiate(TextPrefab);
            ResultText.transform.SetParent(BattleContent);
            ResultText.text = $"����� ���带{PlayerShield} �����.";
            TextDown();
            TextDown();
        }

        yield return new WaitForSeconds(0.5f);

        for (int num = 0; num < EnemyAttackAmount; num++)
        {
            DecreaseDamage = Min(EnemyDamage, PSO._DEF);
            DefenseDamage = Min(PlayerShield, (EnemyDamage - PSO._DEF));
            DefenseDamage = Mathf.Clamp(DefenseDamage, 0, 1000);
            RealDamage = EnemyDamage - PSO._DEF - PlayerShield;
            PlayerShield -= (EnemyDamage - PSO._DEF);
            RealDamage = Mathf.Clamp(RealDamage, 0, 1000);

            var EnemyText1 = Instantiate(TextPrefab);
            EnemyText1.transform.SetParent(BattleContent);
            EnemyText1.text = $"���� {EnemySkillName}�� {RealDamage} ���ظ� �Ծ���.";
            TextDown();

            //yield return new WaitForSeconds(0.5f);

            var EnemyText2 = Instantiate(TextPrefab);
            EnemyText2.transform.SetParent(BattleContent);
            EnemyText2.text = $"({DecreaseDamage}���ҵ�) ({DefenseDamage}����)";
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
}
