using System.Collections.Generic;
using UnityEngine;

/*
 * This class will allow organizing anything into a grid system
 * 
 * You must call InitializeGrid with grid dimensions first
 * The grid dimensions must be positive numbers
 */
public abstract class GridSystem<T> : Singleton<GridSystem<T>>
{
    private T[,] data;

    private Vector2Int dimensions = new Vector2Int(1, 1);

    public Vector2Int Dimensions
    {
        get
        {
            return dimensions;
        }
    }

    private bool isReady;

    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }

    public void InitializeGrid(Vector2Int dimensions)
    {
        if (dimensions.x < 1 || dimensions.y < 1)
            Debug.LogError("Grid dimensions must be positive numbers.");

        this.dimensions = dimensions;

        data = new T[dimensions.x, dimensions.y];

        isReady = true;
    }

    public void Clear()
    {
        data = new T[dimensions.x, dimensions.y];
    }


    public bool BoundsCheck(int x, int y)
    {
        if (!isReady)
            Debug.LogError("Grid has not been initialized.");

        return x >= 0 && x < dimensions.x && y >= 0 && y < dimensions.y;
    }
    public bool BoundsCheck(Vector2Int position)
    {
        return BoundsCheck(position.x, position.y);
    }


    public bool IsEmpty(int x, int y)
    {
        if (!BoundsCheck(x, y))
            Debug.LogError("(" + x + ", " + y + ") is not on the grid.");


        return EqualityComparer<T>.Default.Equals(data[x, y], default(T));
        ;
    }
    public bool IsEmpty(Vector2Int position)
    {
        return IsEmpty(position.x, position.y);
    }


    public bool PutItemAt(T item, int x, int y, bool allowOverwrite = false)
    {
        if (!BoundsCheck(x, y))
            Debug.LogError("(" + x + ", " + y + ") is not on the grid.");

        if (!allowOverwrite && !IsEmpty(x, y))
            return false;

        data[x, y] = item;
        return true;
    }
    public bool PutItemAt(T item, Vector2Int position, bool allowOverwrite = false)
    {
        return PutItemAt(item, position.x, position.y, allowOverwrite);
    }


    public T GetItemAt(int x, int y)
    {
        if (!BoundsCheck(x, y))
            Debug.LogError("(" + x + ", " + y + ") is not on the grid.");

        return data[x, y];
    }
    public T GetItemAt(Vector2Int position)
    {
        return GetItemAt(position.x, position.y);
    }


    public T RemoveItemAt(int x, int y)
    {
        if (!BoundsCheck(x, y))
            Debug.Log("(" + x + ", " + y + ") is not on the grid.");

        T temp = data[x, y];
        data[x, y] = default(T);
        return temp;
    }
    public T RemoveItemAt(Vector2Int position)
    {
        return RemoveItemAt(position.x, position.y);
    }


    public void SwapItemsAt(int x1, int y1, int x2, int y2)
    {
        if (!BoundsCheck(x1, y1))
            Debug.LogError("(" + x1 + ", " + y1 + ") is not on the grid.");

        if (!BoundsCheck(x2, y2))
            Debug.LogError("(" + x2 + ", " + y2 + ") is not on the grid.");

        T temp = data[x1, y1];
        data[x1, y1] = data[x2, y2];
        data[x2, y2] = temp;
    }
    public void SwapItemsAt(Vector2Int position1, Vector2Int position2)
    {
        SwapItemsAt(position1.x, position1.y, position2.x, position2.y);
    }


    public override string ToString()
    {
        string s = "";

        for (int y = dimensions.y - 1; y != -1; --y)
        {
            s += "[ ";

            for (int x = 0; x != dimensions.x; ++x)
            {
                if (IsEmpty(x, y))
                    s += "x";
                else
                    s += data[x, y].ToString();

                if (x != dimensions.x - 1)
                    s += ", ";
            }
            s += " ]\n";
        }
        return s;
    }
}