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
    
    bool isFire = false;

    /// <summary>
    /// 발사할 총알 프리팹
    /// </summary>
    public GameObject bullet;

    /// <summary>
    /// 총알 발사 시간 간격
    /// </summary>
    public float skillInterval;

    /// <summary>
    /// 총알을 계속 발사하는 코루틴
    /// </summary>
    IEnumerator skillCoroutine;

    public bool isLeft = false;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        anim_Skill = GetComponent<Animator>();
        tran_Skill = GetComponent<Transform>();
        skillCoroutine = SkillCoroutine();
    }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Attack3.performed += OnSkill3;
        inputActions.Player.Attack3.canceled += OffSkill3;
    }

    protected override void OnDisable()
    {
        inputActions.Player.Attack3.canceled -= OffSkill3;
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
        GameObject obj = Factory.Inst.GetObject(PoolObjectType.Bullet); //풀에서 Bullet빼서쓰는걸로 변경함
        float posX = 0.0f;//transSkill.position.x;
        float posY = 0.0f; //transSkill.position.y;
        if(isLeft)
        {
            obj.transform.position = new Vector2(posX - 1, posY);
        }
        else
        {
            obj.transform.position = new Vector2(posX + 1, posY);
        }
        
    }

    /// <summary>
    /// 주기적으로 총알을 발사하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator SkillCoroutine()
    {
        while (true)
        {
            if(!isFire)
            {
                OnFire();
                isFire = true;
            }
            
            yield return new WaitForSeconds(skillInterval);
            isFire = false;
        }
    }

    /// <summary>
    /// 총알을 한발 발사하는 함수.
    /// </summary>
    protected virtual void OnFire()
    {
        GameObject obj = Instantiate(bullet);
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

    //public void OnSkill3(InputAction.CallbackContext context)   // 키보드 D
    //{
    //    StartCoroutine(skillCoroutine);        
    //}

    public void OffSkill3(InputAction.CallbackContext context)   // 키보드 D
    {
        StopCoroutine(skillCoroutine);     
    }
}