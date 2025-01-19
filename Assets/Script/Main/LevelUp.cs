using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    public Object GM_Objcet;  // 게임매니저 오브젝트 가져오기

    public RectTransform Joy;
    public RectTransform Stick;

    public static LevelUp instance;
    public bool LevelUpFinish = true;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
        instance = this;
    }

    public void Show() // 아이템 창 띄우기
    {
        LevelUpFinish = false;
        Next();
        rect.localScale = Vector3.one;
        Joy.gameObject.SetActive(false);
        Stick.gameObject.SetActive(false);
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Player_LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide() // 아이템 창 숨기기
    {
        LevelUpFinish = true;
        rect.localScale = Vector3.zero;
        Joy.gameObject.SetActive(true);
        Stick.gameObject.SetActive(true);
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 2. 그 중에서 랜덤 3개 아이템 활성화
        int[] ran = new int[3];

        // 3. 5의 배수 레벨에는 능력치만 선택지에 나타나도록 
        if (GM_Objcet.GetComponent<GameManager>().level % 5 == 0)
        {
            while (true)
            {
                ran[0] = Random.Range(5, 10);
                ran[1] = Random.Range(5, 10);
                ran[2] = Random.Range(5, 10);

                if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                    break;
            }
        }
        else
        {
            while (true)
            {
                ran[0] = Random.Range(0, items.Length);
                ran[1] = Random.Range(0, items.Length);
                ran[2] = Random.Range(0, items.Length);

                if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                    break;
            }
        }
        
        for (int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];

            // 4. 만렙 아이템의 경우는 소비아이템으로 대체
            if (ranItem.level == ranItem.data.damages.Length)
            {
                // 랜덤아이템이 많다면 (4~7번까지) 강의 12 [39:42] 참고
                items[Random.Range(10, 13)].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}
