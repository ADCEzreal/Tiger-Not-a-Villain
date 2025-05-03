using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingSceneUI : MonoBehaviour
{
    public Image panel;
    public TextMeshProUGUI endingText1;
    public TextMeshProUGUI endingText2;
    public Button yesButton;
    public Button noButton;
    public float fadeDuration = 2f;
    public float delay = 2f;
    

    void Start()
    {
         Time.timeScale = 1f; // 일시정지 해제
        // 처음에 비활성화
        endingText1.gameObject.SetActive(false);
        endingText2.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        StartCoroutine(PlayLastQuestion()); 
    }
    IEnumerator PlayLastQuestion()
    {
        // 검정 패널 알파값 0 -> 1
        Color color = panel.color;
        color.a = 0f;
        panel.color = color;

        float timer = 0f;
        float targetAlpha = 0.8f; // 너무 불투명하지 않게 

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, targetAlpha, timer / fadeDuration);
            panel.color = color;
            yield return null;
        }

        // 텍스트 등장
        yield return new WaitForSeconds(delay);
        endingText1.gameObject.SetActive(true);
        // 텍스트 한 줄씩
        yield return new WaitForSeconds(delay);
        endingText2.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);
        // 버튼 등장
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }

    public void OnYesClicked()
    {
        Debug.Log("사람이 되기를 선택했습니다.");
        SceneManager.LoadScene("HumanEndingScene");
    }

    public void OnNoClicked()
    {
        Debug.Log("사람이 되지 않기를 선택했습니다.");
        SceneManager.LoadScene("IntroScene");
    }
}
