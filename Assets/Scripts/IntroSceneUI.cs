using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneUI : MonoBehaviour
{
    public void OnClickStartButton() // 게임 시작 버튼
    {
        SceneManager.LoadScene("Game");
        Debug.Log("게임 시작");
    }
    public void OnClickQuitButton() // 게임 종료 버튼
    {
        Application.Quit();
    }
}
