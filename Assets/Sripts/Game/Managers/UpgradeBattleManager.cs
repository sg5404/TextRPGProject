using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UpgradeBattleManager : MonoSingleton<UpgradeBattleManager>
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

    private List<string> ActionString = new List<string>();
    private List<UnityAction> Actions = new List<UnityAction>();

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

    [SerializeField] Transform PartAttackPannel;
    [SerializeField] Transform PartBackGroundPannel;
    [SerializeField] private PartKind PART;

    private List<Transform> Texts = new List<Transform>();

    //�Ӹ� �����̻�
    bool isFaint = false;
    bool isBlind = false;

    //���� �����̻�
    bool isBleeding = false;
    int BleedingTurn = 0;

    //�� �����̻�
    bool isWristCut = false;
    bool isArmCut = false;

    //�ٸ� �����̻�
    bool isLegCut = false;

    Coroutine _coroutine;
    public void BattleSet(int StageNum)
    {
        NowStage = StageNum;
        //���� �÷��̾��� �̸� �ٲ��ֱ�
        PlayerNameText.text = PSO.Name;
        EnemyNameText.text = EnemySOs[NowStage].Name;
        EnemySOs[NowStage].FirstInit();
        //���� HP�� �÷��̾��� HPǥ��
        UpdateHP();
        SetSkills();
        SetEnemySkills();
        //BattleStart();
        StartCoroutine(BattleStart());
    }

    /// <summary>
    /// ����� �÷��̾�� ���� hp�� ������Ʈ ���ִ� �Լ�
    /// </summary>
    void UpdateHP()
    {
        Player_HP.fillAmount = (float)PSO._CurrentHP / (float)PSO._CurrentMaxHP;
        Player_HP_Text.text = $"{PSO._CurrentHP}/{PSO._CurrentMaxHP}";

        Enemy_HP.fillAmount = (float)EnemySOs[NowStage]._CurrentHP / (float)EnemySOs[NowStage]._CurrentMaxHP;
        Enemy_HP_Text.text = $"{EnemySOs[NowStage]._CurrentHP}/{EnemySOs[NowStage]._CurrentMaxHP}";
    }

    /// <summary>
    /// �÷��̾��� ��ų���� ���� ���ִ� �Լ�
    /// </summary>
    void SetSkills()
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

    /// <summary>
    /// ���� �ൿ�� ���� ���ִ� �Լ�
    /// </summary>
    void SetEnemySkills()
    {
        EnemySkills.Clear();

        for (int num = 0; num < EnemySOs[NowStage].Skills.Count; num++)
            EnemySkills.Add(EnemySOs[NowStage].Skills[num]);
    }

    IEnumerator BattleStart()
    {
        isTurnStart = true;
        isPlayerAction = true;

        while (true)
        {
            if (isTurnStart) EnemyAttackReady();

            yield return new WaitForSeconds(0.1f);

            if (isPlayerAction) continue;

            _coroutine = StartCoroutine(BattleCalculator());
            yield return new WaitForSeconds(6f);
        }
    }

    /// <summary>
    /// ��밡 �����ϱ���, �� ���ݿ� ���� ������, ������ ���� ���, �ΰ��ӿ� ǥ�����ִ� �Լ�
    /// </summary>
    void EnemyAttackReady()
    {
        isTurnStart = false;

        if(isBleeding)
        {
            var Text = Instantiate(TextPrefab);
            Text.transform.SetParent(BattleContent);
            Text.text = $"{EnemySOs[NowStage].Name}�� ���� �����̻� �ɷ��ִ�.";
            TextDown();
            StartCoroutine(TextDown_());
            Texts.Add(Text.transform);

            int ENEMY_HP_5PER = (int)((float)EnemySOs[NowStage]._CurrentMaxHP * 5f / 100f);

            var Text_ = Instantiate(TextPrefab);
            Text_.transform.SetParent(BattleContent);
            Text_.text = $"{EnemySOs[NowStage].Name}���� {ENEMY_HP_5PER}�� ���� ������!";
            TextDown();
            StartCoroutine(TextDown_());
            Texts.Add(Text_.transform);

            EnemyHit(EnemySOs[NowStage], ENEMY_HP_5PER);

            --BleedingTurn; //�� ����

            if (BleedingTurn <= 0)
            {
                var _Text = Instantiate(TextPrefab);
                _Text.transform.SetParent(BattleContent);
                _Text.text = $"{EnemySOs[NowStage].Name}�� ���� �����̻󿡼� �����.";
                TextDown();
                StartCoroutine(TextDown_());
                Texts.Add(_Text.transform);
                isBleeding = false;
            }
        }

        if(isFaint)
        {
            var Text = Instantiate(TextPrefab);
            Text.transform.SetParent(BattleContent);
            Text.text = $"{EnemySOs[NowStage].Name}�� ������ �������� ���Ѵ�.";
            TextDown();
            Texts.Add(Text.transform);

            for (int num = 0; num < PlayerSkills.Count; num++)
            {
                Player_Skill_Button[num].interactable = true;
            }

            isPlayerAction = true;
            isFaint = false;
            return;
        }

        if(isBlind)
        {
            var Text = Instantiate(TextPrefab);
            Text.transform.SetParent(BattleContent);
            Text.text = $"{EnemySOs[NowStage].Name}�� �Ǹ� �ɷ��ִ�.";
            TextDown();
            Texts.Add(Text.transform);
        }

        //�� ���� �غ� �ؽ�Ʈ ����
        int SkillNum = Random.Range(0, 4);

        //����� ���̶� �Ȱ���
        var AttackText = Instantiate(TextPrefab);
        AttackText.transform.SetParent(BattleContent);
        AttackText.text = EnemySkills[SkillNum].SkillSummary;
        TextDown();
        Texts.Add(AttackText.transform);

        //������ ���
        EnemyDamage = 0;
        EnemyDamage = (EnemySkills[SkillNum].SkillDamagePercent * EnemySOs[NowStage]._CurrentATK) / 100;
        //���� �̸� ��� //���ó : ���� EnemySkillName ������ ����ߴ�.
        EnemySkillName = EnemySkills[SkillNum].SkillName;
        //���� Ƚ�� ���
        EnemyAttackAmount = EnemySkills[SkillNum].SkillAttackAcount;

        isPlayerAction = true;

        //���ݱ��� ��ų��� �÷��̾� ��ų���� Ǯ���ִ� ����
        for (int num = 0; num < PlayerSkills.Count; num++)
        {
            Player_Skill_Button[num].interactable = true;
        }

        StartCoroutine(TextDown_());
    }

    /// <summary>
    /// �÷��̾ �����ϴ� �ൿ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="SkillNum"></param>
    public void PlayerAction(int SkillNum)
    {
        for (int num = 0; num < Player_Skill_Button.Count; num++)
        {
            Player_Skill_Button[num].interactable = false;
        }

        BattleInit();

        //�ൿ�� �ڵ� ����
        UnityAction Action = PlayerSkills[SkillNum].S_Type switch
        {
            SkillType.Attack => ()=> Attack(SkillNum),
            SkillType.Shield => ()=> Shield(SkillNum),
            _ => () =>  Attack(SkillNum),
        };

        Action();

        isPlayerAction = false;
    }

    void BattleInit()
    {
        PlayerDamage = 0;
        PlayerShield = 0;
    }

    /// <summary>
    /// �÷��̾ ������ ��������� �ߵ����ִ� �Լ�
    /// </summary>
    void Attack(int SkillNum) //����ٰ� ���������� �־�����ҵ�?
    {
        //�⺻ ������ ����
        PlayerDamage = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentATK) / 100;

        //���������� �����ϴ� �Լ�
        PartAttack();
    }

    /// <summary>
    /// �÷��̾ ���带 ��������� �ߵ����ִ� �Լ�
    /// </summary>
    /// <param name="SkillNum"></param>
    void Shield(int SkillNum)
    {
        //���差 ����
        PlayerShield = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentDEF) / 100;
        ActionString.Add($"����� {PlayerSkills[SkillNum].SkillName}�� ����ߴ�.");
        Actions.Add(Null);
        ActionString.Add($"���带 {PlayerShield} �����.");
        Actions.Add(Null);
    }

    /// <summary>
    /// �ؽ�Ʈ�� �����ִ� �Լ�
    /// </summary>
    void TextDown()
    {
        BattleContent.position = new Vector3(BattleContent.position.x, BattleContent.position.y + 70f, BattleContent.position.z);
    }

    /// <summary>
    /// �ؽ�Ʈ�� �����ִ� �Լ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator TextDown_()
    {
        yield return new WaitForSeconds(0.5f);
        TextDown();
    }

    /// <summary>
    /// ũ��Ƽ���� �������ִ� �Լ�
    /// </summary>
    /// <param name="CRI_PER"></param>
    /// <returns></returns>
    bool IsCritical(int CRI_PER)
    {
        int Rnum = Random.Range(0, 100);
        return Rnum < CRI_PER ? true : false;
    }

    /// <summary>
    /// ���� �̽��� �������ִ� �Լ� true�϶� ������
    /// </summary>
    /// <param name="HIT_RATE"></param>
    /// <returns></returns>
    bool IsMiss(int HIT_RATE)
    {
        int Rnum = Random.Range(0, 100);
        return Rnum < HIT_RATE ? true : false;
    }

    /// <summary>
    /// �������� �������ִ� �Լ�
    /// </summary>
    void PartAttack()
    {
        UnityAction Action = PART switch
        {
            PartKind.HEAD => HeadAttack,
            PartKind.BODY => BodyAttack,
            PartKind.ARM => ArmAttack,
            PartKind.LEG => LegAttack,
            _ => BodyAttack,
        };

        Action();
    }

    /// <summary>
    /// �Ӹ��� ���������� �������ִ� �Լ�
    /// </summary>
    void HeadAttack() //���������� ���ݿ� �����ϸ� ġ��ŸȮ�� 2��
    {
        int Hit_Rate = (PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER) / 2;
        ActionString.Add("����� �Ӹ��� �����ߴ�!");
        Actions.Add(Null);

        if (IsMiss(Hit_Rate))
        {
            if(IsCritical(PSO._CurrentCRI_PER * 2)) //ũ��Ƽ��
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp;
                IsCriticaled = true;
                ActionString.Add($"ġ��Ÿ! {EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���."); //�ؽ�Ʈ ����
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //�̺�Ʈ ����
            }
            else //�������� �ʾ����� ũ��Ƽ���� �ƴ�
            {
                ActionString.Add($"{EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
            }
            HeadAttackEvents();
        }
        else
        {
            ActionString.Add("��������...");
            Actions.Add(Null);
        }
    }

    void HeadAttackEvents()
    {
        //��� : 1
        //�Ǹ� : 5
        //���� : 15

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //�̰� 1 101

        UnityAction Action = Rnum switch
        {
            > 21 => Null,
            > 6 and <= 21 => Faint,
            > 1 and <= 6 => Blind,
            <= 1 => Instant_Dead,
        };

        Action();
    }

    /// <summary>
    /// ���� �����̻�
    /// </summary>
    void Faint()
    {
        if (isFaint) return; //�̹� �Ǹ��̸� �� �Ȱɸ���

        isFaint = true;
        ActionString.Add("������ ���� �����̻��� �ο��ߴ�.");
        Actions.Add(Null);
    }

    /// <summary>
    /// �Ǹ� �����̻�
    /// </summary>
    void Blind()
    {
        isBlind = true;
        ActionString.Add("������ �Ǹ� �����̻��� �ο��ߴ�.");
        Actions.Add(Null);
        ActionString.Add("���� �������߷��� 80% �����ߴ�.");
        Actions.Add(Null);
        Stat stat = new Stat();
        stat.HIT_RATE = -80;
        EnemySOs[NowStage].AddStats(stat);
    }

    /// <summary>
    /// ��� �����̻�
    /// </summary>
    void Instant_Dead()
    {
        ActionString.Remove(ActionString[ActionString.Count - 1]);
        ActionString.Add($"���! {EnemySOs[NowStage].Name}���� {EnemySOs[NowStage]._CurrentHP}�������� �־���.");
        Actions.Add(() => EnemyHit(EnemySOs[NowStage], EnemySOs[NowStage]._CurrentHP));
    }

    /// <summary>
    /// ������ ���������� �������ִ� �Լ�
    /// </summary>
    void BodyAttack()
    {
        int Hit_Rate = (PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER);
        ActionString.Add("����� ������ �����ߴ�!");
        Actions.Add(Null);

        if(IsMiss(Hit_Rate))
        {
            if (IsCritical(PSO._CurrentCRI_PER)) //ũ��Ƽ��
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp - EnemySOs[NowStage]._CurrentDEF;
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                IsCriticaled = true;
                ActionString.Add($"ġ��Ÿ! {EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���."); //�ؽ�Ʈ ����
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //�̺�Ʈ ����
            }
            else //�������� �ʾ����� ũ��Ƽ���� �ƴ�
            {
                PlayerDamage -= EnemySOs[NowStage]._CurrentDEF;
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);
                ActionString.Add($"{EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
                IsCriticaled = false;
            }
            BodyAttackEvents();
        }
        else
        {
            ActionString.Add("��������...");
            Actions.Add(Null);
        }
    }

    /// <summary>
    /// ���� ���� �̺�Ʈ�� �����ϴ� ��
    /// </summary>
    void BodyAttackEvents()
    {
        //���� 20

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //�̰� 1 101

        UnityAction Action = Rnum switch
        {
            > 20 => Null,
            <= 20 => Bleeding,
        };

        Action();
    }

    /// <summary>
    /// ���� �̺�Ʈ(���� ����)
    /// </summary>
    void Bleeding()
    {
        if (isBleeding) return; //�̹� �Ǹ��̸� �� �Ȱɸ���

        isBleeding = true;
        BleedingTurn = 5;
        ActionString.Add("������ ���� �����̻��� �ο��ߴ�.");
        Actions.Add(Null);
    }

    /// <summary>
    /// ���� ���������� �������ִ� �Լ�
    /// </summary>
    void ArmAttack()
    {
        int Hit_Rate = (int)((float)(PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER) * 8f / 10f);
        ActionString.Add("����� ���� �����ߴ�!");
        Actions.Add(Null);

        if (IsMiss(Hit_Rate))
        {
            if (IsCritical(PSO._CurrentCRI_PER)) //ũ��Ƽ��
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp - (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 8f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                IsCriticaled = true;
                ActionString.Add($"ġ��Ÿ! {EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���."); //�ؽ�Ʈ ����
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //�̺�Ʈ ����
            }
            else //�������� �ʾ����� ũ��Ƽ���� �ƴ�
            {
                PlayerDamage -= (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 8f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                ActionString.Add($"{EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
            }
            ArmAttackEvents();
        }
        else
        {
            ActionString.Add("��������...");
            Actions.Add(Null);
        }
    }

    void ArmAttackEvents()
    {
        //�Ӹ� ���� 20
        //�� ���� 10

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //�̰� 1 101

        UnityAction Action = Rnum switch
        {
            > 30 => Null,
            <= 30 and > 20 => ArmCut,
            <= 20 => WristCut,
        };

        Action();
    }

    void WristCut()
    {
        if (isWristCut || isArmCut) return;

        isWristCut = true;
        Stat stat = new Stat();
        stat.HIT_RATE = -50;
        EnemySOs[NowStage].AddStats(stat);

        ActionString.Add("���� �ո��� �߶�´�!");
        Actions.Add(Null);
    }

    /// <summary>
    /// �� ���� �̺�Ʈ(�� ����)
    /// </summary>
    void ArmCut()
    {
        if (isArmCut) return;

        isWristCut = false;
        isArmCut = true;

        Stat stat = new Stat();
        stat.ATK = EnemySOs[NowStage]._CurrentATK / 2;
        stat.CRI_PER = -50;

        EnemySOs[NowStage].AddStats(stat);

        ActionString.Add("���� ���� �߶�´�!");
        Actions.Add(Null);
    }

    /// <summary>
    /// �ٸ��� ���������� �������ִ� �Լ�
    /// </summary>
    void LegAttack()
    {
        int Hit_Rate = (int)((float)(PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER) * 7f / 10f);
        ActionString.Add("����� �ٸ��� �����ߴ�!");
        Actions.Add(Null);

        if (IsMiss(Hit_Rate))
        {
            if (IsCritical(PSO._CurrentCRI_PER)) //ũ��Ƽ��
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp - (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 7f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                IsCriticaled = true;
                ActionString.Add($"ġ��Ÿ! {EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���."); //�ؽ�Ʈ ����
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //�̺�Ʈ ����
            }
            else //�������� �ʾ����� ũ��Ƽ���� �ƴ�
            {
                PlayerDamage -= (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 8f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                ActionString.Add($"{EnemySOs[NowStage].Name}���� {PlayerDamage}�������� �־���.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
            }
            LegAttackEvents();
        }
        else
        {
            ActionString.Add("��������...");
            Actions.Add(Null);
        }
    }

    void LegAttackEvents()
    {
        //�ٸ����� 10

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //�̰� 1 101

        UnityAction Action = Rnum switch
        {
            > 10 => Null,
            <= 10 => LegCut,
        };

        Action();
    }

    void LegCut() //�ٸ� �����ߴٰ� ȿ��
    {
        if (isLegCut) return;
        
        isLegCut = true;

        Stat stat = new Stat();
        stat.HIT_RATE -= 50;
        stat.AVOID_PER -= 50;

        EnemySOs[NowStage].AddStats(stat);

        ActionString.Add("���� �ٸ��� �߶�´�!");
        Actions.Add(Null);
    }

    /// <summary>
    /// ���������°� ��� ���ְ�, �ؽ�Ʈ�� ����ִ� �Լ�
    /// </summary>
    IEnumerator BattleCalculator()
    {
        PlayerDamageAdd();
        Debug.Log($"�̺�Ʈ ���� : {Actions.Count}");
        Debug.Log($"�ؽ�Ʈ ���� : {ActionString.Count}");

        for(int num = 0; num < ActionString.Count; num++)
        {
            //�ؽ�Ʈ ���
            var AttackText = Instantiate(TextPrefab);
            AttackText.transform.SetParent(BattleContent);
            AttackText.text = ActionString[num];
            TextDown();
            StartCoroutine(TextDown_());
            Texts.Add(AttackText.transform);

            //�̺�Ʈ ����
            Actions[num]();
            yield return new WaitForSeconds(0.5f);
        }

        Actions.Clear();
        ActionString.Clear();
        isTurnStart = true;
    }

    void PlayerDamageAdd()
    {
        int DAMAGE = EnemyDamage - PSO._CurrentDEF - PlayerShield;
        Debug.Log(DAMAGE);
        DAMAGE = Mathf.Clamp(DAMAGE, 0, 10000);
        Debug.Log(DAMAGE);
        int DecreaseDamage = Min(PSO._CurrentDEF, EnemyDamage);
        int DefenseDamage = Min(PlayerShield, EnemyDamage - DecreaseDamage);

        ActionString.Add($"{EnemySOs[NowStage].Name}�� {EnemySkillName}!");
        Actions.Add(Null);

        if(!IsMiss(EnemySOs[NowStage]._CurrentHIT_RATE - PSO._AVOID_PER)) //�̽� ����
        {
            ActionString.Add($"����� ���� ������ ���ߴ�!");
            Actions.Add(Null);
            return;
        }
        
        for(int num = 0; num < EnemyAttackAmount; num++)
        {
            ActionString.Add($"{DAMAGE}�� ���ظ� �Ծ���.");
            Actions.Add(() => PlayerHit(PSO, DAMAGE));
            ActionString.Add($"({DecreaseDamage})���ҵ� / ({ DefenseDamage})�����");
            Actions.Add(Null);
        }
    }

    /// <summary>
    /// ������ �������� �ִ� �Լ�
    /// </summary>
    /// <param name="EnemySO"></param>
    /// <param name="Damage"></param>
    void EnemyHit(PlayerSO EnemySO, int Damage)
    {
        EnemySO.Hit(Damage);
        UpdateHP();
        //���� ���� hp�� 0�̸�
        if(EnemySO.IsDie())
        {
            StopCoroutine(_coroutine);
            StartCoroutine(EnemyDie());
        }
    }

    IEnumerator EnemyDie()
    {
        //ȿ�� ��� �ð�
        //���� �θ��� ���鼭 �Ʒ��� �������鼭 ��������� ȿ��(2��)

        yield return new WaitForSeconds(2f);

        UIManager.Instance.BattlePannelSetActive(false);
        TextClear();
        UIManager.Instance.RewardPannelSetActive(true);
    }

    void TextClear()
    {
        for(int num = 0; num < Texts.Count; num++)
        {
            Destroy(Texts[num].gameObject);
        }
        Texts.Clear();
    }

    /// <summary>
    /// �÷��̾����� �������� �ִ� �Լ�
    /// </summary>
    /// <param name="PlayerSO"></param>
    /// <param name="Damage"></param>
    void PlayerHit(PlayerSO PlayerSO, int Damage)
    {
        PlayerSO.Hit(Damage);
        UpdateHP();
        //���� �÷��̾��� hp�� 0�̸�
        if(PlayerSO.IsDie())
        {
            StopCoroutine(BattleCalculator());
            StartCoroutine(PlayerDie());
        }
    }

    IEnumerator PlayerDie()
    {
        Debug.Log("�÷��̾� ����");
        //ȿ�� ��� �ð�
        //�÷��̾ �θ��� ���鼭 �Ʒ��� �������鼭 ��������� ȿ��(2��)

        yield return new WaitForSeconds(2f);

        //���ӿ��� ������ �����ָ� �ɵ�?
    }

    /// <summary>
    /// �г� ���ִ� �Լ�
    /// </summary>
    /// <param name="isActive"></param>
    public void PartAttackPannelSetActive(bool isActive)
    {
        PartAttackPannel.gameObject.SetActive(isActive);
        PartBackGroundPannel.gameObject.SetActive(isActive);
        //���߿� dotween���� ȿ�� �־��ֱ�
    }

    /// <summary>
    /// ������ �κ� �����ִ� �Լ�
    /// </summary>
    /// <param name="part"></param>
    public void SetAttackPart(PartKind part)
    {
        PART = part;
    }

    /// <summary>
    /// �������� �������ִ� �Լ�
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    int Min(int num1, int num2)
    {
        return num1 > num2 ? num2 : num1;
    }

    void Null()
    {

    }
}

public enum PartKind
{
    HEAD,
    BODY,
    ARM,
    LEG,
}
