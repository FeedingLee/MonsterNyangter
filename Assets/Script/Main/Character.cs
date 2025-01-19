using UnityEngine;

public class Character : MonoBehaviour
{    
    public static float Speed           // 이동속도 증가 : 이동속도 관련 옵션
    {
        get
        {
            switch (GameManager.instance.playerId)
            {
                case 0: return 1.0f;
                case 1: return 1.2f;
                case 2: return 0.75f;
                case 3: return 1.5f;
                default: return 1.0f;
            }
        }
    }

    public static float WeaponSpeed     // 스테미나 증가 : 공격속도와 랜스의 돌진유지 시간 관련
    {
        get
        {
            switch (GameManager.instance.playerId)
            {
                case 0: return 1.0f;
                case 1: return 0.8f;
                case 2: return 1.0f;
                case 3: return 0.3f;
                default: return 1.0f;
            }
        }
    }

    public static float WeaponRate      // 쿨타임 감소 : 투사체 발사 속도, 공격 딜레이 감소
    {
        get
        {
            switch (GameManager.instance.playerId)
            {
                case 0: return 1.0f;
                case 1: return 0.8f;
                case 2: return 1.0f;
                case 3: return 1.0f;
                default: return 1.0f;
            }
        }
    }

    public static float Damage          // 공격력 증가 : 전체적인 데미지 관련
    {
        get
        {
            switch (GameManager.instance.playerId)
            {
                case 0: return 1.0f;
                case 1: return 0.9f;
                case 2: return 1.3f;
                case 3: return 0.6f;
                default: return 1.0f;
            }
        }
    }
}
