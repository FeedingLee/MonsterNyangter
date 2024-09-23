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
            SceneManager.LoadScene("Main");
        }
        else
        {
            Debug.Log("Player 오브젝트가 비활성화 상태입니다. 씬을 변경할 수 없습니다.");
        }
    }
}
