using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBerserkMode : MonoBehaviour
{
    [Header("# Multi-Directional Firing")]   // 전방향 발사 관련 변수
    public int MBulletCount;                 // 발사 갯수
    public float MBossBulletDamage;          // 발사 데미지
    public float MFireSpeed;                 // 발사 속도

    [Header("# One-Directional Firing")]     // 단일방향 발사 관련 변수
    public int OBulletCount;                 // 발사 갯수
    public float OBossBulletDamage;          // 발사 데미지
    public float OFireSpeed;                 // 발사 속도

    [Header("# BossDash")]
    public float DashSpeed;                  // 돌진 속도
    public float BossDashDamage;             // 보스 돌진 데미지
    public float BossDashDelay;              // 보스 돌진 대기 시간 

    [Header("# BossState")]
    public float interval;                   // 다음 패턴까지의 쿨타임

    [Header("# Landing Firing")]             // 착지 전방향 발사 관련 변수
    public int LBulletCount;                 // 발사 갯수 20
    public float LBossBulletDamage;          // 발사 데미지
    public float LFireSpeed;                 // 발사 속도 0.5

    BossPattern bossPatten;
    BossReposition bossReposition;

    private void Awake()
    {
        bossPatten = gameObject.GetComponent<BossPattern>();
        bossReposition = gameObject.GetComponent<BossReposition>();
    }

    public void Init()
    {
        // 광폭화 모드가 활성화 되었을때 호출될 함수
        if (bossPatten != null && bossReposition != null)
        {
            // 전방향 발사 계수 수정
            bossPatten.MBulletCount = MBulletCount;
            bossPatten.MBossBulletDamage = MBossBulletDamage;
            bossPatten.MFireSpeed = MFireSpeed;

            // 단일방향 발사 계수 수정
            bossPatten.OBulletCount = OBulletCount;             
            bossPatten.OBossBulletDamage = OBossBulletDamage;   
            bossPatten.OFireSpeed = OFireSpeed;

            // 돌진 계수 수정
            bossPatten.DashSpeed = DashSpeed;           
            bossPatten.BossDashDamage = BossDashDamage;         
            bossPatten.BossDashDelay = BossDashDelay;        
            
            // 패턴 쿨타임 수정
            bossPatten.interval = interval;

            // 착지 발사 계수 수정
            bossReposition.LBulletCount = LBulletCount;            
            bossReposition.LBossBulletDamage = LBossBulletDamage;  
            bossReposition.LFireSpeed = LFireSpeed;                
        }

        else
        {
            Debug.Log("bossPartten or bossReposition is null : " + bossPatten + " , " + bossReposition);
        }
    }
}   
