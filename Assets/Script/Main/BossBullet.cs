using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    
    public float launchForce;                       // 발사할 힘의 크기
    public float damage;                            // 발사체 데미지
    public RuntimeAnimatorController[] animCon;     // 애니메이터 배열

    Rigidbody2D rb;                                 // Rigidbody 컴포넌트를 가져오기 위한 변수
    Animator anim;                                  // 발사체의 애니메이터

    // 오브젝트가 생성되면 자동으로 실행되는 메서드
    private void Awake()
    {
        // Rigidbody 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(ResetBullet());
    }

    // 발사체 초기화 함수
    public void Init(float damage, Vector3 dir, int index)
    {
        this.damage = damage;
        rb.velocity = dir;

        anim.runtimeAnimatorController = animCon[index];
    }

    // 벽이나 기타 오브젝트에 부딪힐 경우 파괴되는 코드
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 발사체가 플레이어와 접촉 시 데미지 입음
            GameManager.instance.health -= damage;

            // 플레이어 데미지, 사망 애니메이션 조정
            Animator anim = collision.gameObject.GetComponent<Animator>();

            // 발사체 비활성화
            gameObject.SetActive(false);
            
            // 피격 모션
            anim.SetTrigger("Damage");
        } 
        else if (collision.CompareTag("Wall"))
        {
            // 발사체가 울타리와 접촉 시 비활성화
            gameObject.SetActive(false);
        }
    }

    private IEnumerator ResetBullet()
    {
        // 보스 발사체가 필드에 남아있을경우 비활성화
        yield return new WaitForSeconds(10.0f);
        gameObject.SetActive(false);
    }
}
