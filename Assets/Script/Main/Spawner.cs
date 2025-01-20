using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public BossSpawnData[] bossSpawnData;
    public float levelTime;
    public Color StrongMonsterColor;

    int level;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 타이머의 시간에 맞춰 소환
        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        // 스폰데이터의 레벨에 따라 소환
        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn(); 
        }
    }
    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

        int randomIndex = Random.Range(0, 100);

        // chance : 몬스터의 체력이 2배가 될 확률
        if (spawnData[level].chance >= randomIndex)
        {
            // 원래 체력 임시 저장
            int tmp = spawnData[level].health;

            // 체력을 임의의 배율만큼 증가
            float newHp = spawnData[level].health * spawnData[level].stronger;

            Debug.Log("new 체력 : " + (int)newHp);

            // 체력을 2배로
            spawnData[level].health = (int)newHp;

            // 체력이 2배인 몬스터는 색상이 지정한 색으로 변경
            enemy.gameObject.GetComponent<SpriteRenderer>().color = StrongMonsterColor;

            // 몬스터 스탯 초기화
            enemy.GetComponent<Enemy>().Init(spawnData[level]);

            // 체력 변수를 원래대로
            spawnData[level].health = tmp;
        }
        else
        {
            enemy.GetComponent<Enemy>().Init(spawnData[level]);
            Debug.Log("기존 체력: " + spawnData[level].health);
        }

    }

    public void BossSpawn()
    {
        GameObject Boss = GameManager.instance.pool.GetEnemy(1);

        // 보스가 스폰되자마자 내려찍기 패턴을 사용하는데 이 코드때문에 플레이어 주위에 대기하게 되므로 주석처리
        // 주석처리 하면 보스가 정상적으로 화면밖에 스폰됨
        //Boss.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        
        Debug.Log("BossSpawn in spawner");
    }
}

//소환데이터 설정 클래스 [ 스프라이트 타입, 소환시간, 체력 등 ]
[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
    public int chance;
    public float stronger;
}

[System.Serializable]
public class BossSpawnData
{
    public float speed;
    public float health;
}