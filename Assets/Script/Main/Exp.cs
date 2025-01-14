using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public Sprite[] expSprites = new Sprite[3];         // exp 스프라이트 배열
    SpriteRenderer sprite;                              // exp 오브젝트의 스프라이트를 변경하기 위해 선언

    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (Item.Exp_Bonus == 1)
        {
            sprite.color = new Color(255f / 255f, 30f / 255f, 0f / 255f, 255f / 255f);                   
        }
        else if (Item.Exp_Bonus == 0)
        {
            sprite.color = new Color(255, 255, 255, 255);
        }
    }

    public void OnEnable()
    {
        // exp가 활성화 될 때마다 스프라이트 변경
        int x = Random.Range(0, expSprites.Length - 1);
        sprite.sprite = expSprites[x];
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트의 이름이 Player 인 경우
        if (collision.gameObject.name == "Player")
        {
            // Player 의 경험치 증가
            GameManager.instance.GetExp();
            if (Item.Exp_Bonus == 1)
            {
                GameManager.instance.GetExp();
            }
            // 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}
