using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public BossSpawnData[] bossSpawnData;
    public float levelTime;

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
            //Spawn(); 
        }
    }
    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }

    public void BossSpawn()
    {
        GameObject Boss = GameManager.instance.pool.GetEnemy(1);
        Boss.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        Boss.GetComponent<BossPattern>().Init(bossSpawnData[0]);
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
}

[System.Serializable]
public class BossSpawnData
{
    public float speed;
    public float health;
}