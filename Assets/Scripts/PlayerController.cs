using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// グリッドベースのマップ上でプレイヤーの移動や状態管理を行うコントローラ。
/// プレイヤーの移動処理、アニメーション制御、LANの生成、敵との接触処理などを担当する
/// </summary>
public class PlayerController : EntityController
{
    private GameObject child; //回転させるプレイヤーの子オブジェクト
    private LANGenerator lanGenerator;


    /// <summary>
    ///プレイヤーが移動していないかどうかの確認プロパティ
    /// </summary>
    public bool IsStay { get; set; }

    /// <summary>
    /// プレイヤーが敵に接触しに行ったかどうかのプロパティ
    /// </summary>
    public bool IsDead { get; set; }

    /// <summary>
    /// エネミーにより攻撃を受けたかどうかのプロパティ
    /// </summary>
    public bool IsDeadForEnemy { get; set; }

    public override void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        //各種参照を取得
        child = this.transform.GetChild(0).gameObject;
        animator = child.GetComponent<Animator>();
        lanGenerator = GameObject.Find("LANGenerator").GetComponent<LANGenerator>();

        //Grid上のプレイヤーの初期位置をセット
        base.CurrentPoint = Algorithm.ToGridPoint(transform.position, gridManager.onSideSquare);
    }

    private void Update()
    {


    }

    /// <summary>
    ///グリッド上でプレイヤーを移動させる処理 移動先の座標を返す
    /// </summary>
    public GridPoint MoveEntityOnGrid(GridPoint targetPoint)
    {

        //targetが床の場合移動実行
        if (gridManager.GetSquare(targetPoint) == Square.Floor)
        {
            lanGenerator.TrackPlayer(targetPoint, this); //プレイヤーの足跡をスタック
            IsStay = false;

            //移動実行
            gridManager.MoveObjectOnGrid(base.CurrentPoint, targetPoint);
            //プレイヤーの現在地更新
            base.CurrentPoint = targetPoint;

            //無事移動出来たらフラグを切り替え
            IsGridMoved = true;

        }
        else if (gridManager.GetSquare(targetPoint) == Square.Enemy) //敵の場合はゲームオーバー処理へ進める
        {
            IsDead = true;
            IsStay = false;
        }
        else //上記以外の場合は何もしない
        {
            IsStay = true;
        }

        return targetPoint;

    }
    /// <summary>
    /// 3D空間上でプレイヤーを移動させる
    /// </summary>
    /// <param name="targetPoint"></param>
    public override void MoveEntityIn3D(GridPoint targetPoint)
    {
        //Grid上の移動が完了しており、3D上で移動中で無ければ実行
        if (!IsMoving && IsGridMoved)
        {

            //アニメーション開始前のプレイヤーの向き
            Vector3 playerForward = child.transform.forward;

            //移動させる目標のベクトル
            Vector3 targetVector = Algorithm.ToVector3(targetPoint, gridManager.onSideSquare);

            //LANオブジェクト配置
            lanGenerator.GenerateLAN(targetVector, transform.position, playerForward);

            animator.SetTrigger("runTrigger");
            StartCoroutine(Move(targetVector));
        }

    }

    /// <summary>
    /// 移動時先まで非同期処理でゆっくり移動させる
    /// </summary>
    public override IEnumerator Move(Vector3 targetPosition)
    {
        IsMoving = true;

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
        IsMoving = false;
        IsGridMoved = false;

        //アニメーション切り替え
        animator.ResetTrigger("runTrigger");
        animator.SetTrigger("idelTrigger");
    }

    public LANGenerator GetLANConnector()
    {
        return lanGenerator;
    }

    public void DeathAnim()
    {
        animator.SetTrigger("deathTrigger");
    }
}

