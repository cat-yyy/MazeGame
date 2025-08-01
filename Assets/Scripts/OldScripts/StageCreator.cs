using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

/* Todo
UIの実装
タイトル画面の実装
 
*/


//characterはInputsystem
//で入力を受け付ける

/*リファクタリングの要素
 ・enum型を他クラスでも参照出来たほうが可読性が上がる
 ・StageCreatorのスクリプトに処理が集中してしまっているため、分割する必要がある
 */
public class StageCreator : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject floorGroup;
    public GameObject wallGroup;
    public OldGameManager gameManager;

    public GameObject[] floorPrefabs;  //インスペクターで床を指定
    public GameObject[] wallPrefabs;  //インスペクターで壁を指定
    [Header("横何マスを生成するか9以上の奇数を指定")]
    public int width; //ステージが横何マスか　9以上の奇数を指定
    [Header("縦何マスを生成するか9以上の奇数を指定")]
    public int height; //ステージが縦何マスか 9以上の奇数を指定 
    [Header("生成するマスの一辺を値を入力")]
    public float onSideSquare;
    [Header("スポーンさせるオブジェクト一覧")]
    public GameObject[] spawnObjects;

    private DebugInfoController debugInfo = new DebugInfoController();
    private Tuple<int, int> currentPlayerPoint; //grid上のプレイヤーの現在座標
    private Tuple<int, int> currentEnemyPoint; //grid上のエネミーの現在座標
    private Tuple<int, int> stairsPoint;//grid上階段の座標
    private OldPlayerController playerController;

    //マス種類を列挙
    public enum Square
    {
        Floor = 0,
        Wall = 1,
        Player=2,
        Enemy=3,
        Stairs=4,
        Nothing=-1
    }
    List<List<Square>> grid; //2次元のマス

    //grid上でオブジェクトを移動させる処理
    public void MigrateObjectOnGrid(Tuple<int, int> current, Tuple<int, int> target,Square square)
    {
        SetGrid(current.Item1, current.Item2, Square.Floor);
        SetGrid(target.Item1 , target.Item2, square);
        if (square == Square.Player)
        {
            currentPlayerPoint = target;
        }else if(square == Square.Enemy)
        {
            currentEnemyPoint = target;
        }
    }

    //grid[y][x]を第3引数の要素に変更する
    public void SetGrid(int x, int y,Square square)
    {
        grid[y][x] = square;   
    }

    //grid[y][x]の要素を返す
    public Square GetElementOfGrid(int targetX,int targetY)
    {
        return (Square)grid[targetY][targetX];
    }

    //gridの2次元配列を返す
    public List<List<Square>> GetGrid()
    {
        return grid;
    }

   //プレイヤーの現在座標を取得
   public Tuple<int, int> GetPlayerPoint()
    {
       // Debug.Log("Playerのgrid上の位置:(" + playerPoint.Item1 + "," + playerPoint.Item2 + ")");
        return currentPlayerPoint;
    }

    //エネミーの現在座標を取得
   public Tuple<int,int> GetEnemyPoint()
    {
        return currentEnemyPoint; 
    }

    //階段の座標を取得
    public Tuple<int, int> GetStarisPoint()
    {
        return stairsPoint;
    }

    //(x,y)が壁かどうか判定する
    public bool IsWall(int x, int y)
    {
        return grid[y][x] == Square.Wall;
    }

    //深さ優先探索でランダムに迷路を生成する
    void GenerateMaze()
    {
        //gridの初期化　各要素はすべて壁で設定
        grid = Enumerable.Range(0,height)
            .Select(height=>Enumerable.Repeat(Square.Wall,width).ToList()).ToList();

        //迷路生成のための開始地点をランダムに設定する
        int startX = UnityEngine.Random.Range(1, width - 1) / 2 * 2 + 1;
        int startY = UnityEngine.Random.Range(1, height - 1) / 2 * 2 + 1;


        //迷路生成の開始地点を床に設定
        SetGrid(startX, startY,Square.Floor);

        //スタックを使用して探索
        var stack = new Stack<Tuple<int, int>>();
        stack.Push(Tuple.Create(startX, startY));

        //ランダム移動用の配列
        var directions = new List<Tuple<int, int>>
        {
            Tuple.Create(0,-2), //↑方向
            Tuple.Create(0,2), //↓方向
            Tuple.Create(-2,0), //←方向
            Tuple.Create(2,0) //→方向
        };

        //穴掘りスタート
        while (stack.Count > 0)
        {
            int currentX = stack.Peek().Item1; //現在地を更新
            int currentY = stack.Peek().Item2;
            stack.Pop();

            //移動先の候補を保存するためのneighborsリスト
            var neighbors = new List<Tuple<int, int>>();
            for (int i = 0; i < 4; i++)
            {
                int neighborX = currentX + directions[i].Item1;
                int neighborY = currentY + directions[i].Item2;

                //(neighborX,neighborY)が迷路の内側かつ壁ならリストに追加
                if (neighborX > 0 && neighborY > 0 && neighborX < width - 1 && neighborY < height - 1
                    && IsWall(neighborX, neighborY))
                {
                    neighbors.Add(Tuple.Create(neighborX, neighborY));
                }

                if (neighbors.Count > 0)
                {
                    //追加したリストからランダムで次へ進むマスを決める
                    int randIndex = UnityEngine.Random.Range(0, neighbors.Count);
                    int nextX = neighbors[randIndex].Item1;
                    int nextY = neighbors[randIndex].Item2;

                    //次へ進むマスおよび現在地との中間地点を掘る
                    SetGrid(nextX, nextY, Square.Floor);
                    SetGrid((nextX + currentX) / 2, (nextY + currentY) / 2,Square.Floor);

                    //現在地、次へ進むマスの順でスタックする
                    stack.Push(Tuple.Create(currentX, currentY));
                    stack.Push(Tuple.Create(nextX, nextY));
                }

            }

        }


    }

    //生成した迷路にループ箇所を追加する
    void AddLoop()
    {
        Debug.Log("addLoop:①");

        //迷路の大きさに比例してループ箇所を何回作るか
        int counts = (int)(MathF.Sqrt(width * height) / 2);
        Debug.Log(counts);

        //ループ生成
        for (int i = 0; i < counts; i++)
        {
            Debug.Log("addLoop:②");

            int count = 0;
            //壁を壊してもよいマスがでるまで繰り返す
            while (count<height*width)
            {
                count++;

                int randX = UnityEngine.Random.Range(1, width - 1);
                int randY = UnityEngine.Random.Range(1, height - 1);

                bool conditionA = IsWall(randX, randY - 1) && IsWall(randX, randY + 1); //上下が壁
                bool conditionB = IsWall(randX - 1, randY) && IsWall(randX + 1, randY); //左右が壁
                bool conditionC = !IsWall(randX, randY - 1) && !IsWall(randX, randY + 1); //上下ともに壁ではない
                bool conditionD = !IsWall(randX - 1, randY) && !IsWall(randX + 1, randY); //左右がともに壁ではない
                //壁を壊してもよい条件その1
                if ((randX == 1 || randX == width - 2) && (randY == 1 || randY == height - 2) && IsWall(randX, randY))
                {
                    SetGrid(randX, randY, Square.Floor);
                    break;

                    //壁を壊してもよい条件その2
                }
                else if ((conditionA && conditionD) ^ (conditionB && conditionC) && IsWall(randX, randY))
                {
                    SetGrid(randX, randY, Square.Floor);
                    break;
                }
            }

        }

    }//addLoop

    //生成したgridデータからランダムで床のマスの座標を取得 
    Tuple<int,int> RandomPointOfFloor()
    {
        int i = 0;
        int X, Y;
        //床の座標が選ばれるまで繰り返す
        while (i<Mathf.Pow(width*height,2)) {
            ++i;
            X = UnityEngine.Random.Range(1, width - 1);
            Y = UnityEngine.Random.Range(1, height - 1);
            if (grid[Y][X]==0)
            {
                return new Tuple<int, int>(X, Y);
                
            }
         }

        throw new Exception("RandomPointOfFloor()で例外がスロー");
    }

    //マスのクローンオブジェクトのスケールのx,zを同期させる処理
    void SetToSameScaleOfXAandZ(GameObject squareClone)
    {
        Vector3 modifiedScale = squareClone.transform.localScale;
        modifiedScale.x = onSideSquare;
        modifiedScale.z = onSideSquare;
        squareClone.transform.localScale = modifiedScale;
    }

    //迷路データに基づき床および壁を配置する
    void GenerateSquares()
    {
        Vector3 currentPosition = new Vector3(0.0f, 0.0f, 0.0f); //生成するブロックの現在座標
        for (int i = 0; i < grid.Count; i++)
        {
            for (int j = 0; j < grid[i].Count; j++)
            {
                if (grid[i][j] == (int)(Square.Floor))
                {
                    GameObject squareClone = Instantiate(floorPrefabs[0], currentPosition, Quaternion.identity);
                    squareClone.name += "(" + j + "," + i + ")";
                    squareClone.transform.parent = floorGroup.transform;
                    SetToSameScaleOfXAandZ(squareClone);
                }
                else if (grid[i][j] == Square.Wall)
                {
                    GameObject squareClone = Instantiate(wallPrefabs[0], currentPosition, Quaternion.identity);
                    squareClone.name += "(" + j + "," + i + ")";
                    squareClone.transform.parent = wallGroup.transform;
                    SetToSameScaleOfXAandZ(squareClone);
                }
                currentPosition.x += onSideSquare; //現在座標を更新
            }
            currentPosition.x = 0.0f;
            currentPosition.z -= onSideSquare;
        }
    }

    //プレイヤー、エネミー、階段等のオブジェクトをスポーンさせる
    void SpawnObjects()
    {
        


        for(int i = 0; i < spawnObjects.Length; i++)
         {
             Vector3 currentPosition=new Vector3(0.0f,0.0f,0.0f);
             var spawnPoint = RandomPointOfFloor();
             currentPosition.x = spawnPoint.Item1 * onSideSquare;
             currentPosition.z=-spawnPoint.Item2 * onSideSquare;
             GameObject spawnObjectClone = Instantiate(spawnObjects[i], currentPosition, Quaternion.identity);
             //gridデータの更新
             if (i == 0)  //プレイヤー
             {
                 SetGrid(spawnPoint.Item1, spawnPoint.Item2, Square.Player);
                currentPlayerPoint = spawnPoint;  //プレイヤーの現在座標更新
             }

            if (i == 1)  //エネミー
             {
                 SetGrid(spawnPoint.Item1, spawnPoint.Item2, Square.Enemy);
                currentEnemyPoint = spawnPoint;  //エネミーの現在座標更新
             }
            if (i == 2) //階段
            {
                SetGrid(spawnPoint.Item1,spawnPoint.Item2, Square.Stairs);
                stairsPoint = spawnPoint;
            }

         }
    }


    void Start()
    {
        //迷路のデータを生成
        GenerateMaze();
        AddLoop();
       

        //生成データに基づき3Dデータを配置する
        GenerateSquares();
        SpawnObjects();
        playerController=GameObject.Find("PlayerPrefab(Clone)").GetComponent<OldPlayerController>();
        if (playerController != null)
        {
            Debug.Log("Start() で playerController を取得しました。");
        }
        else
        {
            Debug.LogError("Start() で playerController を取得できませんでした。");
        }
        //メインカメラをスポーン
        /* Vector3 positionOfCamera = GameObject.Find("Player(Clone)").transform.position;
         if(positionOfCamera == null)
         {
             throw new Exception();
         }
         positionOfCamera.y = 3.0f;
         Instantiate(mainCamera, positionOfCamera, Quaternion.identity);
         Debug.Log("positionOfCamera.y:" + positionOfCamera.y);
        */

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Maze()メソッド呼び出し");
        //debugInfo.Maze(grid,playerController);
        //GetPointOfPlayer();
       
    }
}