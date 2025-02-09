using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowSpriteChanger : MonoBehaviour
{
    public int level;                                       
    public int weapon_num;                          
    
    public Sprite[] arrow;                                  
    SpriteRenderer spriterenderer;                          

    private void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {    
        switch (weapon_num)
        {
            case 1:
                level = Weapon.HB_level;
                break;
            case 2:
                level = Weapon.SH_level;
                break;
        }

        if (level == 5)
        {
            spriterenderer.sprite = arrow[1];
        }
        else
        {
            spriterenderer.sprite = arrow[0];           
        }
    }
}
