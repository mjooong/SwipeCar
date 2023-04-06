using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarController : MonoBehaviour
{
    float speed = 0;
    Vector2 startPos;

    GameDirector m_GDirector;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; //실행 프레임 속도 60프레임으로 고정 시키기.. 코드
        QualitySettings.vSyncCount = 0;
        //모니터 주사율(플레임율)이 다른 컴퓨터일 경우 캐릭터 조작시 빠르게 움직일 수 있다.

        GameObject a_GDObj = GameObject.Find("GameDirector");
        if (a_GDObj != null)
            m_GDirector = a_GDObj.GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameDirector.s_State == GameState.Ready)
        {
            // 스와이프 길이 구하기
            if (Input.GetMouseButtonDown(0) == true)
            { // 마우스 단추를 클릭한 좌표
                startPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {   // 마우스 버튼에서 손을 떼었을 때 좌표
                Vector2 endPos = Input.mousePosition;
                float swipeLength = (endPos.x - startPos.x);

                // 스와이프 길이를 처음 속도로 변경
                speed = swipeLength / 500.0f;

                // 효과음 재생 (추가)
                GetComponent<AudioSource>().Play();

                GameDirector.s_State = GameState.MoveIng;
            }

        }//if(GameDirector.s_State == GameState.Ready)
        else if (GameDirector.s_State == GameState.MoveIng)
        {
            transform.Translate(speed, 0, 0);   // 이동
            speed *= 0.98f;                     // 감속

            if(speed < 0.0005f) // 자동차가 멈췄다고 판단
            {
                speed = 0.0f;

                // 게임 상태 변경
                GameDirector.s_State = GameState.Ready;

                if(m_GDirector !=null)
                    m_GDirector.RecordLength(); // 지금 플레이어가 끝난 유저의 기록을 저장

                // --- 다음 플레이어를 위한 초기화
                transform.position = new Vector3(-7.0f, -3.7f, 0.0f);

            }// if(speed < 0.0005f) // 자동차가 멈췄다고 판단

        }//else if (GameDirector.s_State == GameState.MoveIng)

    }

    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject()​


}