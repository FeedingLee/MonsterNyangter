using UnityEngine;

public class EasterEggSound : MonoBehaviour
{
    private void Start()
    {
        // 클릭될 때 소리가 나도록 OnClick 메서드를 호출
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        // AudioManager를 통해 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.NyanEaster);
    }
}
