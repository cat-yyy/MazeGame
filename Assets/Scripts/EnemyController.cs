using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// エネミーの制御を担うクラス
/// </summary>
public class EnemyController : EntityController
{

    private EnemyMovingLogic enemyMovingLogic;
    private PlayerController playerController;

    //エネミーの移動方向リスト
    private GridPoint[] movePoint =
            {
        new GridPoint(0,-1),//↑方向
        new GridPoint(0,1),//↓方向
        new GridPoint(-1,0),//←方向
        new GridPoint(1,0),//→方向
    };


    public override void Start()
    {
        gridManager=GameObject.Find("GridManager").GetComponent<GridManager>();
        enemyMovingLogic = new EnemyMovingLogic(gridManager);
        //PlayerController取得
        playerController = GameObject.Find("PlayerPrefab(Clone)").GetComponent<PlayerController>();
        animator=GetComponent<Animator>();
        //エネミーの現在地を取得
        CurrentPoint = Algorithm.ToGridPoint(transform.position, gridManager.onSideSquare);
    }

    void Update()
    {
        /*
        if (IsMoving)
        {
            return;
        }
        var point = MoveEntityOnGrid();
        MoveEntityIn3D(point);
        */
    }

    /// <summary>
    ///グリッド上でエネミーを移動させる処理 移動先の座標を返す
    /// </summary>
    public GridPoint MoveEntityOnGrid()
    {
        //プレイヤーの現在地取得
        Debug.Log("gridManager:" + gridManager +"playerController:"+playerController);
        GridPoint playerPoint = playerController.CurrentPoint;

        //プレイヤーの現在地までの最短経路となる次の一手の座標を取得
        GridPoint targetPoint = enemyMovingLogic.GetShortestPathStep(CurrentPoint, playerPoint, gridManager);

        //エネミーの座標を更新
        gridManager.MoveObjectOnGrid(CurrentPoint, targetPoint);

        //移動先がプレイヤーとなる場合
        if (targetPoint == playerPoint)
        {
            playerController.IsDeadForEnemy = true;
            return targetPoint;
        }
        else
        {
        //移動したらフラグ切り替え
        IsGridMoved = true;

        }

        //フィールドのエネミーの現在地更新
        CurrentPoint = targetPoint;

        return targetPoint;
    }

    /// <summary>
    /// 3D空間でエネミーを移動させる処理 引数は目標となるgridの座標を指定
    /// </summary>

    public override void MoveEntityIn3D(GridPoint targetPoint)
    {
        if (IsGridMoved && !IsMoving)
        {
            Debug.Log("Enemyを3Dで動かすよ!!!!!!!!!!!!!!!!!!!!!!!!");
            animator.SetTrigger("walkTrigger"); //アニメーション遷移
                                                //移動させる対象のベクトル
            Vector3 targetVector = Algorithm.ToVector3(targetPoint, gridManager.onSideSquare);
            StartCoroutine(Move(targetVector));
            animator.SetTrigger("runTrigger");
        }
    }
    
    /// <summary>
    /// コルーチン 3D空間で動かす際にアニメーション分処理を待機させるために使う
    /// </summary>
    public override IEnumerator Move(Vector3 targetPosition)
    {
        IsMoving = true;
        var rotation = Quaternion.LookRotation(targetPosition - this.transform.position, Vector3.up);
        this.transform.rotation = rotation;
        //目標の位置まで距離を詰めていく
        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        IsMoving = false;
        IsGridMoved = false;
        animator.SetTrigger("idelTrigger"); //アニメーション遷移
    }

    public void EnemyAttackAnim()
    {
        animator.SetTrigger("attackTrigger");
    }
}
