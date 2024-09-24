using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public RuntimeAnimatorController[] animCon;

    public JoystickController joystick;  // JoystickController를 연결합니다.

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
    }

    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // JoystickController에서 inputVec을 가져옵니다.
        inputVec = joystick.GetInputVector();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);
        // 방향에 따른 스프라이트 뒤집기
        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x > 0;
        }
    }

   

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            {
            if (!GameManager.instance.isLive)
                return;
            GameManager.instance.health -= Time.deltaTime * 50;
            
            // 사망로직
            if (GameManager.instance.health <= 0)
            {
                // 자식 오브젝트를 비활성화 하는 이유 : 사망을 했을 경우 나머지 Player의 자식 이펙트들을 비활성화 해야하는데,
                // Index 0 또는 1까지 비활성화 하면 뭔가 다른것까지 사라질거라고 판단됨
                // index가 0~1일 경우 : 바닥이 사라짐
                /*for (int index = 2; index < transform.childCount; index++)
                {
                    transform.GetChild(index).gameObject.SetActive(false);
                }*/

                anim.SetTrigger("Dead");
                GameManager.instance.GameOver();
            }
            else
            {
                anim.SetTrigger("Damage");
            }
        }
    }
}
