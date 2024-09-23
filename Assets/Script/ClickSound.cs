using UnityEngine;

public class ClickSound : MonoBehaviour
{
    public AudioManager.Sfx clickSoundEffect; // 재생할 효과음 선택

    private void Start()
    {
        // 클릭될 때 소리가 나도록 OnClick 메서드를 호출
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        // AudioManager를 통해 효과음 재생
        AudioManager.instance.PlaySfx(clickSoundEffect);
    }
}
