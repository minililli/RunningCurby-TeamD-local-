using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Skill3 : PoolObject
{
    PlayerInputAction inputActions;
    Transform tran_Skill;
    Transform tran_SkillRange;
    Animator anim_Skill;
    Vector3 inputDir = Vector3.zero;
    Collider2D coll_Skill;

    /// <summary>
    /// 스킬 데미지 계산용 변수
    /// </summary>

    public float skillSpeed = 1.0f;

    public float skillCoolTimeMAx = 1.0f;
    private float skillCoolTime = 0;
    public float SkillCoolTime
    {
        get => skillCoolTime;        
        set
        {
            skillCoolTime = value;
            onSkillCoolTimeChange?.Invoke(skillCoolTime / skillCoolTimeMAx);
        }
    }
    public Action<float> onSkillCoolTimeChange;

    public int skillComboMax = 3;
    private int skillCombo;
    public int SkillCombo
    {
        get => skillCombo;        
        set
        {
            skillCombo = Mathf.Clamp(value, 0, skillComboMax);
            onSkillComboChange?.Invoke(skillCombo);
        }
    }
    public Action<int> onSkillComboChange;

    bool isOnSkill = false;

    /// <summary>
    /// 발사할 총알 프리팹
    /// </summary>
    public GameObject bullet;

    private bool isLeft = false;
    public bool IsLeft
    {
        get => isLeft;        
        set
        {
            isLeft = value;
        }
    }

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        anim_Skill = GetComponent<Animator>();
        tran_Skill = GetComponent<Transform>();
    }

    void Start()
    {
        SkillCombo = skillComboMax;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Attack3.performed += OnSkill3;
    }

    protected override void OnDisable()
    {
        inputActions.Player.Attack3.performed -= OnSkill3;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();

        base.OnDisable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        inputDir = dir;

        if (dir.x > 0)                                            // 마지막 이동 위치 확인용 
        {
            isLeft = false;
        }
        if (dir.x < 0)
        {
            isLeft = true;
        }


    }

    public void OnSkill3(InputAction.CallbackContext context)   // 키보드 A키
    {
        if (!isOnSkill && SkillCombo != 0)
        {
            StartCoroutine(IEOnSkill());
        }
    }

    private void Update()
    {
        if (SkillCombo == 0)
        {
            SkillCoolTime += Time.deltaTime;
        }
    }

    IEnumerator IEOnSkill()
    {
        SkillCombo--;
        isOnSkill = true;
        OnFire();
        if (SkillCombo == 0)
        {
            yield return new WaitForSeconds(skillCoolTimeMAx);
            SkillCombo = skillComboMax;
            SkillCoolTime = 0;
            StopCoroutine(IEOnSkill());
        }
        isOnSkill = false;

    }

    /// <summary>
    /// 총알을 한발 발사하는 함수.
    /// </summary>
    protected virtual void OnFire()
    {
        GameObject obj = Factory.Inst.GetObject(PoolObjectType.Bullet); //풀에서 Bullet빼서쓰는걸로 변경함
        float posX = tran_Skill.position.x;
        float posY = tran_Skill.position.y;
        if (isLeft)
        {
            obj.transform.position = new Vector2(posX - 1, posY);
        }
        else
        {
            obj.transform.position = new Vector2(posX + 1, posY);
        }
    }


}