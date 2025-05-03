using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 라이브러리
using UnityEngine.SceneManagement;
using TMPro; // 씬 관리 관련 라이브러리
using Controller; 

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    [Header("HP")]
    public PlayerHPUI healthUI; 
    public int maxHP = 5;
    public int currentHP = 5;
    public PlayerController player;
    [Header("Garlic")]
    public int garlicCount = 0; // 먹은 마늘 개수
    public int garlicGoal = 5;  // 이 스테이지에서 먹어야 할 마늘 수
    public float timeGoal = 25f;   // 이 스테이지에서 버텨야 할 시간 (초 단위)
    [Header("Info")]
    public TextMeshProUGUI dayText; // UI에 일차 표시용 (선택)
    public TextMeshProUGUI garlicText; // UI에 마늘 수 표시용 (선택)
    [Header("Stage")]
    public GameObject stageClearPanel;  // 스테이지 마다 질문 패널  
    public GameObject gameOverPanel; // 게임오버 패널
    public GameObject level;    // 맵 회전위한 패널
    public int currentStage = 1;
    public int maxStage = 4; // 마지막 스테이지 번호 (필요에 맞게 조정)
    public AudioClip gameClearSound; // 마지막 스테이지 클리어용 사운드
    private AudioSource audioSource; // 소리 재생용
    private float surviveTime; // 생존 시간
    private bool isGameover; // 게임 오버 상태
    void Awake() 
    {
        Instance = this;
    }
    void Start()
    {
        Time.timeScale = 0f; // 조작 가이드 뜨기 전까지 일시정지 상태로 대기
    }
    public void StartGame() {
        Time.timeScale = 1f;
        // 생존 시간과 게임 오버 상태를 초기화
        surviveTime = 0;
        isGameover = false;
        currentHP = maxHP;
        healthUI.SetHealth(currentHP, maxHP);

         // 스테이지 1 시작 시 사람 1명씩 생성
        ObstaclePersonSpawner.Instance.SpawnObstaclePersons(1);
        PersonSpawner.Instance.SpawnGarlicPersons(1);
    }

    void Update() {
        // 게임 오버가 아닌 동안
        if (!isGameover)
        {
            // 생존 시간 갱신
            surviveTime += Time.deltaTime;

            UpdateUIText(); // 생존 시간 기반 일차 표시

            CheckStageClearCondition(); // 다음 스테이지로 넘어갈 지 물어보는 패널 표시 
        }
    }
    public void OnEatGarlic() // 마늘을 먹는 메소드 
    {
        garlicCount++;
        UpdateUIText();

        // 마늘개수에 대해 정화스킬 제한 처리
        player.OnEatGarlicForSkill();   
    }
    public void TakeDamage(int amount) // 돌을 맞았을 때 데미지를 받는 메소드
    {
        currentHP -= amount;
        healthUI.SetHealth(currentHP, maxHP);
        Debug.Log("피해! 현재 HP : " + currentHP);

        if (currentHP <= 0)
        {
            EndGame();
        }
    }
    public void ApplyStun() // 가짜 마늘을 맞았을 때 기절 상태를 주는 메소드
    {
        Debug.Log("기절 상태!");
        player.ApplyStunEffect(); 
    }

    void UpdateUIText() // 생존 일수와 마늘 먹은 개수 메소드    
    {
        if (dayText != null)
            dayText.text = $"{(int)surviveTime} 일차";

        if (garlicText != null)
            garlicText.text = $"마늘 개수 : {garlicCount}/{garlicGoal}";
    }
    void CheckStageClearCondition() // 클리어 조건을 달성했는지 확인하는 메소드 
    {
        if (garlicCount >= garlicGoal && surviveTime >= timeGoal) // 조건을 달성했다면  
        {
            // 게임을 멈추고 다음 스테이지로 넘어갈지 물어보는 패널 활성화    
            Time.timeScale = 0f;
            stageClearPanel.SetActive(true);
        }
    }
    public void OnConfirmNextStage() // 다음 스테이지로 넘어가는 버튼 클릭 시   
    {
        stageClearPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 일시정지 해제  
        Debug.Log("다음 스테이지로 진입!");
        LoadNextStage();
    }
    void LoadNextStage()
    {
        // 4스테이지에서는 회전 활성화
        if (currentStage + 1 == 4)
        {
            Debug.Log("4스테이지 진입! 맵 회전 시작!");
            GameObject level = GameObject.Find("Level");
            if (level != null)
            {
                Rotator rotator = level.GetComponent<Rotator>();
                if (rotator != null)
                {
                    rotator.enabled = true;
                }
            }
        }
        // 현재 스테이지가 최대 스테이지에 도달했다면   
        if (currentStage >= maxStage)
        {
            Debug.Log("최종 스테이지 클리어! 엔딩씬으로 이동");
            
            SceneManager.LoadScene("EndingScene"); // 엔딩질문씬으로 이동   
            return;
        }
        Debug.Log("스테이지 클리어!");

        // 기본 조건 초기화 및 증가
        garlicCount = 0;
        surviveTime = 0;
        garlicGoal += 5;
        currentStage++;

        // 스테이지가 증가할 수록 스포너 증가   
        ObstaclePersonSpawner.Instance.SpawnObstaclePersons(currentStage);
        PersonSpawner.Instance.SpawnGarlicPersons(currentStage);
    }
    public void EndGame() {
        // 현재 상태를 게임 오버 상태로 전환
        isGameover = true;
        Time.timeScale = 0f; // 일시 정지   
        gameOverPanel.SetActive(true); // 게임오버 패널 활성화
    }
    public void OnDeclineNextStage()
    {
        Time.timeScale = 1f; // 일시 정지 해제
        SceneManager.LoadScene("IntroScene"); 
    }
    public void OnClickRestart()
    {
        Time.timeScale = 1f; // 일시 정지 해제
        SceneManager.LoadScene("Game");
    }
}