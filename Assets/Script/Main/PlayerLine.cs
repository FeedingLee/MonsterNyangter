using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLine : MonoBehaviour
{
    public GameObject Player;
    Vector2 PlayerVec;

    void Update()
    {
        ChangePosition();
    }

    void ChangePosition()
    {
        PlayerVec = Player.transform.position;
        transform.position = PlayerVec;
    }
}
