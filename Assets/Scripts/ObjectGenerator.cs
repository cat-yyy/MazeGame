using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField]
    GridManager gridManager;
    [SerializeField] //インスペクターで床を指定
    private GameObject[] floorPrefabs;
    [SerializeField]//インスペクターで壁を指定
    private GameObject[] wallPrefabs;
    [SerializeField]//インスペクターでプレイヤーを指定
    private GameObject playerPrefab;
    [SerializeField]//インスペクターでエネミーを指定
    private GameObject[] enemyPrefabs;
    [SerializeField]//インスペクター階段を指定
    private GameObject stairs;
    [SerializeField]
    private GameObject floorGroup;
    [SerializeField]
    private GameObject wallGroup;
    //生成したエネミーのオブジェクトの参照を保管する
    private List<GameObject> enemies;
    //生成したプレイヤーのオブジェクトの参照を取得
    private GameObject player;
    // Start is called before the first frame update
    void Awake()
    {
        enemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Gridデータに基づき床および壁を配置する
    public void GenerateSquares()
    {
        Vector3 currentPosition = new Vector3(0.0f, 0.0f, 0.0f); //生成するブロックの現在座標
        for (int z = 0; z < gridManager.height; z++)
        {
            for (int x = 0; x < gridManager.width; x++)
            {
                //壁を配置
                if (gridManager.GetSquare(new GridPoint(x, z)) == (Square.Wall))
                {
                    GameObject squareClone = Instantiate(wallPrefabs[0], currentPosition, Quaternion.identity);
                    squareClone.name += "(" + x + "," + z + ")";//オブジェクトの名前を分かりやすく変更
                    squareClone.transform.parent = wallGroup.transform;//親オブジェクトに入る
                    SetToSameScaleOfXAndZ(squareClone);
                }
                //床を配置
                else
                {
                    GameObject squareClone = Instantiate(floorPrefabs[0], currentPosition, Quaternion.identity);
                    squareClone.name += "(" + x + "," + z + ")"; //オブジェクトの名前を分かりやすく変更
                    squareClone.transform.parent = floorGroup.transform;//親オブジェクトに入る
                    SetToSameScaleOfXAndZ(squareClone);
                }

                //プレイヤー配置
                if (gridManager.GetSquare(new GridPoint(x, z)) == Square.Player)
                {
                    GameObject player = Instantiate(playerPrefab, currentPosition, Quaternion.identity);
                    //生成したインスタンスの参照をフィールドに保存
                    this.player = player;
                }
                //エネミーを配置
                else if (gridManager.GetSquare(new GridPoint(x, z)) == Square.Enemy)
                {
                    int randIndex = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                    GameObject enemy = Instantiate(enemyPrefabs[randIndex], currentPosition, Quaternion.identity);
                    //生成したインスタンスの参照をフィールドに保存
                    enemies.Add(enemy);
                }
                //階段を配置
                else if (gridManager.GetSquare(new GridPoint(x, z)) == Square.Stairs)
                {
                    GameObject squareClone = Instantiate(stairs, currentPosition, Quaternion.identity);
                }
                /*else
                {
                    Debug.LogError("無効なSquareの値:"+ gridManager.GetSquare(new GridPoint(x, z)));
                }
                */

                currentPosition.x += gridManager.onSideSquare; //現在座標を更新
            }
            currentPosition.x = 0.0f;
            currentPosition.z -= gridManager.onSideSquare;
        }
    }


    //マスのクローンオブジェクトのスケールのx,zを同期させる処理
    public void SetToSameScaleOfXAndZ(GameObject squareClone)
    {
        Vector3 modifiedScale = squareClone.transform.localScale;
        modifiedScale.x = gridManager.onSideSquare;
        modifiedScale.z = gridManager.onSideSquare;

        //壁の場合は高さも調整
        if (squareClone.tag == "Wall")
        {
            modifiedScale.y = gridManager.onSideSquare*0.7f;
        }
        squareClone.transform.localScale = modifiedScale;
    }

    //プレイヤーオブジェクトへの参照を取得
    public GameObject GetPlayerObject()
    {
        return player;
    }

    //エネミーオブジェクトへの参照を取得
    public List<GameObject> GetEnemiesObject()
    {
        return enemies;
    }
}
