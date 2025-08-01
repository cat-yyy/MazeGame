using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Unity.VisualScripting;

public class OldEnemyController : MonoBehaviour
{
    StageCreator stageCreator;  //ステージのパラメータを参照する為に必要
    public float moveSpeed;  //移動速度

    private Animator animator;
    Vector3 moveDirection = Vector3.zero;
    bool isMoving = false; //移動中判定
    private Tuple<int, int>[] movePoint =
    {
        Tuple.Create(0,-1),//↑方向
        Tuple.Create(0,1),//↓方向
        Tuple.Create(-1,0),//←方向
        Tuple.Create(1,0),//→方向
    };

    //グラフの各ノードに置けるgoalまでの最短経路を保存する配列
    int[] dist;
    //親ノードの記録用の配列
    int[] parent;
    List<int>[] graph;

    // Start is called before the first frame update
    void Start()
    {
        stageCreator = GameObject.Find("StageCreator").GetComponent<StageCreator>();
        InitiallizeGraph();
        animator=GetComponent<Animator>();
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;
        var rotation= Quaternion.LookRotation(targetPosition-this.transform.position,Vector3.up);
        this.transform.rotation = rotation;
        //目標の位置まで距離を詰めていく
        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        isMoving = false;
        animator.SetTrigger("idelTrigger"); //アニメーション遷移
    }

    void InitiallizeGraph()
    {
        int width=stageCreator.width;
        int height=stageCreator.height;

        //StageCreatorのgridを隣接リストのグラフに変換する
        graph = new List<int>[width*height];
        for (int i = 0; i < graph.Length; i++)
        {
            graph[i] = new List<int>();
        }

        //隣接リスト初期化
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x <width; x++)
            {
                if (!stageCreator.IsWall(x, y))
                {
                    if (!stageCreator.IsWall(x, y - 1)) //↑方向
                    {
                        graph[y * width + x].Add((y - 1) * width + x);
                    }
                    if (!stageCreator.IsWall(x, y + 1))//↓方向
                    {
                        graph[y * width + x].Add((y + 1) * width + x);
                    }
                    if (!stageCreator.IsWall(x - 1, y))//←方向
                    {
                        graph[y * width + x].Add(y * width + (x - 1));
                    }
                    if (!stageCreator.IsWall(x + 1, y))//→方向
                    {
                        graph[y * width + x].Add(y * width + (x + 1));
                    }

                }
            }
        }
    }

    public string MoveEnemyOnGrid()
    {
        if (!isMoving)
        {
            var enemyPoint = stageCreator.GetEnemyPoint();
            Vector3 v = Vector3.zero;
            /*ランダムで動くロジック
            Algorithm.Shuffle(movePoint); //配列をシャッフル
            for (int i = 0; i < movePoint.Length; i++)
            {
                var targetPoint = Tuple.Create(enemyPoint.Item1 + movePoint[i].Item1, enemyPoint.Item2 + movePoint[i].Item2);
                if (stageCreator.GetStateOfSquare(targetPoint.Item1, targetPoint.Item2) == 0) //床の場合
                {
                    //Grid上の移動を行う
                    stageCreator.MigrateObjectOnGrid(enemyPoint, targetPoint, StageCreator.Square.Enemy);
                    //moveDirectionの値を更新
                    moveDirection = new Vector3(targetPoint.Item1 * stageCreator.onSideSquare, 0.0f, -targetPoint.Item2 * stageCreator.onSideSquare);
                    break;
                }
            }
            */
            var targetPoint = ShortestPath(enemyPoint, stageCreator.GetPlayerPoint()); //最短経路の次の一手の座標を取得
            if (stageCreator.GetElementOfGrid(targetPoint.Item1,targetPoint.Item2)==StageCreator.Square.Player) //targetPointがプレイヤーの場合の処理
            {
                Debug.Log("アタック！！！！！！！！！！！！！！！！");
                return "GameOver";
            }
            else
            {
                stageCreator.MigrateObjectOnGrid(enemyPoint, targetPoint, StageCreator.Square.Enemy);//Grid上の移動を行う           
                moveDirection = new Vector3(targetPoint.Item1 * stageCreator.onSideSquare, 0.0f, -targetPoint.Item2 * stageCreator.onSideSquare); //moveDirectionの値を更新
            }

        }
        return "IsNotGameOver";
    }
    public void MoveEnemyIn3D()
    {
        Debug.Log("Enemyを3Dで動かすよ!!!!!!!!!!!!!!!!!!!!!!!!");
        animator.SetTrigger("walkTrigger"); //アニメーション遷移
        StartCoroutine(Move(moveDirection));
    }

    //startからgoalまでの最短経路を調べてその次の一手の座標を返す関数
    Tuple<int, int> ShortestPath(Tuple<int, int> start, Tuple<int, int> goal)
    {
        Debug.Log("rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
        int width = stageCreator.width;
        int height = stageCreator.height;
        int startX = start.Item1;
        int startY = start.Item2;
        int goalX = goal.Item1;
        int goalY = goal.Item2;

        dist=new int[width* height];
        parent = new int[width* height];
        Array.Fill(dist, -1);
        Array.Fill(parent, -1);

        //幅優先探索でキューを使用
        Queue<int> queue=new Queue<int>();

        //スタート地点に置ける最短経路は0
        dist[startY * width + startX] = 0;
        queue.Enqueue(startY * width + startX);

        //幅優先探索実行
        while(queue.Count > 0)
        {
            int currentPosition = queue.Peek();

            //ゴールにたどり着いたら探索を終える
            if (currentPosition == goalY * width + goalX)
            {
                break;
            }
            queue.Dequeue();
            for(int i = 0; i < graph[currentPosition].Count; i++)
            {
                int nextPosition = graph[currentPosition][i];
                if (dist[nextPosition] == -1)
                {
                    dist[nextPosition] = dist[currentPosition] + 1;
                    parent[nextPosition] = currentPosition;
                    queue.Enqueue(nextPosition);
                }
            }
        }

        //最短経路を復元
        List<Tuple<int, int>> path=new List<Tuple<int, int>>();
        int current = goalY * width + goalX;
        while (current != -1)
        {
            int x = current % width;
            int y = current / height;
            path.Add(Tuple.Create(x, y));
            current = parent[current];
        }
        Debug.Log("次の敵の一手は:(" + path[path.Count - 1].Item1 + "," + path[path.Count-1].Item2+")");
        return path[path.Count - 2];
 
    }


}
