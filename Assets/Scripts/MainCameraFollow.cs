using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{

    [Header("PlayerからMainCameraまでの座標を指定ベクトル分移動")]
    [SerializeField] private Vector3 targetVector;
    [SerializeField] private float followSpeed;

    [SerializeField] private GameObject mainCamera; //インスペクターでシーン上の追従させるカメラを指定

    void Start()
    {
        mainCamera = Camera.main.gameObject; //シーン内のカメラオブジェクトを取得
        mainCamera.transform.parent = transform; //カメラの親をプレイヤーに設定
        mainCamera.transform.position = transform.position + targetVector;　//カメラの位置を指定ベクトル分移動
    }

    void LateUpdate()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position+targetVector, Time.deltaTime * followSpeed);
    }
}
