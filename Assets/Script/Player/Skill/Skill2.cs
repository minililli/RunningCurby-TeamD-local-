using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Skill2 : MonoBehaviour
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
    public float skillpoint = 1.0f;
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

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        anim_Skill = GetComponent<Animator>();
        tran_Skill = GetComponent<Transform>();
        tran_SkillRange = tran_Skill.GetChild(0);
        coll_Skill = tran_SkillRange.GetComponent<Collider2D>();
    }

    private void Start()
    {
        anim_Skill.SetFloat("SkillSpeed", skillSpeed);
        SkillCombo = skillComboMax;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Attack2.performed += OnSkill2;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack2.performed -= OnSkill2;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        inputDir = dir;                                                         // 마지막 이동 위치 확인용 

        if (dir.x > 0)
        {
            anim_Skill.SetBool("isLeft", false);
        }
        else if (dir.x < 0)
        {
            anim_Skill.SetBool("isLeft", true);
        }
    }

    public void OnSkill2(InputAction.CallbackContext context)                   // 키보드 S키
    {
        if (!isOnSkill && skillCoolTime == 0)
        {
            StartCoroutine(IEOnSkill());
        }
    }

    private void Update()
    {
        
    }

    public void SkillComboDown()
    {
        isOnSkill = false;
    }

    IEnumerator IEOnSkill()
    {
        if(skillCombo > 0)
        {
            isOnSkill = true;
            SkillCombo--;
            anim_Skill.SetTrigger("attack");
        }

        if(skillCombo == 0)
        {
            while(skillCoolTime < skillCoolTimeMAx)
            {
                SkillCoolTime += Time.deltaTime;
                yield return null;
            }
            SkillCombo = skillComboMax;
            SkillCoolTime = 0;
            StopCoroutine(IEOnSkill());
        }
    }
}