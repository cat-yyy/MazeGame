using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [Header("生成するエネミーの数")]
    [SerializeField] private int numberOfEnemies;

    //private Dictionary<string, Entity> entities;

    // Start is called before the first frame update
    void Start()
    {
      //  entities = new Dictionary<string, Entity>();
    }

    /*
    //キーの対応するエンティティの座標を返す
    public GridPoint GetEntityPoint(String name)
    {
        if (entities.TryGetValue(name, out Entity entity))
        {
            return entity.Point;
        }
        else
        {
            throw new Exception("キーが存在しない");
        }
    }
    */

    /// <summary>
    /// Grid上にエンティティを配置する処理
    /// </summary>
    public void InitializeEntities(GridManager gridManager)
    {
        //Playerの配置
        GridPoint player = GetRandomFloorPoint(gridManager);
        gridManager.SetSquare(player,Square.Player);

        //エネミーの配置
        for (int i = 0; i < numberOfEnemies; i++)
        {
           GridPoint enemy= GetRandomFloorPoint(gridManager);
            gridManager.SetSquare(enemy,Square.Enemy);
        }

        //階段の配置
        GridPoint stairs= GetRandomFloorPoint(gridManager);
        gridManager.SetSquare(stairs,Square.Stairs);

    }

    //gird上にエンティティを配置する処理
    /*  public void InitializeGameObjects(GridManager gridManager)
      {
          //プレイヤーエンティティ生成
          var player = new Entity(GetRandomFloorPoint(gridManager), Square.Player);
          gridManager.SetSquare(player.Point, Square.Player);
          entities.Add("Player", player);

          //階段配置
          var stairs = new Entity(GetRandomFloorPoint(gridManager), Square.Stairs);
          gridManager.SetSquare(stairs.Point, Square.Stairs);
          entities.Add("Stairs", stairs);

          //エネミーエンティティ生成
          for (int i = 0; i < numberOfEnemies; i++)
          {
              var enemy = new Entity(GetRandomFloorPoint(gridManager), Square.Enemy);
              gridManager.SetSquare(enemy.Point, Square.Enemy);
              entities.Add("Enemy" + i, enemy);
          }
      }
    */

    /// <summary>
    /// ランダムで1つの床の座標を取得 
    /// </summary>
    GridPoint GetRandomFloorPoint(GridManager gridManager)
    {
        List<GridPoint> availableFloorPoints = new List<GridPoint>();

        //空いている床の座標をリストに格納
        for (int z = 1; z < gridManager.height - 1; z++)
        {
            for (int x = 1; x < gridManager.width - 1; x++)
            {
                if (gridManager.GetSquare(new GridPoint(x,z)) == Square.Floor)
                {
                    availableFloorPoints.Add(new GridPoint(x, z));
                }
            }
        }

        //床が見つからなかった場合
        if (availableFloorPoints.Count == 0)
        {
            throw new Exception("空いている床がありません");
        }

        return availableFloorPoints[UnityEngine.Random.Range(0, availableFloorPoints.Count)];
    }

    /*
    //entities辞書を返却
    public Dictionary<string, Entity>  GetEntities()
    {
        return entities; 
    }
    */
}
