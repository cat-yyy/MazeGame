using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// マスの状態を表すenum型
/// </summary>
public enum Square
{
    Floor = 0,
    Wall = 1,
    Player = 2,
    Enemy = 3,
    Stairs = 4,
    Nothing = -1
}

/// <summary>
/// 迷路のGridのデータモデル
/// </summary>
public class GridModel
{
    //平面におけるマスの状態を保持
    public List<List<Square>> grid;

    public GridModel(int width, int height)
    {
        InitializeGrid(width, height);
    }


    /// <summary>
    /// gridの初期化
    /// </summary>
    public void InitializeGrid(int width, int height)
    {
        //各要素はすべて壁で設定
        grid = Enumerable.Range(0, height)
            .Select(height => Enumerable.Repeat(Square.Wall, width).ToList()).ToList();

    }

    /// <summary>
    /// gridのpointにおける要素を第3引数に変更する
    /// </summary>
    public void SetSquare(GridPoint point, Square square)
    {
        grid[point.Z][point.X] = square;
    }

    /// <summary>
    //gridのpointにおける要素を返す
    /// </summary>
    public Square GetSquare(GridPoint point)
    {
        return grid[point.Z][point.X];
    }

}

/// <summary>
/// xz平面図(xが横軸、zが縦軸)における座標(x,z)をフィールドとして持つクラス
/// </summary>
public struct GridPoint
{
    public int X;
    public int Z;

    public GridPoint(int x, int z)
    {
        this.X = x;
        this.Z = z;
    }

    public override string ToString()
    {
        return $"GridPoint(X:{X},Z:{Z})";
    }

    public override bool Equals(object obj)
    {
        if (obj is GridPoint gridpoint)
        {
            return (X == gridpoint.X) && (Z == gridpoint.Z);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    public static bool operator ==(GridPoint left, GridPoint right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GridPoint left, GridPoint right)
    {
        return !(left == right);
    }
}
