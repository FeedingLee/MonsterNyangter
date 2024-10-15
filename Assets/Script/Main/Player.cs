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

    public JoystickController joystick;      // JoystickController를 연결합니다.

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
        inputVec = joystick.GetInputVector(); // JoystickController에서 inputVec을 가져옵니다.
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
        if (inputVec.x != 0)                  // 방향에 따른 스프라이트 뒤집기
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
                 
            if (GameManager.instance.health <= 0)// 사망로직
            {
                gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
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
