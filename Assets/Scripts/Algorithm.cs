using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//便利なstaticメソッド
public class Algorithm
{
    /// <summary>
    /// shuffleアルゴリズム
    /// </summary>
    public static void Shuffle<T>(IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    /// <summary>
    /// GridPointをVector3に変換するアルゴリズム
    /// 第二引数はマスの一辺の長さ
    /// </summary>

    public static Vector3 ToVector3(GridPoint point,float length)
    {
        Vector3 vector = Vector3.zero;
        vector.x = point.X* length;
        vector.z = -point.Z* length;
        return vector;
    }

    /// <summary>
    /// Vector3をGridPointに変換するアルゴリズム
    /// </summary>

    public static GridPoint ToGridPoint(Vector3 vector3,float length)
    {
        return new GridPoint((int)(vector3.x / length), (int)Mathf.Abs(vector3.z / length));

    }
}
