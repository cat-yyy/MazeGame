using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// グリッドの状態を更新・制御するクラス
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("グリッドサイズ設定(9以上の奇数)")]
    public int width;
    public int height;

    [Header("生成するマスの一辺を値を入力")]
    public float onSideSquare;

    //インスペクターでシーン上のオブジェクトを指定
    [SerializeField] private EntityManager entityManager;

    private GridModel gridModel;
    private GridPoint stairsPoint;

    //GridModelのインスタンス生成＆gridの初期化
    void Start()
    {
        gridModel = new GridModel(width, height);
 
    }

    /// <summary>
    /// gridのentityをtargetPointに移動させる処理
    /// </summary>
    public void MoveObjectOnGrid(GridPoint current,GridPoint target)
    {
        //移動可能なマスかどうかを判定
        Debug.Log("呼び出し直後current:"+gridModel.GetSquare(current));
        Square targetSquare = gridModel.GetSquare(target);
        if (targetSquare != Square.Floor && targetSquare != Square.Stairs)
        {
            Debug.LogWarning("targetPointに移動できません");
            return;
        }

        //移動前のcurrentが階段だったかを記録
        bool wasOnStairs=(current==stairsPoint);

        //移動先が階段の場合階段位置を保存
        if (gridModel.GetSquare(target) == Square.Stairs)
        {
            stairsPoint = target;
        }
       
         gridModel.SetSquare(target,gridModel.GetSquare(current));

        //currentを階段に変更
        if (wasOnStairs)
        {
            gridModel.SetSquare(current, Square.Stairs);
        }
        else //それ以外の場合床に変更
        {
            gridModel.SetSquare(current, Square.Floor);
        }
        Debug.Log("呼び出し最後current:"+gridModel.GetSquare(current));
        
    }

  /*  //gridのentityをtargetPointに移動させる処理
    public void MoveObjectOnGrid(Entity entity, GridPoint targetPoint)
    {
        if (gridModel.GetSquare(targetPoint) != Square.Floor)
        {
            Debug.LogWarning("targetPointに移動できません");
            return;
        }

        //階段エンティティが保持するPointと現在地一致する場合、現在の位置をStairsに変更
        //それ以外の場合、現在の位置をFloorに戻し、移動先にエンティティのタイプをセット
        if (gridModel.GetSquare(entityManager.GetEntities()["Stairs"].Point) == gridModel.GetSquare(entity.Point))
        {
            gridModel.SetSquare(entity.Point, Square.Stairs);
        }
        else
        {
        gridModel.SetSquare(entity.Point, Square.Floor);
        }
        gridModel.SetSquare(targetPoint, entity.Type);

        //エンティティの位置を更新
        entity.Point = targetPoint;
    }
  */

    //壁かどうかチェック
    public bool IsWall(GridPoint point)
    {
        return gridModel.grid[point.Z][point.X] == Square.Wall;
    }


    //gridのpointにおける要素を第3引数に変更する
    public void SetSquare(GridPoint point, Square square)
    {
        gridModel.SetSquare(point, square); 
    }

    //gridのpointにおける要素を返す
    public Square GetSquare(GridPoint point)
    {
        return gridModel.GetSquare(point);
    }



}

