using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public JoystickController joystickController;

    [Header("# Game Control")]
    public bool isLive;
    // 실제 흐르는 게임 시간
    public float gameTime;
    // 최대 게임 시간 
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public int playerId;
    public int weaponcode;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# GameObject")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public GameObject uiJoyStick;
    public GameObject enemyCleaner;
    
    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        switch (playerId)
        {
            case 0:
                weaponcode = 0;
                break;
            case 1:
                weaponcode = 1;
                break;
            case 2:
                weaponcode = 4;
                break;
            case 3:
                weaponcode = 2;
                break;
        }        

        player.gameObject.SetActive(true);
        uiJoyStick.SetActive(true);
        uiLevelUp.Select(weaponcode); 
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }
    
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        // 획득했던 스텟 값 초기화
        Gear.rate_stat = 0.0f;
        Gear.damage_stat = 0.0f;
        Gear.stamina_stat = 0.0f;
        Gear.upgrade_stat = 0;
        // 무기 레벨 초기화
        Weapon.DB_level = 0;
        Weapon.HB_level = 0;
        Weapon.SH_level = 0;
        Weapon.GS_level = 0;
        Weapon.LC_level = 0;

        isLive = false;

        yield return new WaitForSeconds(1f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        // 이전 게임에서 얻었던 값 초기화
        Gear.rate_stat = 0.0f;
        Gear.damage_stat = 0.0f;
        Gear.stamina_stat = 0.0f;
        Gear.upgrade_stat = 0;
        // 무기 레벨 초기화
        Weapon.DB_level = 0;
        Weapon.HB_level = 0;
        Weapon.SH_level = 0;
        Weapon.GS_level = 0;
        Weapon.LC_level = 0;

        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(1);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }

    public void GetExp()
    {
        if (!isLive)
            return;

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        uiJoyStick.SetActive(false);
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoyStick.SetActive(true);

        // 게임 Resume 시 조이스틱 초기화
        JoystickController joystickHandle = uiJoyStick.GetComponent<JoystickController>();
        if (joystickHandle != null)
        {
            joystickHandle.ResetJoystick();
        }
    }
}