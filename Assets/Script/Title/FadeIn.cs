using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    private AudioSource audioSource;
    public float maxVolume; // 최대 음량 설정
    public double fadeInSeconds;
    bool isFadeIn = true;
    double fadeDeltaTime = 0;

    // 시작 시점에서 호출됨
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 매 프레임마다 호출됨
    void Update()
    {
        if (isFadeIn)
        {
            fadeDeltaTime += Time.deltaTime;
            if (fadeDeltaTime >= fadeInSeconds)
            {
                fadeDeltaTime = fadeInSeconds;
                isFadeIn = false;
            }
            float volume = (float)(fadeDeltaTime / fadeInSeconds) * maxVolume; // 최대 음량을 곱해서 설정
            audioSource.volume = volume;
        }
    }
}
