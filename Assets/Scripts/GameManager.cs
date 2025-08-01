using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体の初期化と進行管理の中心的な役割を担うクラス
/// </summary>
public class GameManager : MonoBehaviour
{

    //GridManager→MazeGenerator→EntityManager→ObjectGeneratorの順番
    
    /// <summary>
    /// Gameのステートを表すenum型
    /// </summary>
    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        MovingTurn,
        StageClear,
        GameOver,
        Pause
    }

    //インスペクターで各シーン上のオブジェクトを指定
    [SerializeField] private GridManager gridManager;
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private ObjectGenerator objectGenerator;
    [SerializeField] private DebugInfoController debugInfo;
    [SerializeField] private LANGenerator lanGenerator;
    [SerializeField] private MazeUIManager mazeUIManager;

    private TurnManager turnManager;
    private MazeGenerator mazeGenerator;

    // Start is called before the first frame update
    void Start()
    {
        mazeGenerator =new MazeGenerator();
        mazeGenerator.GenerateMaze(gridManager);
        mazeGenerator.AddLoop(gridManager);
        entityManager.InitializeEntities(gridManager);
        objectGenerator.GenerateSquares();
        turnManager = new TurnManager(objectGenerator);
        debugInfo.InitializeDebugInfo();
       // lanGenerator.Initialize(gridManager);
    }

    private void Update()
    {
        //ゲームオーバーが確認出来たらボタンパネルをアクティブにする
        if (turnManager.IsGameOver)
        {
            mazeUIManager.ExecuteButtonPanel(true);
            return;
        }
        turnManager.TurnSystem(gridManager);

        debugInfo.Maze(gridManager);
    }
}
