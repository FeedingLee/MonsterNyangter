using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    
    public float launchForce;       // 발사할 힘의 크기
    public float damage;            // 발사체 데미지
    //public int count;
    //public int per;

    // Rigidbody 컴포넌트를 가져오기 위한 변수
    Rigidbody2D rb;
    

    // 오브젝트가 생성되면 자동으로 실행되는 메서드
    void Awake()
    {
        // Rigidbody 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(ResetBullet());
    }

    // 발사체 초기화 함수
    public void Init(float damage, /*int per,*/ Vector3 dir)
    {
        this.damage = damage;
        //this.per = per;
        rb.velocity = dir;// * launchForce;

        //if (per > -1)
        //{
        //    rb.velocity = dir * launchForce;
        //}
    }

    // 벽이나 기타 오브젝트에 부딪힐 경우 파괴되는 코드
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("BulletTrigger");
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

    IEnumerator ResetBullet()
    {
        // 보스 발사체가 필드에 남아있을경우 비활성화
        yield return new WaitForSeconds(10.0f);
        gameObject.SetActive(false);
    }
}
