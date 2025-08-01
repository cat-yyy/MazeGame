using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{

    /// <summary>
    /// 3D上で移動中かどうかの判定プロパティ
    /// </summary>
    public bool IsMoving { get; set; }

    /// <summary>
    /// Grid上で移動が完了した場合のフラグ
    /// </summary>
    public bool IsGridMoved { get; set; }
    
    /// <summary>
    /// Grid上の現在地のプロパティ
    /// </summary>
    public GridPoint CurrentPoint { get; set; } 


    [SerializeField] protected float moveSpeed;  //移動速度
    protected Animator animator;
    protected GridManager gridManager;

    public abstract void Start();

    public abstract void MoveEntityIn3D(GridPoint targetPoint);

    public abstract IEnumerator Move(Vector3 targetPosition);
}

