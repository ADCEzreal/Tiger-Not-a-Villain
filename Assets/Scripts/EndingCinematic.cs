using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingCinematic : MonoBehaviour
{
    [Header("Character")]
    public GameObject tiger;
    public GameObject person;
    private Animator tAnimator, pAnimator;  
    
    [Header("Camera")]
    public Camera mainCamera;
    
    [Header("UI")] 
    public Image blackPanel; // 패널 알파값 조절용 (UI 이미지)
    public GameObject restartButton;
    public GameObject quitButton;
    public TextMeshProUGUI endingMessage;

    [Header("Timing")]
    public float fadeDuration = 2f;
    public float transformTime = 5f;
    public float walkDuration = 3f;
    public float messageDelay = 5f;

    private float timer = 0f;

    private bool fadedIn = false;
    private bool transformed = false;
    private bool messageStart = false;
    void Start()
    {
        // 처음은 0.8에서 시작. 
        SetPanelAlpha(0.8f);

        StartCoroutine(FadeOutPanel());

        tiger.SetActive(true);
        tAnimator = tiger.GetComponent<Animator>();
        pAnimator = person.GetComponent<Animator>();
        person.SetActive(false);
        restartButton.SetActive(false);
        quitButton.SetActive(false);
        endingMessage.text = "";

        StartCoroutine(FadeOutPanel());
    }

    IEnumerator FadeOutPanel() // 패널을 점점 투명하게하는 코루틴    
    {
        float t = 0f; 
        Color color = blackPanel.color; // 현재 패널 색상 가져오기  

        while (t < fadeDuration) // 서서히 알파값 줄이기    
        {
            t += Time.deltaTime;
            // 0.8 → 0으로 알파값을 선형 보간 (Lerp)
            // Mathf.Lerp(시작값, 끝값, 진행도) 
            // 진행도는 0 ~ 1 사이 값 (t / fadeDuration)
            color.a = Mathf.Lerp(0.8f, 0f, t / fadeDuration);   
            blackPanel.color = color; // 변경된 알파값 패널에 적용  
            yield return null;
        }
        fadedIn = true;
        timer = 0f; // 호랑이 걷기 시작
    }

    void Update()
    {
        if (!fadedIn) return;

        timer += Time.deltaTime;
        float speed = 2f;
        // 호랑이 걷기
        if (!transformed && timer < transformTime)
        {
            tiger.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            tAnimator.SetFloat("Speed", speed);
        }

        // 변신 시점
        if (!transformed && timer >= transformTime)
        {
            tiger.SetActive(false); // 호랑이 활성화
            
            // 호랑이의 포지션과 로테이션 값 받아오기   
            person.transform.position = tiger.transform.position; 
            person.transform.rotation = tiger.transform.rotation;

            person.SetActive(true); // 사람 활성화
            transformed = true;
        }

        // 사람 계속 걷기   
        if (transformed)
        {
            person.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            pAnimator.SetFloat("MoveSpeed", speed);
        }

        // 변신 후 일정 시간 지나면 메세지 시작 
        if (transformed && !messageStart && timer >= 2f)
        {
            StartCoroutine(PlayFinalMessage());
            messageStart = true;
        }
    }
    private void SetPanelAlpha(float alpha) // 투명도 설정 함수 
    {
        Color c = blackPanel.color; // 패널의 색상을 가져옴.
        c.a = alpha;    // 색상의 알파값만 수정 
        blackPanel.color = c; // 다시 패널에 적용   
    }
    IEnumerator PlayFinalMessage()
    {
        // 화면을 다시 어둡게 (알파 0 → 0.7)
        float t = 0f;
        Color color = blackPanel.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 0.7f, t / fadeDuration);
            blackPanel.color = color;
            yield return null;
        }
        // 텍스트 순차 출력
        string[] messages =
        {
            "사람들은 너를 길들이려 하지 않았다.",
            "그저... 네가 스스로 '사람'처럼 되기를 원했을 뿐.",
            "마늘은 던져진 것이 아니다.",
            "너는 스스로 먹기를 선택했을 뿐."
        };
         // 메시지 보여주기
        endingMessage.gameObject.SetActive(true);
        // 앞의 두 문장은 한 줄씩 출력하고 사라지게
        for (int i = 0; i < 2; i++)
        {
            endingMessage.text = messages[i];
            yield return new WaitForSeconds(2.5f);
        }
        // 마지막 두 문장은 같이 표시
        endingMessage.text = messages[2];
        yield return new WaitForSeconds(2.5f);

        endingMessage.text += "\n" + messages[3];
        yield return new WaitForSeconds(2.5f);
        // 버튼 등장
        restartButton.SetActive(true);
        quitButton.SetActive(true);
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickQuit()
    {
        Debug.Log("게임 나가기 버튼 클릭됨");
        Application.Quit();
       
    }
}
