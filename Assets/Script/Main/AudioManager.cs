using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#Checking Is Playing")]
    public GameObject Need_GameResult_Obj;
    public GameObject Need_Player_Obj;

    [Header("#BGM")]
    public List<AudioClip> bgmClip = new List<AudioClip>();
    public float bgmVolume;
    public AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;
    // BGM 마지막 재생곡을 체크하기 위한 값
    private int LastIndex = -1;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    public AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }

    void Awake()
    {
        bgmPlayer = GetComponent<AudioSource>();
        instance = this;
        Init();
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = false;
        bgmPlayer.volume = bgmVolume;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            RandomPlay();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    // Player가 On이고, GameResult가 OFF면 게임진행중이니 다시 랜덤 노래 재생
    public void Update()
    {
        if (Need_GameResult_Obj.gameObject.activeInHierarchy == false
            && Need_Player_Obj.gameObject.activeInHierarchy == true
            && !bgmPlayer.isPlaying)
        {
            RandomPlay();
        }
    }

    // 노래 랜덤재생 함수
    public void RandomPlay()
    {
        int RanNum = Random.Range(0, bgmClip.Count);

        while (RanNum == LastIndex)
        {
            RanNum = Random.Range(0, bgmClip.Count);
        }

        bgmPlayer.clip = bgmClip[RanNum];
        LastIndex = RanNum;

        bgmPlayer.Play();
    }
}