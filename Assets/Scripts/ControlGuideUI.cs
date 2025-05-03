using UnityEngine;
using UnityEngine.UI;

public class ControlGuideUI : MonoBehaviour
{
    public GameObject guidePanel;  
    public Button okBtn; // 확인 버튼

    void Start()
    {
        guidePanel.SetActive(true); // 게임 시작 시 UI 표시
        okBtn.onClick.AddListener(CloseGuide);
    }

    void CloseGuide()
    {
        guidePanel.SetActive(false); // 가이드패널 비활성화
        GameManager.Instance.StartGame(); // 비활성화 한 후에 게임 시작 
    }
}
