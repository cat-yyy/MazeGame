using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageCreator;

/// <summary>
/// 2次元の迷路を生成するクラス
/// </summary>
public class MazeGenerator
{


    //深さ優先探索でランダムに迷路を生成する
    public void GenerateMaze(GridManager gridManager)
    {
        //迷路生成のための開始地点をランダムに設定する
        int startX = UnityEngine.Random.Range(1, gridManager.width - 1) / 2 * 2 + 1;
        int startZ = UnityEngine.Random.Range(1, gridManager.height - 1) / 2 * 2 + 1;

        //迷路生成の開始地点を床に設定
        gridManager.SetSquare(new GridPoint(startX, startZ), Square.Floor);

        //深さ優先探索にスタックを使用してする
        var stack = new Stack<GridPoint>();
        stack.Push(new GridPoint(startX, startZ));

        //移動する方向をの配列に格納
        var directions = new List<GridPoint>
        {
            new GridPoint(0,-2), //↑方向
            new GridPoint(0,2), //↓方向
            new GridPoint(-2,0), //←方向
            new GridPoint(2,0) //→方向
        };

        //スタックにがなくなるまで、壁を床に変更する処理を進める
        while (stack.Count > 0)
        {
            GridPoint currentPoint = stack.Peek(); //現在地を更新
            stack.Pop(); //スタックのトップを削除

            //移動先の候補を保存するためのneighborsリスト
            var neighbors = new List<GridPoint>();
            for (int i = 0; i < 4; i++)
            {
                int neighborX = currentPoint.X + directions[i].X;
                int neighborZ = currentPoint.Z + directions[i].Z;
                GridPoint neighborPoint = new GridPoint(neighborX, neighborZ);

                //(neighborX,neighborY)が迷路の内側かつ壁ならリストに追加
                if (neighborX > 0 && neighborZ > 0 && neighborX < gridManager.width - 1 && neighborZ < gridManager.height - 1
                    && gridManager.GetSquare(neighborPoint) == Square.Wall)
                {
                    neighbors.Add(new GridPoint(neighborX, neighborZ));
                }

                if (neighbors.Count > 0)
                {
                    //追加したリストからランダムで次へ進むマスを決める
                    int randIndex = UnityEngine.Random.Range(0, neighbors.Count);
                    int nextX = neighbors[randIndex].X;
                    int nextZ = neighbors[randIndex].Z;

                    //次へ進むマスおよび現在地との中間地点を掘る
                    gridManager.SetSquare(new GridPoint(nextX, nextZ), Square.Floor);
                    gridManager.SetSquare(new GridPoint((nextX + currentPoint.X) / 2, (nextZ + currentPoint.Z) / 2), Square.Floor);

                    //現在地、次へ進むマスの順でスタックする
                    stack.Push(currentPoint);
                    stack.Push(new GridPoint(nextX, nextZ));
                }

            }

        }

    }

    //生成した迷路にループ箇所を追加する
    public void AddLoop(GridManager gridManager)
    {
        //迷路の大きさに比例してループ箇所を何回作るか
        int counts = (int)(MathF.Sqrt(gridManager.width * gridManager.height) / 2);
        //ループ生成
        for (int i = 0; i < counts; i++)
        {
            int count = 0;
            //壁を壊してもよいマスがでるまで繰り返す
            while (count < gridManager.height * gridManager.width)
            {
                count++;

                int randX = UnityEngine.Random.Range(1, gridManager.width - 1);
                int randZ = UnityEngine.Random.Range(1, gridManager.height - 1);
                GridPoint randPoint = new GridPoint(randX, randZ);

                bool conditionA = gridManager.IsWall(new GridPoint(randX, randZ - 1)) && gridManager.IsWall(new GridPoint(randX, randZ + 1)); //上下が壁
                bool conditionB = gridManager.IsWall(new GridPoint(randX - 1, randZ)) && gridManager.IsWall(new GridPoint(randX + 1, randZ)); //左右が壁
                bool conditionC = !gridManager.IsWall(new GridPoint(randX, randZ - 1)) && !gridManager.IsWall(new GridPoint(randX, randZ + 1)); //上下ともに壁ではない
                bool conditionD = !gridManager.IsWall(new GridPoint(randX - 1, randZ)) && !gridManager.IsWall(new GridPoint(randX + 1, randZ)); //左右がともに壁ではない
                //壁を壊してもよい条件その1
                if ((randX == 1 || randX == gridManager.width - 2) && (randZ == 1 || randZ == gridManager.height - 2) && gridManager.IsWall(randPoint))
                {
                    gridManager.SetSquare(randPoint, Square.Floor);
                    break;

                    //壁を壊してもよい条件その2
                }
                else if ((conditionA && conditionD) ^ (conditionB && conditionC) && gridManager.IsWall(randPoint))
                {
                    gridManager.SetSquare(randPoint, Square.Floor);
                    break;
                }
            }

        }

    }//addLoop

}
