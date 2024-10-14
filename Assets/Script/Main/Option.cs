using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class Option : MonoBehaviour
{
    public GameObject player;        // 플레이 유무를 파악할 PlayerObject 받기
    public GameObject panel;         // Option Panel 오브젝트 배치 필요
    public Button optionButton;      // Option Button 오브젝트 배치 필요
    public Button continueButton;    // Option Panle -> Continue 배치 필요
    public GameObject uiJoy;         // JoyStick 오브젝트 배치 필요
    public GameObject GameResult;    // GameResult 오브젝트 배치 필요

    private bool isPaused = false;
    private bool OptionPanel = false;

    void Start()
    {
        // 게임 시작 시 Option Panel을 비활성화
        panel.SetActive(false);

        // Option 버튼에 클릭 이벤트 리스너 등록
        optionButton.onClick.AddListener(ToggleOption);

        // Continue 버튼에 클릭 이벤트 리스너 등록
        continueButton.onClick.AddListener(ContinueGame);
    }

    public void Update()
    {
        /* [ 조건 ] 
        1. LevelUpFinish 가 true 일때
        2. Player 오브젝트가 true 일때
        3. 게임결과 판넬이 False 일때 
        4. 옵션창이 꺼져있을 때
        =  조이스틱 활성화.*/
        if (LevelUp.instance.LevelUpFinish == true &&
            player.gameObject.activeInHierarchy == true &&
            GameResult.gameObject.activeInHierarchy == false &&
            OptionPanel == false)
        {
            uiJoy.SetActive(true);
        }
        // 레벨업이 끝나지 않았다면, 정지함수 실행
        else if (LevelUp.instance.LevelUpFinish == false)
        {
            GameManager.instance.Stop();
        }
        // 게임을 승리하거나 패배하여 종료된다면, 옵션 아이콘 비활성화
        else if (GameResult.gameObject.activeInHierarchy == true)
        {
            optionButton.gameObject.SetActive(false);
        }
    }

    public void ToggleOption()
    {
        isPaused = !isPaused;

        // Option Panel 활성화 상태 전환
        panel.SetActive(isPaused);

        // 게임 일시정지 및 재개
        if (isPaused)
        {
            OptionPanel = true;
            GameManager.instance.Stop();
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
            uiJoy.SetActive(false); // 조이스틱 꺼진상태 유지
            isPaused = false;
        }
        else
        {
            panel.SetActive(false);
            GameManager.instance.Resume();
            isPaused = false;
            OptionPanel = false;
        }
    }
}
