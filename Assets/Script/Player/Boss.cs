using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class Boss : PoolObject
{

    Player player;
    Skill1 skill1;
    Skill2 skill2;
    Bullet bullet;
    Transform tran_Enemy;
    Rigidbody2D rigi_Enemy;
    Collider2D coll_Enemy;
    SpriteRenderer spri_Enemy;
    Animator anim_Enemy;
    Slider slider;

    Collider2D coll_Enemy_PlayerChecker;

    Transform tran_Target;
    //Rigidbody2D rigi_Target;
    //Collider2D coll_Target;
    GameObject obj;                                                             //collision.gameObject 줄여쓰기 위해 ..

    Vector2 dirVec;
    Vector2 nextVec;

    public GameObject monsterPrefab; // 소환할 몬스터 프리팹
    private float summonInterval = 3f; // 3초마다 몬스터 소환
    private Vector3 summonPosition; // 몬스터 소환 위치

    /// <summary>
    /// 보스죽을때 이펙트 프리팹
    /// </summary>
    public GameObject bossexplosionEffectPrefab;

    /// <summary>
    /// 보스최대hp
    /// </summary>    
    public float maxHealth = 100.0f;

    /// <summary>
    /// 보스 현재 HP
    /// </summary>
    float currentHealth;

    /// <summary>
    /// Enemy HP 델리게이트
    /// </summary>
    public Action<int> onChangeEnemyHP;

    /// <summary>
    /// 기본 공격력 
    /// </summary>
    public float attackPoint;

    /// <summary>
    /// 기본 방어력 
    /// </summary>
    public float defencePoint;

    /// <summary>
    /// 공격속도.  공격 애니매이션 증폭에 사용 
    /// </summary>
    public float attackSpeed;


    /// <summary>
    /// Enemy 사망시 player가 얻게 될 경험치
    /// </summary>
    public int exp;

    /// <summary>
    /// 맞고 있으면 true 아니면 falase
    /// </summary>
    public bool isHit = false;

    /// <summary>
    /// 살아 있으면 true 죽었으면 falase
    /// </summary>
    bool isLive = true;

    /// <summary>
    /// 공격중이면 true 아니면 falase
    /// </summary>
    bool isAttack = false;

    /// <summary>
    /// 데미지 계산용 player SkillPoint
    /// </summary>
    float pSkillPoint;

    /// <summary>
    /// enemy가 공격으로 넘어가기 위한 기준 거리 
    /// </summary>
    public float skill_Range = 2.0f;

    /// <summary>
    /// enemy hp 바(UI)
    /// </summary>
    public TMP_Text EnemyHpText;
    
    
    /// <summary>
    /// 스텟 초기화용
    /// </summary>
    protected virtual void InitStat()
    {
        maxHealth = 100;
        attackPoint = 10.0f;
        defencePoint = 10.0f;
        attackSpeed = 1.0f;
        exp = 10;
    }

    protected virtual void Awake()
    {
        tran_Enemy = GetComponent<Transform>();
        rigi_Enemy = GetComponent<Rigidbody2D>();
        coll_Enemy = GetComponent<Collider2D>();
        spri_Enemy = GetComponent<SpriteRenderer>();
        anim_Enemy = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        slider = FindObjectOfType<Canvas>().GetComponent<Slider>();
        coll_Enemy_PlayerChecker = GetComponentInChildren<CircleCollider2D>();
    }

    private void OnEnable()
    {
        //InitStat();                                                           //초기화
        isHit = false;
        isLive = true;
        currentHealth = maxHealth;
        tran_Target = null;
    }

    void Start()
    {
        InvokeRepeating("SummonMonster", 3.0f, summonInterval); // SummonMonster() 함수를 3초 대기 후 summonInterval마다 호출
        currentHealth = maxHealth;

        // Canvas에서 Slider 오브젝트를 찾습니다.
        slider = GameObject.FindObjectOfType<Slider>();
        currentHealth = maxHealth;
        slider.minValue = 0f;  // Slider의 minValue를 0으로 설정합니다.
        slider.maxValue = maxHealth;  // Slider의 maxValue를 maxHealth로 설정합니다.
        slider.value = maxHealth;  // Slider의 value를 maxHealth로 초기화합니다.
    }
    private void SummonMonster() // 몬스터를 생성할 위치 계산
    {
        // 몬스터 생성
        Instantiate(monsterPrefab, summonPosition, Quaternion.identity);
    }
    protected virtual void FixedUpdate()
    {
        
    }

    void Update()
    {

        slider.value = currentHealth;
        //slider.value = currentHealth/maxHealth;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)                                    //skill layer의 tirger와 충돌 시 
        {
            obj = collision.gameObject;
            if (collision.CompareTag("Skill1"))                                 //어떤 스킬인지 확인하여 
            {
                pSkillPoint = obj.GetComponentInParent<Skill1>().skillpoint;    // 해당 스킬의 skillpoint 를 받아옴 
            }
            else if (collision.CompareTag("Skill2"))
            {
                pSkillPoint = obj.GetComponentInParent<Skill2>().skillpoint;
            }
            else if (collision.CompareTag("Skill3"))
            {
                pSkillPoint = obj.GetComponent<Bullet>().skillpoint;
            }
            Hit_Enemy();                                                        //맞는 처리
        }
        else if (collision.gameObject.layer == 7)                               //player layer가 triger와 충돌 시  
        {
            obj = collision.gameObject;
            tran_Target = obj.transform;                                        //타겟으로 설정                                         
            anim_Enemy.SetBool("isWalk", true);                                 //걷는 에니메이션 참으로 변경
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isHit = false;
        if (collision.gameObject.layer == 7)                                    //player layer가 triger에 머물 시  
        {
            dirVec = tran_Target.position - tran_Enemy.position;                //타겟과의 거리를 구하여 
            if (dirVec.x < skill_Range && !isAttack)                            //기준 거리보다 가깝고 공격중이 아니면  
            {
                anim_Enemy.SetTrigger("Attack_Skill1");                         //공격 에니메이션 트리거 발동
                                                                                //추후 랜덤함수 로 스킬 랜덤하게 나가게 구현
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isHit = false;
        if (collision.gameObject.layer == 7)                                    //player layer가 triger에서 나가면   
        {
            tran_Target = null;                                                 //타겟을 null로 변경
            anim_Enemy.SetBool("isWalk", false);                                //걷는 애니메이션 거짓으로 변경 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)                                     //skill layer의 collision 과 충돌 시 
        {
            obj = collision.gameObject;                                         //triger와 동일한 구조 
            if (obj.CompareTag("Skill1"))
            {
                pSkillPoint = obj.GetComponentInParent<Skill1>().skillpoint;
            }
            else if (obj.CompareTag("Skill2"))
            {
                pSkillPoint = obj.GetComponentInParent<Skill2>().skillpoint;
            }
            else if (obj.CompareTag("Skill3"))
            {
                pSkillPoint = obj.GetComponent<Bullet>().skillpoint;
            }
            Hit_Enemy();
        }
    }

    /// <summary>
    /// enemy 가 target에게 데미지 주는 함수
    /// </summary>
    /// <returns></returns>
    public float Attack_Enemy()
    {
        return attackPoint;
    }

    /// <summary>
    /// 맞았을떄 데미지 및 애니메이션 처리 함수 
    /// </summary>
    private void Hit_Enemy()
    {
        isHit = true;
        float pAttackPoint = player.attackPoint;                                //플레이어 공격점수 가져오기 
        float skillPower = pSkillPoint * pAttackPoint;                          //맞은 스킬의 기술점수 * 공격점수 = 기술 힘 
        float damage = skillPower - (defencePoint * 0.3f);                      //데미지 = 기술힘 - 방어점수의30%

        currentHealth -= (damage > 0) ? damage : 1.0f;                             //데미지 최소값 확보

        if (isLive)                                                             //죽었는데 계속 때리면 맞는 경우가 발생하여 
        {
           // anim_Enemy.SetTrigger("isHit");                                     //맞는 에니메이션 트리거 발동

            if (currentHealth <= 0)
            {
                //anim_Enemy.SetTrigger("isDie");                                 //죽는 에니메이션 트리거 발동 
                //Debug.Log("죽음");
                Die_Enemy();
            }
        }
        Debug.Log($"{currentHealth}");                                              //UI 붙으면 삭제 
    }

    /// <summary>
    /// 죽었을 떄 오브젝트 풀로 되돌리고 경험치 및 점수 주는 함수 
    /// </summary>
    void Die_Enemy()
    {
        isLive = false;
        gameObject.SetActive(false);                                            // Enemy 비활성화

        // 보스가 죽으면 이펙트를 생성합니다.
        GameObject explosionEffect1 = Instantiate(bossexplosionEffectPrefab,
                transform.position + Random.insideUnitSphere * 3f, Quaternion.identity);
        Destroy(explosionEffect1, 2f);
        InvokeRepeating("CreateExplosionEffect", 0.5f, 0.5f);  // 0.5초마다 호출하여 이펙트를 생성합니다

        //player.AddExp((int)exp);                                              // player에 exp 추가
    }
    void CreateExplosionEffect()  // 이펙트 호출 되는 함수
    {
        GameObject explosionEffect = Instantiate(bossexplosionEffectPrefab, 
            transform.position + Random.insideUnitSphere * 3f, Quaternion.identity);
        Destroy(explosionEffect, 2f);
    }
    public float GetEnemyHP()
    {
        return currentHealth;
    }
    


}