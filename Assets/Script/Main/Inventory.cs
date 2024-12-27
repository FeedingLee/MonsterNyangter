using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class Inventory : MonoBehaviour
{
    Text textLevel;                                         // 텍스트를 받아올 변수
    public Object weapon_data;                              // 무기의 상태를 가지고있는, Weapon 오브젝트 받아오기
    public int level;                                       // 레벨을 저장

    void Start()                                            // 최초 실행시
    {
        Text[] text = GetComponentsInChildren<Text>();      // 자식오브젝트중 'Text'를 가진 컴포넌트를 찾아 Text의 배열에 넣는다
        textLevel = text[0];                                // 가장 첫번째로 찾아진 Text는 Level을 담당하는 TextLevel이다
    }
    void Update()                                           
    {   
        if (level != weapon_data.GetComponent<Item>().level) // 데이터 오브젝트가 레벨이 올랐는데, 현재 level과 다르다면 실행
        { 
            level = weapon_data.GetComponent<Item>().level;  // Level 변수에, 오브젝트로 연결한 무기의 현재 Level을 넣음
            textLevel.text = "Lv." + (level);               
        }
    }
}
