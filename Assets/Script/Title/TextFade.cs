using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextFade : MonoBehaviour
{
    public float fadeDuration = 2f; // 페이드 인/아웃 지속 시간 (초)
    public Color targetColor = Color.white; // 목표 색상 (이 경우, 흰색)
    private Color initialColor; // 텍스트 UI의 초기 색상
    private Text textComponent; // 텍스트 UI 컴포넌트에 대한 참조

    private void Start()
    {
        textComponent = GetComponent<Text>(); // 이 게임 오브젝트에 부착된 텍스트 컴포넌트 가져오기
        initialColor = textComponent.color; // 텍스트의 초기 색상 저장
        // 시작할 때 페이드 아웃 효과를 위해 초기 알파 값을 0으로 설정
        initialColor.a = 0f;
        textComponent.color = initialColor;
        // 페이드 프로세스 시작
        StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        while (true)
        {
            // 페이드 아웃
            yield return FadeOut();
            // 페이드 인 전에 잠시 대기
            yield return new WaitForSeconds(0.5f);
            // 페이드 인
            yield return FadeIn();
            // 다시 페이드 아웃하기 전에 잠시 대기
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            // 초기 알파에서 0까지의 알파 값을 보간
            Color newColor = Color.Lerp(initialColor, targetColor, t);
            newColor.a = Mathf.Lerp(1f, 0f, t); // 페이드 아웃을 위해 알파 값을 설정
            textComponent.color = newColor;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            // 0에서 초기 알파까지의 알파 값을 보간
            Color newColor = Color.Lerp(initialColor, targetColor, t);
            newColor.a = Mathf.Lerp(0f, 1f, t); // 페이드 인을 위해 알파 값을 설정
            textComponent.color = newColor;
            yield return null;
        }
    }
}
