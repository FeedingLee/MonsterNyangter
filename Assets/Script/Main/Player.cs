using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public float memoryspeed; 
    public Scanner scanner;
    public RuntimeAnimatorController[] animCon;
    public bool ismove;                      // 플레이어 이동불가상태(ex: 넉백)을 위한 변수

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
        ismove = true;
    }

    void OnEnable()
    {
        speed *= Character.Speed;
        memoryspeed = speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    void Update()
    {
        // 사망로직
        if (GameManager.instance.health <= 0)
        {
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }

        if (!GameManager.instance.isLive)
            return;               
        inputVec = joystick.GetInputVector(); // JoystickController에서 inputVec을 가져옵니다.
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive || !ismove)
            return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive || !ismove)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);
        if (inputVec.x != 0)                  // 방향에 따른 스프라이트 뒤집기
        {
            spriter.flipX = inputVec.x > 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("Boss"))
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            if (collision.gameObject.GetComponent<BossPattern>().IsBossDashing)
            {
                ismove = false;
                Debug.Log("StartCorountine KnockBack");
                Debug.Log("BossStop in Player");
                StartCoroutine(PlayerKnockBack(collision));
                collision.gameObject.GetComponent<BossPattern>().BossStop();
            }
        }
    }

    IEnumerator PlayerKnockBack(Collider2D collision)
    {
        // 피격 애니메이션 재생
        anim.SetTrigger("Damage");

        // 보스가 플레이어와 충돌 시 데미지를 받음
        Debug.Log("Damage: " + collision.gameObject.GetComponent<BossPattern>().BossDashDamage);
        GameManager.instance.health -= collision.gameObject.GetComponent<BossPattern>().BossDashDamage;
        
        // 보스 위치 계산
        Rigidbody2D target = collision.gameObject.GetComponent<Rigidbody2D>();
        
        // 플레이어 반동 방향 계산
        Vector2 dirVec = rigid.position - target.position;
        Vector2 nextVec = dirVec.normalized;
        Debug.Log("반동 방향 : " + nextVec);
        
        // 보스와 충돌 시 플레이어가 일정한 힘으로 밀려남
        rigid.velocity = Vector2.zero;
        rigid.velocity = nextVec * 15.0f;
        //rigid.AddForce(nextVec * 5f, ForceMode2D.Impulse);
        
        // 보스 돌진패턴에 피격당할 시 1.5초간 이동불가        
        yield return new WaitForSeconds(1.5f);
        rigid.velocity = Vector2.zero;
        ismove = true;
    }
}
