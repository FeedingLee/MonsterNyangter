using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    public GameObject player; // 플레이 유무를 파악할 PlayerObject 받기
    public GameObject panel;         // Option Panel 오브젝트
    public Button optionButton;      // Option 버튼 오브젝트
    public Button continueButton;    // Continue 버튼 오브젝트
    public GameObject uiJoy; // JOYstick 오브젝트

    private bool isPaused = false;

    void Start()
    {
        // 게임 시작 시 Option Panel을 비활성화
        panel.SetActive(false);

        // Option 버튼에 클릭 이벤트 리스너 등록
        optionButton.onClick.AddListener(ToggleOption);

        // Continue 버튼에 클릭 이벤트 리스너 등록
        continueButton.onClick.AddListener(ContinueGame);
    }

    public void ToggleOption()
    {
        isPaused = !isPaused;

        // Option Panel 활성화 상태 전환
        panel.SetActive(isPaused);

        // 게임 일시정지 및 재개
        if (isPaused)
        {
            Time.timeScale = 0;  // 게임 시간 정지
            uiJoy.SetActive(false);
        }
        else
        {
            ContinueGame();
        }
    }

    public void ContinueGame()
    {
        // 플레이어 오브젝트가 꺼져있으면, 게임 실행중이 아니기 때문에
        if (player.gameObject.activeInHierarchy == false)
        {
            panel.SetActive(false);
            uiJoy.SetActive(false); // 조이스틱이 꺼진 상태를 유지
            Time.timeScale = 1;
            isPaused = false;
        }
        else { 
            panel.SetActive(false);
            uiJoy.SetActive(true);
            Time.timeScale = 1;
            isPaused = false;
        }
    }
}
