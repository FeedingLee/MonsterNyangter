using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    public float expSpawnRange;                         // 경험치 스폰 범위
    public int expSpawnIndex;                         // 경험치구슬 드랍 갯수

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data, int index)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        expSpawnIndex = index;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Enemy_Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;

            // 몬스터가 Exp구슬을 드랍하므로 직접 경험치가 증가되는 코드 주석처리
            //GameManager.instance.GetExp();
            
            if (expSpawnIndex > 1)
            {
                // 강화몬스터인 경우
                SpawnExp(expSpawnIndex);
            }
            else
            {
                // 일반 몬스터인 경우
                Transform exp = GameManager.instance.pool.GetEnemy(0).transform;
                exp.parent = GameManager.instance.pool.transform;
                exp.position = transform.position;
            }

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Enemy_Dead);
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);
    }

    void Dead()
    {
        // 빨간색의 강화몬스터의 색상을 초기화하기 위한 코드
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;

        gameObject.SetActive(false);
    }

    void SpawnExp(int expIndex)
    {
        for (int i=0; i< expIndex; i++)
        {
            Transform exp = GameManager.instance.pool.GetEnemy(0).transform;
            exp.parent = GameManager.instance.pool.transform;

            float expSpawnX = Random.Range(-expSpawnRange / 2, expSpawnRange / 2 + 1);
            float expSpawnY = Random.Range(-expSpawnRange / 2, expSpawnRange / 2 + 1);

            exp.position = transform.position + new Vector3(expSpawnX, expSpawnY);
        }
    }
}
