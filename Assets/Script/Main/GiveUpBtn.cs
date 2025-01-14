using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GiveUpBtn : MonoBehaviour
{
    public GameObject player; // Player 오브젝트를 참조하기 위한 변수

    void Start()
    {
        // Player 오브젝트를 찾아서 변수에 할당
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
    }

    public void SceneChange()
    {
        // Player 오브젝트가 활성화된 상태일 때만 작동
        if (player != null && player.activeInHierarchy)
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
            // 경험치 보너스 상태 초기화
            Item.Exp_Bonus = 0;

            SceneManager.LoadScene("Main");
        }
        else
        {
            Debug.Log("Player 오브젝트가 비활성화 상태입니다. 씬을 변경할 수 없습니다.");
        }
    }
}
