using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動のキー入力を受け付けるクラス
/// </summary>
public class KeyManager
{
    //移動先のキー入力を受付 
    public GridPoint InputKey()
    {
        GridPoint input = new GridPoint(0, 0);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            input.Z = -1;

        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            input.Z = 1;

        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            input.X = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            input.X = 1;
        }
        else
        {
            
        }



        return input;
    }

}

