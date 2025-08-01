using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class OldPlayerController : MonoBehaviour
{
    //ランケーブル
    public GameObject straightLANCarble;
    public GameObject curveLANCarble;
    StageCreator stageCreator;  //ステージのパラメータを参照する為に必要
    public float moveSpeed;  //移動速度
    public List<List<int>> playerTrack; //プレイヤートラック
    private Stack<Tuple<int, int>> stack; //プレイヤーのトラックをスタック
    private Tuple<int, int> targetPoint; //移動先のGrid上の座標
    private Vector3 targetDirection; //移動先の3D上のベクトル
    private GameObject child; //回転させるプレイヤーオブジェクト
    private Vector3 inputVector; //入力したキーをVectorで保存するための変数
    private bool isMoving; //移動中判定
    private Animator animator;
    private float currentEulerY;
    private int objectIndex; //生成するオブジェクトをカウントするためのデバック用変数
    private GameObject LANGroup; //生成したLANをグループとしてまとめるための変数
    // Start is called before the first frame update

    void Start()
    {
        stageCreator = GameObject.Find("StageCreator").GetComponent<StageCreator>();
        child = this.transform.GetChild(0).gameObject;
        animator = child.GetComponent<Animator>();
        //playerTrackの初期化
        playerTrack = Enumerable.Range(0, stageCreator.height)
            .Select(height => Enumerable.Repeat(0, stageCreator.width).ToList()).ToList();
        //stack初期化
        stack = new Stack<Tuple<int, int>>();
        objectIndex = 0;
        LANGroup = GameObject.Find("LANGroup");
    }

    public string ShowPoint(Tuple<int, int> point)
    {
        string str = "(x,y)=(" + point.Item1 + "," + point.Item2 + ")";
        return str;
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa");
        Debug.Log("targetPosition:" + targetPosition + "    this.transform.position:" + this.transform.position);

        //プレイヤーを進行方向に回転させる
        var rotation = Quaternion.LookRotation(targetPosition - this.transform.position, Vector3.up);
        child.transform.rotation = rotation;
        //目標の位置まで距離を詰めていく
        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        isMoving = false;
        animator.SetTrigger("idelTrigger");
        Debug.Log("アニメーション終了");
    }


    //移動先のキー入力を受付 
    public void InputKey()
    {
        inputVector = Vector3.zero;

        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                inputVector.z = 1.0f;

            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                inputVector.z = -1.0f;

            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputVector.x = -1.0f;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                inputVector.x = 1.0f;
            }
        }
    }

    //移動先のキー情報を渡し,targetPointの値更新　移動先のgridの要素を返す
    public StageCreator.Square GetTargetElementOfGrid()
    {
        if (inputVector != Vector3.zero)
        {
            var playerPoint = stageCreator.GetPlayerPoint();
            Debug.Log("(inputVector.x,inputVector.z)=(" + inputVector.x + "," + inputVector.z + ")");
            Debug.Log("(playerPoint.Item1,playerPoint.Item2)=(" + playerPoint.Item1 + "," + playerPoint.Item2 + ")");
            targetPoint = Tuple.Create(playerPoint.Item1 + (int)inputVector.x, playerPoint.Item2 - (int)inputVector.z);
            StageCreator.Square targetElement = stageCreator.GetElementOfGrid(targetPoint.Item1, targetPoint.Item2);
            return targetElement;
        }
        return StageCreator.Square.Nothing;
    }

    //プレイヤーの歩いたマスを記録する
    public void TrackingPlayer(Tuple<int, int> targetPoint)
    {
        Debug.Log("①TrackingPlayer()");
        //現在のマス
        Tuple<int, int> currentPoint = stageCreator.GetPlayerPoint();
        //次に移動するマス
        Tuple<int, int> nextPoint = targetPoint;
        //次へ進むマスがスタックのトップでなければplayerTrackを1加算
        if (stack.Count > 0)
        {
            Debug.Log("stack.Peek:" + ShowPoint(stack.Peek()));
            Debug.Log("targetPoint:" + ShowPoint(nextPoint));
        }
        if (stack.Count <= 0 || (nextPoint.Item1 != stack.Peek().Item1 || nextPoint.Item2 != stack.Peek().Item2))
        {
            stack.Push(currentPoint);
            playerTrack[nextPoint.Item2][nextPoint.Item1]++;
            Debug.Log("②TrackingPlayer()");
        }
        else //次へ進むマスがスタックのトップにあればplayerTrackを1減らす
        {
            playerTrack[currentPoint.Item2][currentPoint.Item1]--;
            stack.Pop();
            //playerTrack[nextPoint.Item2][nextPoint.Item1]--;
            Debug.Log("③TrackingPlayer()");
        }

        Debug.Log("④TrackingPlayer()");

    }

    //Grid上でプレイヤーを動かす処理
    public void MovePlayerOnGrid()
    {
        int X = stageCreator.GetPlayerPoint().Item1;
        int Y = stageCreator.GetPlayerPoint().Item2;
        Debug.Log("プレイヤーTrack:" + playerTrack[Y][X]);
        TrackingPlayer(targetPoint);
        stageCreator.MigrateObjectOnGrid(stageCreator.GetPlayerPoint(), targetPoint, StageCreator.Square.Player);
    }

    //3D上でプレイヤーを移動させる処理
    public void MovePlayerIn3D()
    {

        //アニメーション開始前のプレイヤーの向き
        Vector3 currentV = child.transform.forward;
        Debug.Log("currentV:" + currentV);
        float currentEulerY = child.transform.localRotation.eulerAngles.y;
        Debug.Log("アニメーション開始:currentEulerY(" + currentEulerY + ")");
        animator.SetTrigger("runTrigger");
        inputVector *= stageCreator.onSideSquare;
        targetDirection = transform.position + inputVector;
        StartCoroutine(Move(targetDirection));

        //アニメーション開始後のプレイヤーの向き
        Vector3 targetV=child.transform.forward;
        var after = child.transform.localRotation.eulerAngles.y;
        Debug.Log("アニメーション開始後:after(" + after + ")");
        Debug.Log("回転されるY軸の角度：" + (currentEulerY - after));
        Tuple<int, int> playerPoint = stageCreator.GetPlayerPoint();
        //上下方向移動の場合のLANケーブル配置
        if (playerTrack[playerPoint.Item2][playerPoint.Item1] > 0)
        {
            

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
            float diff = currentEulerY - after;
            if (diff == 0)
            {
                Debug.Log("diff==0");
                var ob = GameObject.Instantiate(straightLANCarble, new Vector3(playerPoint.Item1 * stageCreator.onSideSquare,
               0.2f, -playerPoint.Item2 * stageCreator.onSideSquare), Quaternion.Euler(0.0f, after, 0.0f));
                ob.transform.parent = LANGroup.transform;
                ob.name += "[" + objectIndex + "]";
                objectIndex++;
            }
            //移動先と移動元でキャラクターが向いてる方向が異なる場合
            else
            {
                Debug.Log("diff!=0");
                //z軸方向の移動の場合
                if (inputVector.z != 0)
                {
                    
                    var ob = GameObject.Instantiate(straightLANCarble, new Vector3(playerPoint.Item1 * stageCreator.onSideSquare,
                          0.2f, -playerPoint.Item2 * stageCreator.onSideSquare), Quaternion.Euler(0.0f, after, 0.0f));
                    ob.transform.parent = LANGroup.transform;
                    ob.name += "[" + objectIndex + "]";
                    objectIndex++;
                }
                //x軸方向の移動の場合
                else
                {
                    var ob = GameObject.Instantiate(straightLANCarble, new Vector3(playerPoint.Item1 * stageCreator.onSideSquare,
                          0.2f, -playerPoint.Item2 * stageCreator.onSideSquare), Quaternion.Euler(0.0f, currentEulerY - after, 0.0f));
                    ob.transform.parent = LANGroup.transform;
                    ob.name += "[" + objectIndex + "]";
                    objectIndex++;
                }
            }
        }
        /* //左右方向移動の場合のLANケーブル配置
         else if (playerTrack[playerPoint.Item2][playerPoint.Item1] > 0 && Mathf.Abs(inputVector.x) > 0)
         {
             GameObject ob=GameObject.Instantiate(straightLANCarble, new Vector3(playerPoint.Item1 * stageCreator.onSideSquare,
                 0.2f, -playerPoint.Item2 * stageCreator.onSideSquare), Quaternion.Euler(0.0f,90.0f,0.0f));
         }
        */

    }

    /*if (inputVector != Vector3.zero)
   {
       var playerPoint = stageCreator.GetPlayerPoint();
       Debug.Log("(inputVector.x,inputVector.z)=(" + inputVector.x + "," + inputVector.z + ")");
       Debug.Log("(playerPoint.Item1,playerPoint.Item2)=(" + playerPoint.Item1 + "," + playerPoint.Item2 + ")");
       var targetPoint = Tuple.Create(playerPoint.Item1 + (int)inputVector.x, playerPoint.Item2 - (int)inputVector.z);
       return stageCreator.GetStateOfSquare(targetPoint.Item1, targetPoint.Item2);
    */
    /* //targetPointが壁なら移動無し
     if (stageCreator.IsWall(targetPoint.Item1, targetPoint.Item2))
     {

     }
     //エネミーの場合の処理
     else if (stageCreator.GetStateOfSquare(targetPoint.Item1, targetPoint.Item2) == 3)
     {

     }
     //移動できる場合
     else
     {
         //Grid上の移動を行う
         stageCreator.MigrateObjectOnGrid(playerPoint, targetPoint, StageCreator.Square.Player);
         //moveDirectionの値を更新
         v *= stageCreator.onSideSquare; 
         moveDirection = transform.position + v;
         return true;
     }

     */



}
