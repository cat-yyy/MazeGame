using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// エネミーの行動ロジックを実装したクラス
/// </summary>
public class EnemyMovingLogic
{
    //gridを隣接リストのグラフに変換したものを格納する
    private List<int>[] graph;

    public EnemyMovingLogic(GridManager gridManager)
    {
        InitializeGraph(gridManager);
    }

    //初期化処理
    void InitializeGraph(GridManager gridManager)
    {
        int width = gridManager.width;
        int height = gridManager.height;
        //gridを隣接リストのグラフに変換する
        graph = new List<int>[width * height];
        for (int i = 0; i < graph.Length; i++)
        {
            graph[i] = new List<int>();
        }

        //隣接リスト初期化
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!gridManager.IsWall(new GridPoint(x, z)))
                {
                    if (!gridManager.IsWall(new GridPoint(x, z - 1)))//↑方向が壁で無ければ対象座標を隣接リストに追加
                    {
                        graph[z * width + x].Add((z - 1) * width + x);
                    }
                    if (!gridManager.IsWall(new GridPoint(x, z + 1)))//↓方向が壁で無ければ対象座標を隣接リストに追加
                    {
                        graph[z * width + x].Add((z + 1) * width + x);
                    }
                    if (!gridManager.IsWall(new GridPoint(x - 1, z)))//←方向が壁で無ければ対象座標を隣接リストに追加
                    {
                        graph[z * width + x].Add(z * width + (x - 1));
                    }
                    if (!gridManager.IsWall(new GridPoint(x + 1, z)))//→方向が壁で無ければ対象座標を隣接リストに追加
                    {
                        graph[z * width + x].Add(z * width + (x + 1));
                    }

                }
            }
        }
    }

    /// <summary>
    /// startからgoalまでの最短経路を調べてその次の一手の座標を返す関数
    /// </summary>
    public GridPoint GetShortestPathStep(GridPoint start, GridPoint goal,GridManager gridManager)
    {
        int width = gridManager.width;
        int height=gridManager.height;
        //グラフの各ノードに置けるgoalまでの最短経路を保存する配列
        int[] dist = new int[width * height];

        //親ノードの記録用の配列
        int[] parent = new int[width * height];



        Array.Fill(dist, -1); //各要素-1で初期化
        Array.Fill(parent, -1);//各要素-1で初期化

        //幅優先探索でキューを使用
        Queue<int> queue = new Queue<int>();

        //スタート地点に置ける最短経路は0
        dist[start.Z * width + start.X] = 0;
        queue.Enqueue(start.Z * width + start.X);

        //幅優先探索実行
        while (queue.Count > 0)
        {
            int currentPoint = queue.Peek();

            //ゴールにたどり着いたら探索を終える
            if (currentPoint == goal.Z * width + goal.X)
            {
                break;
            }
            queue.Dequeue();

            //currentPointノードと隣接しているノードを調べる
            for (int i = 0; i < graph[currentPoint].Count; i++)
            {
                int nextPoint = graph[currentPoint][i];
                //未探索のノードがあればそのノードのdistの値を更新
                //またそのノードの親ノードとしてcurretPointを設定
                if (dist[nextPoint] == -1)
                {
                    dist[nextPoint] = dist[currentPoint] + 1;
                    parent[nextPoint] = currentPoint;
                    queue.Enqueue(nextPoint);
                }
            }
        }

        //最短経路を復元
        List<GridPoint> path = new List<GridPoint>();
        int current = goal.Z * width + goal.X;
        while (current != -1)
        {
            int x = current % width;
            int z = current / height;
            path.Add(new GridPoint(x, z));
            current = parent[current];
        }
        Debug.Log("次の敵の一手は:(" + path[path.Count - 1].X + "," + path[path.Count - 1].Z + ")");
        return path[path.Count - 2];

    }

    /// <summary>
    ///ランダムな移動の次の一手を返す
    /// </summary>
    public GridPoint GetRandomStep(GridPoint currentPoint, GridManager gridManager)
    {
        GridPoint[] movePoint =
{
        new GridPoint(0,-1),//↑方向
        new GridPoint(0,1),//↓方向
        new GridPoint(-1,0),//←方向
        new GridPoint(1,0),//→方向
    };

        Algorithm.Shuffle(movePoint); //配列をシャッフル
        for (int i = 0; i < movePoint.Length; i++)
        {
            var targetPoint = new GridPoint(currentPoint.X + movePoint[i].X, currentPoint.Z + movePoint[i].Z);
            if (gridManager.GetSquare(targetPoint) == Square.Floor) //床の場合
            {
                return targetPoint;
            }
        }
        throw new Exception("GetRandomStep()内エラー:進めるマスがありません");
    }
}
