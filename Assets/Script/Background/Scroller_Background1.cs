using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller_Background1 : MonoBehaviour
{
    bool isStart = false;
    UI_GameCounter gameCounter;    

    public float scrollingSpeed = 2.5f;  // 스크롤 이동 속도
    Transform[] bgSlots = null;  // 배경 이미지가 두개 붙어있는 슬롯의 집합
    float slot_Width = 19.68f;  // 이미지 한변의 길이

    protected virtual void Awake()
    {   

        bgSlots = new Transform[transform.childCount];  // 슬롯이 들어갈 배열 생성
        for (int i=0;i<transform.childCount;i++)        
        {
            bgSlots[i] = transform.GetChild(i);         // 슬롯 하나씩 찾기
        }
        slot_Width = bgSlots[1].position.x - bgSlots[0].position.x;   // 이미지 한변의 길이 계산
    }

    private void Start()
    {
        gameCounter = FindObjectOfType<UI_GameCounter>();
        gameCounter.StartRun += OnStart;
    }

    void OnStart()
    {
       isStart = true;
    }


    private void Update()
    {
        if (isStart)             // 게임카운트 후에
        {
            for (int i = 0; i < bgSlots.Length; i++)  // 아래 foreach와 같은 코드. 하지만 foreach가 더 빠르다.
            {
                Transform slot = bgSlots[i];
                slot.Translate(Time.deltaTime * scrollingSpeed * -transform.right);
                if (slot.localPosition.x < 0)      // 슬롯이 부모 위치보다 왼쪽으로 갔을 때
                {

                    MoveRightEnd(i);  // 슬롯을 오른쪽 끝으로 이동 시키기
                }
            }
        }
       
    }

    protected virtual void MoveRightEnd(int index)
    {
        bgSlots[index].Translate(slot_Width * bgSlots.Length * transform.right);
    }
}
