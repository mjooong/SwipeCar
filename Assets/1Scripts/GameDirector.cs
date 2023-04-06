using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Ready = 0,      // 출발선 준비 상태
    MoveIng = 1,    // 자동차가 움직이는 상태
    GameEnd = 2,    // 플레이어 3명의 플레이가 끝난 상태
}

public class PlayerData
{
    public int m_idx = 0;           // 플레이어 순서대로 번호 0, 1, 2
    public float m_SvLen = 0.0f;    // 각 플레이어 별로 이동거리를 저장하기 위한 변수
    public int m_Ranking = -1;      // 랭킹 변수
}

public class GameDirector : MonoBehaviour
{
    public static GameState s_State = GameState.Ready;  //  초기화

    GameObject car;
    GameObject flag;
    GameObject distance;

    public Text[] PlayerUI;  // 번호 표시 UI 연결 변수
    int PlayerCount = 0;  // 플레이어 인덱스 카운드용 변수
    // 0 일때 Player1 플레이 중,
    // 1 일때 Player2 플레이 중,
    // 2 일때 Player3 플레이 중

    Text ResultText;
    float m_Length = 0.0f; // 거리 저장용 변수

    //float[] m_SvLen = new float[3];     // 각 플레이어 별로 이동거리를 저장하기 위한 변수
    //int[] m_Ranking = new int[3];       // 각 플레이어의 순위를 저장하기 위한 변수
    List<PlayerData> m_PlayerList = new List<PlayerData>();

    public Button ResetBtn;

    // 정렬 조건 함수
    int SvLenComp(PlayerData a, PlayerData b)
    {
        return a.m_SvLen.CompareTo(b.m_SvLen);  // 오름차순 정렬 : 낮은 값에서 높은 값으로
    }

    // Start is called before the first frame update
    void Start()
    {
        s_State = GameState.Ready;  // 게임이 다시 플레이 될 수 있도록 초기화

        car = GameObject.Find("car");
        flag = GameObject.Find("flag");
        distance = GameObject.Find("Distance");

        if (distance != null)
            ResultText = distance.GetComponent<Text>();

        if (ResetBtn != null)
            ResetBtn.onClick.AddListener(ResetBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        float length = flag.transform.position.x - car.transform.position.x;
        length = Mathf.Abs(length);
        if(ResultText != null)
            ResultText.text = "목표 지점까지" + length.ToString("F2") + "m";

        m_Length = length;
    }

    // -- 각 플레이어가 도착하면 기록을 화면에 표시하고 저장해 놓기 위한 함수
    public void RecordLength()
    {
        if(PlayerCount < PlayerUI.Length)
        {
            PlayerUI[PlayerCount].text = "Player " + (PlayerCount + 1).ToString() + " : " + m_Length.ToString("F2") + "m";

            //m_SvLen[PlayerCount] = m_Length;
            PlayerData a_Node = new PlayerData();
            a_Node.m_idx = PlayerCount; // 유저별 인덱스
            a_Node.m_SvLen = m_Length;  // 유저별 기록
            m_PlayerList.Add(a_Node);

            PlayerCount++;
        }

        // 자동차가 멈출 때 마다 게임 종료 조건 체크하는 코드
        if(3 <= PlayerCount)
        {
            s_State = GameState.GameEnd;

            // 순위 판정
            RankingAlgorithm();

            // 리플레이 버튼 활성화
            if (ResetBtn != null)
                ResetBtn.gameObject.SetActive(true);
        }

    }// public void RecordLength()

    void RankingAlgorithm()
    {
        // 오름 차순 정렬(ASC)
        m_PlayerList.Sort(SvLenComp);   // 정렬

        PlayerData a_Player = null;
        for(int ii =0; ii < m_PlayerList.Count; ii++)
        {
            a_Player = m_PlayerList[ii];

            if (PlayerUI.Length <= a_Player.m_idx)
                continue;

            a_Player.m_Ranking = ii + 1;    // 랭킹 저장
            PlayerUI[a_Player.m_idx].text = "Player " + (a_Player.m_idx + 1).ToString() + " : " + a_Player.m_SvLen.ToString("F2") + "m " + 
                                             a_Player.m_Ranking.ToString() + " 등";
        }
    }

    void ResetBtnClick()
    {
        SceneManager.LoadScene("Game");
    }
}
