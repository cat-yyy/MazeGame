using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;


/// <summary>
/// ターン制ゲームの進行を管理するクラス。
/// プレイヤーとエネミーの行動、3D空間での移動処理、ゲームオーバーなどの状態遷移を制御する。
/// ステートマシンを用いてゲームの各フェーズ（入力待ち、プレイヤーのターン、エネミーのターン、
/// 移動開始、移動中、ゲームオーバー）を管理し、GridManagerやPlayerController等と連携する。
/// </summary>
public class TurnManager
{
    private GridPoint playerTarget;//Grid上のプレイヤーが次へ進む座標
    private GridPoint[] enemiyTargets;//Grid上のエネミーが次へ進む座標
    private enum TurnState
    {
        Inputing,
        PlayerTurn,
        EnemyTurn,
        StartMoving,
        MovingState,
        StageClear,
        GameOver,
    }
    private TurnState currentState;
    private PlayerController playerController;
    private List<EnemyController> enemyControllers;
    private KeyManager keyManager;

    /// <summary>
    /// ゲームオーバーかどうかの判定
    /// </summary>
    public bool IsGameOver { get; set; }


    public TurnManager(ObjectGenerator objectGenerator)
    {
        //プレイヤーターンで開始
        currentState = TurnState.Inputing;
        //プレイヤーのコンポーネントの参照を取得
        playerController = objectGenerator.GetPlayerObject().GetComponent<PlayerController>();
        enemyControllers = new List<EnemyController>();
        List<GameObject> enemies = objectGenerator.GetEnemiesObject();
        //エネミーのコンポーネントの参照を取得
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyControllers.Add(enemies[i].GetComponent<EnemyController>());
        }
        enemiyTargets = new GridPoint[enemyControllers.Count];
        keyManager = new KeyManager();
    }

    public void TurnSystem(GridManager gridManager)
    {
        switch (currentState)
        {
            case TurnState.Inputing:
                GridPoint key = keyManager.InputKey();
                //入力があるまでステートを維持
                if (key != new GridPoint(0, 0))
                {
                    playerTarget = new GridPoint(key.X + playerController.CurrentPoint.X, key.Z + playerController.CurrentPoint.Z);
                    currentState = TurnState.PlayerTurn;
                }
                break;

            case TurnState.PlayerTurn:

                //Grid上でプレイヤーを移動させる
                playerTarget = playerController.MoveEntityOnGrid(playerTarget);

                //プレイヤーがその場にと止まる行動をしていたらInputStateに戻す
                if (playerController.IsStay)
                {
                    currentState = TurnState.Inputing;
                }
                //プレイヤーがエネミーのマスに進んだ場合
                else if (playerController.IsDead)
                {
                    currentState = TurnState.GameOver;
                }
                else
                {
                    currentState = TurnState.EnemyTurn;
                }
                break;

            case TurnState.EnemyTurn:

                bool isBreak = false;
                //エネミーのGrid上の移動先を保存する
                for (int i = 0; i < enemiyTargets.Length; i++)
                {
                    enemiyTargets[i] = enemyControllers[i].MoveEntityOnGrid();
                    if (playerController.IsDeadForEnemy)
                    {
                        currentState = TurnState.GameOver;
                        isBreak = true;

                        // プレイヤーの3D空間での移動を開始
                        playerController.MoveEntityIn3D(playerTarget);
                        break;
                    }
                }

                if (isBreak)
                {
                    break;
                }

                //3D空間の移動を行うターンへ遷移
                currentState = TurnState.StartMoving;
                break;

            //このステート開始時点ではGrid上の現在地と3D空間上の現在地は一致していない
            case TurnState.StartMoving:
                Debug.Log("currentState:MovingState");

                // プレイヤーおよびエネミーの3D空間での移動を開始
                playerController.MoveEntityIn3D(playerTarget);
                for (int i = 0; i < enemiyTargets.Length; i++)
                {
                    enemyControllers[i].MoveEntityIn3D(enemiyTargets[i]);
                }
                currentState = TurnState.MovingState;
                break;

            case TurnState.MovingState:
                //移動中(IsMoving==true)のオブジェクトがないかチェックを行う
                bool checkMoving = playerController.IsMoving;
                bool[] enemyConditions = new bool[enemyControllers.Count];
                for (int i = 0; i < enemyConditions.Length; i++)
                {
                    enemyConditions[i] = enemyControllers[i].IsMoving;
                    checkMoving |= enemyConditions[i];
                    Debug.Log("enemyConditions[i]" + enemyConditions[i]);
                }

                //すべてのオブジェクトが3D空間での移動が完了したらInputStateに戻す
                if (!checkMoving)
                {
                    currentState = TurnState.Inputing;
                }
                break;

            case TurnState.GameOver:

                 //プレイヤーの移動アニメーションが終わるまで待機
                if (playerController.IsMoving)
                {
                    return;
                }

                //プレイヤー死亡モーション開始
                playerController.DeathAnim();
                Debug.Log("ゲームオーバーだよ");
                IsGameOver = true;
                break;
        }
    }
}
