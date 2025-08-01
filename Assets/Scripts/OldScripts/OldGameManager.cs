using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class OldGameManager : MonoBehaviour
{

    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        MovingTurn,
        StageClear,
        GameOver,
        Pause
    }
    public StageCreator stageCreator;

    private GameState gameState = GameState.PlayerTurn;
    private OldPlayerController playerController;
    private OldEnemyController enemyController;
    private bool isPlayerMoving;

    public void GameStateMachine()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        //Playerコントローラーの取得処理
        if (playerController == null || enemyController == null)
        {
            playerController = GameObject.Find("PlayerPrefab(Clone)").GetComponent<OldPlayerController>();
            enemyController = GameObject.Find("EnemyPrefab(Clone)").GetComponent<OldEnemyController>();
        }
        if (playerController != null && enemyController)
        {
            Debug.Log(gameState);
            switch (gameState)
            {
                //Grid上のプレイヤーを移動
                case GameState.PlayerTurn:
                    playerController.InputKey(); //入力受付
                    StageCreator.Square targetElement = playerController.GetTargetElementOfGrid();
                    if (targetElement == StageCreator.Square.Floor)  //移動先が床の場合の処理
                    {
                        playerController.MovePlayerOnGrid();
                        gameState = GameState.EnemyTurn;
                    }
                    else if (targetElement == StageCreator.Square.Stairs)
                    {
                        playerController.MovePlayerOnGrid();
                        gameState = GameState.StageClear;
                    }
                    else if (targetElement == StageCreator.Square.Enemy)
                    {
                        Debug.Log("敵に接触");
                        gameState = GameState.Pause;
                    }
                    break;

                //Grid上のエネミーを移動
                case GameState.EnemyTurn:
                   Debug.Log("Grid上のエネミーを移動");
                   Debug.Log("enemyController:" + enemyController.ToString());


                   string str = enemyController.MoveEnemyOnGrid();
                    Debug.Log("str:" + str);
                    if (str.Equals("GameOver"))
                    {
                        Debug.Log("GameState:EnemyTurn  GameOverだ");
                        gameState = GameState.GameOver;
                        break;
                    }
                    
                   
                    gameState = GameState.MovingTurn;
                    break;
                   
                case GameState.MovingTurn:
                    playerController.MovePlayerIn3D();  //プレイヤーを3D上で移動させる
                    enemyController.MoveEnemyIn3D(); //エネミーを3D上で移動させる
                    gameState = GameState.PlayerTurn;
                    break;
                case GameState.StageClear:
                    playerController.MovePlayerIn3D();  //プレイヤーを3D上で移動させる
                    ResetScene.Reset();
                    break;
                case GameState.GameOver:
                    Debug.Log("GameOverだよ!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    playerController.MovePlayerIn3D();  //プレイヤーを3D上で移動させる
                    enemyController.MoveEnemyIn3D(); //エネミーを3D上で移動させる
                    gameState=GameState.Pause;
                    break;
                case GameState.Pause:

                    break;
                /*case GameState.GameOverP:
                    Debug.Log("GameOverだよ");
                    ResetScene.Reset();
                    break;
                */
            }
        }

    }

}
