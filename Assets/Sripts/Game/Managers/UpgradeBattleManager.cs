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

    //머리 상태이상
    bool isFaint = false;
    bool isBlind = false;

    //몸통 상태이상
    bool isBleeding = false;
    int BleedingTurn = 0;

    //팔 상태이상
    bool isWristCut = false;
    bool isArmCut = false;

    //다리 상태이상
    bool isLegCut = false;

    Coroutine _coroutine;
    public void BattleSet(int StageNum)
    {
        NowStage = StageNum;
        //적과 플레이어의 이름 바꿔주기
        PlayerNameText.text = PSO.Name;
        EnemyNameText.text = EnemySOs[NowStage].Name;
        EnemySOs[NowStage].FirstInit();
        //적의 HP와 플레이어의 HP표시
        UpdateHP();
        SetSkills();
        SetEnemySkills();
        //BattleStart();
        StartCoroutine(BattleStart());
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

    /// <summary>
    /// 플레이어의 스킬들을 셋팅 해주는 함수
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
    /// 적의 행동을 셋팅 해주는 함수
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
    /// 상대가 공격하기전, 그 공격에 대한 정보와, 데미지 등을 기록, 인게임에 표기해주는 함수
    /// </summary>
    void EnemyAttackReady()
    {
        isTurnStart = false;

        if(isBleeding)
        {
            var Text = Instantiate(TextPrefab);
            Text.transform.SetParent(BattleContent);
            Text.text = $"{EnemySOs[NowStage].Name}는 출혈 상태이상에 걸려있다.";
            TextDown();
            StartCoroutine(TextDown_());
            Texts.Add(Text.transform);

            int ENEMY_HP_5PER = (int)((float)EnemySOs[NowStage]._CurrentMaxHP * 5f / 100f);

            var Text_ = Instantiate(TextPrefab);
            Text_.transform.SetParent(BattleContent);
            Text_.text = $"{EnemySOs[NowStage].Name}에게 {ENEMY_HP_5PER}의 출혈 데미지!";
            TextDown();
            StartCoroutine(TextDown_());
            Texts.Add(Text_.transform);

            EnemyHit(EnemySOs[NowStage], ENEMY_HP_5PER);

            --BleedingTurn; //턴 감소

            if (BleedingTurn <= 0)
            {
                var _Text = Instantiate(TextPrefab);
                _Text.transform.SetParent(BattleContent);
                _Text.text = $"{EnemySOs[NowStage].Name}는 출혈 상태이상에서 벗어났다.";
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
            Text.text = $"{EnemySOs[NowStage].Name}는 기절해 움직이지 못한다.";
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
            Text.text = $"{EnemySOs[NowStage].Name}는 실명에 걸려있다.";
            TextDown();
            Texts.Add(Text.transform);
        }

        //적 전투 준비 텍스트 생성
        int SkillNum = Random.Range(0, 4);

        //여기는 전이랑 똑같게
        var AttackText = Instantiate(TextPrefab);
        AttackText.transform.SetParent(BattleContent);
        AttackText.text = EnemySkills[SkillNum].SkillSummary;
        TextDown();
        Texts.Add(AttackText.transform);

        //데미지 기록
        EnemyDamage = 0;
        EnemyDamage = (EnemySkills[SkillNum].SkillDamagePercent * EnemySOs[NowStage]._CurrentATK) / 100;
        //공격 이름 기록 //사용처 : 적이 EnemySkillName 공격을 사용했다.
        EnemySkillName = EnemySkills[SkillNum].SkillName;
        //공격 횟수 기록
        EnemyAttackAmount = EnemySkills[SkillNum].SkillAttackAcount;

        isPlayerAction = true;

        //지금까지 잠궈놨던 플레이어 스킬들을 풀어주는 역할
        for (int num = 0; num < PlayerSkills.Count; num++)
        {
            Player_Skill_Button[num].interactable = true;
        }

        StartCoroutine(TextDown_());
    }

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

        BattleInit();

        //행동할 코드 선택
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
    /// 플레이어가 공격을 사용했을때 발동해주는 함수
    /// </summary>
    void Attack(int SkillNum) //여기다가 부위공격을 넣어줘야할듯?
    {
        //기본 데미지 저장
        PlayerDamage = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentATK) / 100;

        //부위공격을 실행하는 함수
        PartAttack();
    }

    /// <summary>
    /// 플레이어가 쉴드를 사용했을때 발동해주는 함수
    /// </summary>
    /// <param name="SkillNum"></param>
    void Shield(int SkillNum)
    {
        //쉴드량 저장
        PlayerShield = (PlayerSkills[SkillNum].SkillDamagePercent * PSO._CurrentDEF) / 100;
        ActionString.Add($"당신은 {PlayerSkills[SkillNum].SkillName}를 사용했다.");
        Actions.Add(Null);
        ActionString.Add($"쉴드를 {PlayerShield} 얻었다.");
        Actions.Add(Null);
    }

    /// <summary>
    /// 텍스트를 내려주는 함수
    /// </summary>
    void TextDown()
    {
        BattleContent.position = new Vector3(BattleContent.position.x, BattleContent.position.y + 70f, BattleContent.position.z);
    }

    /// <summary>
    /// 텍스트를 내려주는 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator TextDown_()
    {
        yield return new WaitForSeconds(0.5f);
        TextDown();
    }

    /// <summary>
    /// 크리티컬을 판정해주는 함수
    /// </summary>
    /// <param name="CRI_PER"></param>
    /// <returns></returns>
    bool IsCritical(int CRI_PER)
    {
        int Rnum = Random.Range(0, 100);
        return Rnum < CRI_PER ? true : false;
    }

    /// <summary>
    /// 공격 미스를 판정해주는 함수 true일때 맞은거
    /// </summary>
    /// <param name="HIT_RATE"></param>
    /// <returns></returns>
    bool IsMiss(int HIT_RATE)
    {
        int Rnum = Random.Range(0, 100);
        return Rnum < HIT_RATE ? true : false;
    }

    /// <summary>
    /// 부위공격 실행해주는 함수
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
    /// 머리를 공격했을때 실행해주는 함수
    /// </summary>
    void HeadAttack() //공격했을때 공격에 성공하면 치명타확률 2배
    {
        int Hit_Rate = (PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER) / 2;
        ActionString.Add("당신은 머리를 공격했다!");
        Actions.Add(Null);

        if (IsMiss(Hit_Rate))
        {
            if(IsCritical(PSO._CurrentCRI_PER * 2)) //크리티컬
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp;
                IsCriticaled = true;
                ActionString.Add($"치명타! {EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다."); //텍스트 저장
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //이벤트 저장
            }
            else //빗나가지 않았으나 크리티컬은 아님
            {
                ActionString.Add($"{EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
            }
            HeadAttackEvents();
        }
        else
        {
            ActionString.Add("빗나갔다...");
            Actions.Add(Null);
        }
    }

    void HeadAttackEvents()
    {
        //즉사 : 1
        //실명 : 5
        //기절 : 15

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //이거 1 101

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
    /// 기절 상태이상
    /// </summary>
    void Faint()
    {
        if (isFaint) return; //이미 실명이면 더 안걸리게

        isFaint = true;
        ActionString.Add("적에게 기절 상태이상을 부여했다.");
        Actions.Add(Null);
    }

    /// <summary>
    /// 실명 상태이상
    /// </summary>
    void Blind()
    {
        isBlind = true;
        ActionString.Add("적에게 실명 상태이상을 부여했다.");
        Actions.Add(Null);
        ActionString.Add("적의 공격적중률이 80% 감소했다.");
        Actions.Add(Null);
        Stat stat = new Stat();
        stat.HIT_RATE = -80;
        EnemySOs[NowStage].AddStats(stat);
    }

    /// <summary>
    /// 즉사 상태이상
    /// </summary>
    void Instant_Dead()
    {
        ActionString.Remove(ActionString[ActionString.Count - 1]);
        ActionString.Add($"즉사! {EnemySOs[NowStage].Name}에게 {EnemySOs[NowStage]._CurrentHP}데미지를 주었다.");
        Actions.Add(() => EnemyHit(EnemySOs[NowStage], EnemySOs[NowStage]._CurrentHP));
    }

    /// <summary>
    /// 몸통을 공격했을때 실행해주는 함수
    /// </summary>
    void BodyAttack()
    {
        int Hit_Rate = (PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER);
        ActionString.Add("당신은 몸통을 공격했다!");
        Actions.Add(Null);

        if(IsMiss(Hit_Rate))
        {
            if (IsCritical(PSO._CurrentCRI_PER)) //크리티컬
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp - EnemySOs[NowStage]._CurrentDEF;
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                IsCriticaled = true;
                ActionString.Add($"치명타! {EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다."); //텍스트 저장
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //이벤트 저장
            }
            else //빗나가지 않았으나 크리티컬은 아님
            {
                PlayerDamage -= EnemySOs[NowStage]._CurrentDEF;
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);
                ActionString.Add($"{EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
                IsCriticaled = false;
            }
            BodyAttackEvents();
        }
        else
        {
            ActionString.Add("빗나갔다...");
            Actions.Add(Null);
        }
    }

    /// <summary>
    /// 몸통 공격 이벤트들 구분하는 곳
    /// </summary>
    void BodyAttackEvents()
    {
        //출혈 20

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //이거 1 101

        UnityAction Action = Rnum switch
        {
            > 20 => Null,
            <= 20 => Bleeding,
        };

        Action();
    }

    /// <summary>
    /// 출혈 이벤트(몸통 공격)
    /// </summary>
    void Bleeding()
    {
        if (isBleeding) return; //이미 실명이면 더 안걸리게

        isBleeding = true;
        BleedingTurn = 5;
        ActionString.Add("적에게 출혈 상태이상을 부여했다.");
        Actions.Add(Null);
    }

    /// <summary>
    /// 팔을 공격했을때 실행해주는 함수
    /// </summary>
    void ArmAttack()
    {
        int Hit_Rate = (int)((float)(PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER) * 8f / 10f);
        ActionString.Add("당신은 팔을 공격했다!");
        Actions.Add(Null);

        if (IsMiss(Hit_Rate))
        {
            if (IsCritical(PSO._CurrentCRI_PER)) //크리티컬
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp - (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 8f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                IsCriticaled = true;
                ActionString.Add($"치명타! {EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다."); //텍스트 저장
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //이벤트 저장
            }
            else //빗나가지 않았으나 크리티컬은 아님
            {
                PlayerDamage -= (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 8f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                ActionString.Add($"{EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
            }
            ArmAttackEvents();
        }
        else
        {
            ActionString.Add("빗나갔다...");
            Actions.Add(Null);
        }
    }

    void ArmAttackEvents()
    {
        //속목 절단 20
        //팔 절단 10

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //이거 1 101

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

        ActionString.Add("적의 손목을 잘라냈다!");
        Actions.Add(Null);
    }

    /// <summary>
    /// 팔 절단 이벤트(팔 공격)
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

        ActionString.Add("적의 팔을 잘라냈다!");
        Actions.Add(Null);
    }

    /// <summary>
    /// 다리를 공격했을때 실행해주는 함수
    /// </summary>
    void LegAttack()
    {
        int Hit_Rate = (int)((float)(PSO._CurrentHIT_RATE - EnemySOs[NowStage]._CurrentAVOID_PER) * 7f / 10f);
        ActionString.Add("당신은 다리를 공격했다!");
        Actions.Add(Null);

        if (IsMiss(Hit_Rate))
        {
            if (IsCritical(PSO._CurrentCRI_PER)) //크리티컬
            {
                float temp = 0;
                temp = (float)PlayerDamage + (float)PlayerDamage * (float)PSO._CurrentCRI_DMG / 100f;
                PlayerDamage = (int)temp - (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 7f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                IsCriticaled = true;
                ActionString.Add($"치명타! {EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다."); //텍스트 저장
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage)); //이벤트 저장
            }
            else //빗나가지 않았으나 크리티컬은 아님
            {
                PlayerDamage -= (int)((float)EnemySOs[NowStage]._CurrentDEF / 10f * 8f);
                PlayerDamage = Mathf.Clamp(PlayerDamage, 0, 10000);

                ActionString.Add($"{EnemySOs[NowStage].Name}에게 {PlayerDamage}데미지를 주었다.");
                Actions.Add(() => EnemyHit(EnemySOs[NowStage], PlayerDamage));
            }
            LegAttackEvents();
        }
        else
        {
            ActionString.Add("빗나갔다...");
            Actions.Add(Null);
        }
    }

    void LegAttackEvents()
    {
        //다리절단 10

        int Rnum = 0;
        Rnum = Random.Range(1, 101); //이거 1 101

        UnityAction Action = Rnum switch
        {
            > 10 => Null,
            <= 10 => LegCut,
        };

        Action();
    }

    void LegCut() //다리 절단했다고 효과
    {
        if (isLegCut) return;
        
        isLegCut = true;

        Stat stat = new Stat();
        stat.HIT_RATE -= 50;
        stat.AVOID_PER -= 50;

        EnemySOs[NowStage].AddStats(stat);

        ActionString.Add("적의 다리를 잘라냈다!");
        Actions.Add(Null);
    }

    /// <summary>
    /// 데미지들어가는걸 계산 해주고, 텍스트를 띄워주는 함수
    /// </summary>
    IEnumerator BattleCalculator()
    {
        PlayerDamageAdd();
        Debug.Log($"이벤트 갯수 : {Actions.Count}");
        Debug.Log($"텍스트 갯수 : {ActionString.Count}");

        for(int num = 0; num < ActionString.Count; num++)
        {
            //텍스트 출력
            var AttackText = Instantiate(TextPrefab);
            AttackText.transform.SetParent(BattleContent);
            AttackText.text = ActionString[num];
            TextDown();
            StartCoroutine(TextDown_());
            Texts.Add(AttackText.transform);

            //이벤트 실행
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

        ActionString.Add($"{EnemySOs[NowStage].Name}의 {EnemySkillName}!");
        Actions.Add(Null);

        if(!IsMiss(EnemySOs[NowStage]._CurrentHIT_RATE - PSO._AVOID_PER)) //미스 판정
        {
            ActionString.Add($"당신은 적의 공격을 피했다!");
            Actions.Add(Null);
            return;
        }
        
        for(int num = 0; num < EnemyAttackAmount; num++)
        {
            ActionString.Add($"{DAMAGE}의 피해를 입었다.");
            Actions.Add(() => PlayerHit(PSO, DAMAGE));
            ActionString.Add($"({DecreaseDamage})감소됨 / ({ DefenseDamage})방어함");
            Actions.Add(Null);
        }
    }

    /// <summary>
    /// 적에게 데미지를 주는 함수
    /// </summary>
    /// <param name="EnemySO"></param>
    /// <param name="Damage"></param>
    void EnemyHit(PlayerSO EnemySO, int Damage)
    {
        EnemySO.Hit(Damage);
        UpdateHP();
        //만약 적의 hp가 0이면
        if(EnemySO.IsDie())
        {
            StopCoroutine(_coroutine);
            StartCoroutine(EnemyDie());
        }
    }

    IEnumerator EnemyDie()
    {
        //효과 재생 시간
        //적이 부르르 떨면서 아래로 내려가면서 희미해지는 효과(2초)

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
    /// 플레이어한테 데미지를 주는 함수
    /// </summary>
    /// <param name="PlayerSO"></param>
    /// <param name="Damage"></param>
    void PlayerHit(PlayerSO PlayerSO, int Damage)
    {
        PlayerSO.Hit(Damage);
        UpdateHP();
        //만약 플레이어의 hp가 0이면
        if(PlayerSO.IsDie())
        {
            StopCoroutine(BattleCalculator());
            StartCoroutine(PlayerDie());
        }
    }

    IEnumerator PlayerDie()
    {
        Debug.Log("플레이어 뒤짐");
        //효과 재생 시간
        //플레이어가 부르르 떨면서 아래로 내려가면서 희미해지는 효과(2초)

        yield return new WaitForSeconds(2f);

        //게임오버 씬으로 보내주면 될듯?
    }

    /// <summary>
    /// 패널 켜주는 함수
    /// </summary>
    /// <param name="isActive"></param>
    public void PartAttackPannelSetActive(bool isActive)
    {
        PartAttackPannel.gameObject.SetActive(isActive);
        PartBackGroundPannel.gameObject.SetActive(isActive);
        //나중에 dotween으로 효과 넣어주기
    }

    /// <summary>
    /// 공격할 부분 정해주는 함수
    /// </summary>
    /// <param name="part"></param>
    public void SetAttackPart(PartKind part)
    {
        PART = part;
    }

    /// <summary>
    /// 작은수를 리턴해주는 함수
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
