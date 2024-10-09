using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 기본값을 설정하고, 저장된 값이 있다면 불러옴
        bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume", 0.2f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.5f);

        // 슬라이더 값이 변경될 때마다 호출될 메서드 연결
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        // 초기 볼륨 설정
        SetBgmVolume(bgmSlider.value);
        SetSfxVolume(sfxSlider.value);
    }

    // BGM 볼륨 설정 및 저장
    public void SetBgmVolume(float volume)
    {
        AudioManager.instance.bgmVolume = volume;
        AudioManager.instance.bgmPlayer.volume = volume;
        AudioManager.instance.WaitingbgmPlayer.volume = volume;
        PlayerPrefs.SetFloat("BgmVolume", volume);
    }

    // SFX 볼륨 설정 및 저장
    public void SetSfxVolume(float volume)
    {
        AudioManager.instance.sfxVolume = volume;

        // 모든 SFX 채널의 볼륨을 업데이트
        foreach (AudioSource sfxPlayer in AudioManager.instance.sfxPlayers)
        {
            sfxPlayer.volume = volume;
        }
        PlayerPrefs.SetFloat("SfxVolume", volume);
    }
}
