using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLine : MonoBehaviour
{
    // Player의 위치와 동기화
    public GameObject PlayerObject;
    public Vector2 PlayerJoyVector;
    Vector2 PlayerVec;
    // 애니메이션 컨트롤러 불러오기
    public RuntimeAnimatorController[] AnimCon;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;

    // Awake = 최초 활성화 시 
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        sprite = GetComponent<SpriteRenderer>();
    }

    // OnEnable = 활성화 될 때마다
    void OnEnable()
    {
        // 애니메이션 런 타임 컨트롤러에 -> GameManger의 아이디 번호 가져오기
        // 즉 3번이 입력되면, 3번째 배열에 있는 AC를 가져옴
        anim.runtimeAnimatorController = AnimCon[GameManager.instance.playerId];
    }
    // Update = 매 프레임마다 
    void Update()
    {
        //위치 확인 & 변경 함수 실행
        ChangePosition();
        /* Player 스크립트를 참고하여, Player 스크립트의 IinputVec 변수를 
        PlayerJoyVector로 가져옴*/
        PlayerJoyVector = PlayerObject.GetComponent<Player>().inputVec;
        
        // 걷는 모습을 위한 Animation Speed 조정
        anim.SetFloat("Speed", PlayerJoyVector.magnitude);

        // PlayerJoyVector의 X값에 따라 스프라이트를 뒤집음
        if (PlayerJoyVector.x != 0)
        {
            sprite.flipX = PlayerJoyVector.x > 0;
        }
    }

    // 위치를 바꾸는 함수 
    void ChangePosition()
    {
        PlayerVec = PlayerObject.transform.position;
        transform.position = PlayerVec;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 사망로직
            if (GameManager.instance.health <= 0)
            {
                anim.SetTrigger("Dead");
            }
            else
            {
                anim.SetTrigger("Damage");
            }
        }
    }
}
