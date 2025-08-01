using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LANGenerator : MonoBehaviour
{
    [SerializeField] private GameObject straightLANPrefab;
    [SerializeField] private GameObject curveLANPrefab;
    [SerializeField] private GameObject LANGroup; //生成したLANをグループとしてまとめるための変数
    [SerializeField] private GridManager gridManager;
    public List<List<int>> playerTrack; //プレイヤートラック
    private Stack<GridPoint> stack; //プレイヤーのトラックをスタック
    private Stack<GameObject> LANStack;
    private bool isReturn;
    private int totalSteps;


    /// <summary>
    /// インスタンス生成時に呼ぶ初期化処理
    /// </summary>

    void Start()
    {
        playerTrack = Enumerable.Range(0, gridManager.height)
    .Select(height => Enumerable.Repeat(0, gridManager.width).ToList()).ToList();

        stack = new Stack<GridPoint>();   //stack初期化
        LANStack = new Stack<GameObject>();
        LANGroup = GameObject.Find("LANGroup");


    }

    /// <summary>
    /// オブジェクト生成および生成したオブジェクトをスタックに格納
    /// </summary>
    public void GenerateLAN(Vector3 target, Vector3 playerPoint, Vector3 playerToward)
    {
        //正規化
        Vector3 aNormalized = (target - playerPoint).normalized;
        Vector3 bNormalized = playerToward.normalized;

        //aからbへの0°～ 180° 範囲で回転角を求める
        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(aNormalized, bNormalized), -1f, 1f)) * Mathf.Rad2Deg;
        //回転方向を求める y>0なら左回転、y<0なら右回転
        if (Vector3.Cross(aNormalized, bNormalized).y < 0)
        {
            angle = -angle;
        }

        GameObject generatedObject;

        if (!isReturn)
        {
            //angle=0の場合(プレイヤーが直進)
            if (angle == 0)
            {

                if (playerToward.x == 0)//プレイヤーの向きがz軸と平行の場合
                {
                    generatedObject = GameObject.Instantiate(straightLANPrefab, playerPoint, Quaternion.identity);
                }
                else //プレイヤーの向きがx軸と平行の場合
                {
                    generatedObject = GameObject.Instantiate(straightLANPrefab, playerPoint, Quaternion.Euler(0, 90f, 0));

                }
            }
            else if (angle == 90)
            {
                generatedObject = GameObject.Instantiate(curveLANPrefab, playerPoint, Quaternion.Euler(0, angle, 0));
            }
            else
            {
                generatedObject = GameObject.Instantiate(curveLANPrefab, playerPoint, Quaternion.Euler(0, angle, 0));

            }
        //生成したオブジェクトをグループにまとめる
        generatedObject.transform.parent = LANGroup.transform;

        //生成したオブジェクトの参照をスタックする
        LANStack.Push(generatedObject);
        }
        else
        {
            Destroy(LANStack.Peek());
            LANStack.Pop();
        }


        /*
        targetVector,currentVectorとおく
        1 targetVectorとcurrentVectorのオイラー角が0度の場合
        そのベクトルと平行な直線のLANオブジェクトをtargetVector配置

        2 targetVectorとcurrentVectorのオイラー角がn*180度の場合
        currenVectorにあるLANオブジェクトを破棄＆そのベクトルと平行な直線のLANを配置

        3 targetVectorとcurrentVectorのオイラー角がn*90度の場合
        垂直なLANをオブジェクトをcurrentVectorに配置
        tagetVectorにはtargetVectorと平行な直線LANを配置

        */
    }

    /// <summary>
    /// プレイヤーが歩いたマスをスタックに保存する関数
    /// プレイヤーの進行方向と反対に移動するとスタックのトップを削除する
    /// </summary>

    public void TrackPlayer(GridPoint targetPoint, PlayerController player)
    {
        //現在のマス
        GridPoint currentPoint = player.CurrentPoint;
        //次に移動するマス
        GridPoint nextPoint = targetPoint;
        if (stack.Count > 0)
        {
        }
        //次へ進むマスがスタックのトップでなければplayerTrackを1加算
        if (stack.Count <= 0 || nextPoint != stack.Peek())
        {
            stack.Push(currentPoint);
            playerTrack[nextPoint.Z][nextPoint.X]++;
            isReturn = false; 
            totalSteps++;
        }
        else //次へ進むマスがスタックのトップにあればplayerTrackを1減らす
        {
            playerTrack[currentPoint.Z][currentPoint.X]--;
            stack.Pop();
            isReturn = true;
            totalSteps--;
        }

    }

    /// <summary>
    /// 現在の歩数を返す
    /// </summary>
    public int GetTotalSteps()
    {
        return totalSteps;
    }

    

}
