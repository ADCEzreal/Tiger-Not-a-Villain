using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHPUI : MonoBehaviour
{
    public Slider hpSlider;
    public TextMeshProUGUI stunText;
    public TextMeshProUGUI blinkMessageText;
    private Coroutine blinkMessageCoroutine; // 순간이동 메세지 제어하는 Coroutine  
    public GameObject blinkIcon; 
    public void SetHealth(int current, int max) // HP 슬라이더 바 함수  
    {
        hpSlider.value = (float)current / max;
    }

    public void ShowStun(bool isStunned) // 기절 상태 함수  
    {
        stunText.gameObject.SetActive(isStunned);
    }   
    public void ShowBlinkMessage(string message) // 순간이동 메세지를 화면에 잠깐 보여주는 함수 
    {
        // 메세지 표시중이라면 중단 
        if (blinkMessageCoroutine != null)
            StopCoroutine(blinkMessageCoroutine);
        // 새로운 메세지 표시하는 코루틴 시작   
        blinkMessageCoroutine = StartCoroutine(BlinkMessageRoutine(message));
    }
    IEnumerator BlinkMessageRoutine(string message) // 순간이동 메세지 잠깐 표시 후 숨기는 Coroutine    
    {
        blinkMessageText.text = message;
        blinkMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        blinkMessageText.gameObject.SetActive(false);
    }
    public void ShowBlinkIcon(bool isVisible) // 순간이동 아이콘 함수   
    {
        blinkIcon.SetActive(isVisible);
    }
}
