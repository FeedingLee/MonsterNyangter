using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeBtn : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Fade 시간 (초)
    public float delayBeforeSceneChange = 1.0f; // 씬 변경 전 대기 시간 (초)
    public Image transparentBlock; // 투명한 블럭 UI
    public AudioSource bgmAudioSource; // BGM AudioSource
    public Button ExitBtn; // 게임종료 버튼

    private bool isClicked = false; // 버튼이 클릭되었는지 여부
    private bool hasFadedOut = false; // FadeOut이 완료되었는지 여부
    private bool hasSceneChanged = false; // 씬이 변경되었는지 여부

    private void Update()
    {
        // 버튼이 클릭되고 아직 FadeOut이 실행되지 않았다면
        if (isClicked && !hasFadedOut)
        {
            // FadeOut 실행
            FadeOut();
        }
    }

    public void ExitBtnOff()
    {
        ExitBtn.interactable = false;
    }

    // 버튼을 클릭하면 씬 변경
    public void SceneChange()
    {
        // 버튼을 처음 눌렀을 때 한 번만 실행
        if (!isClicked)
        {
            isClicked = true;

            AudioSource audio = GetComponent<AudioSource>(); 
            audio.Play();

            // FadeOut 실행
            FadeOut();
        }

        // 이미 FadeOut이 실행되었고, 씬이 아직 변경되지 않았다면
        if (hasFadedOut && !hasSceneChanged)
        {
            // 씬 변경을 위한 코루틴 실행
            StartCoroutine(DelayedSceneChange());
        }
    }

    // FadeOut 실행 함수
    private void FadeOut()
    {
        // FadeOut이 실행 중임을 표시
        hasFadedOut = true;

        // 투명한 블럭 UI의 투명도를 점진적으로 증가시키기
        StartCoroutine(FadeImageAndAudio());
    }

    // Image와 Audio FadeOut 코루틴
    private IEnumerator FadeImageAndAudio()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // 투명한 블럭 UI의 투명도를 서서히 증가시킴
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            Color color = transparentBlock.color;
            color.a = alpha;
            transparentBlock.color = color;

            // BGM 볼륨을 서서히 줄임
            bgmAudioSource.volume = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            yield return null;
        }

        // 씬 변경을 위한 대기
        yield return new WaitForSeconds(delayBeforeSceneChange);

        // 씬 변경
        SceneManager.LoadScene("Main");
    }

    // 씬 변경을 위한 딜레이 코루틴
    private IEnumerator DelayedSceneChange()
    {
        // 씬 변경을 위한 대기
        yield return new WaitForSeconds(delayBeforeSceneChange);

        // 씬 변경
        SceneManager.LoadScene("Main");
    }
}
