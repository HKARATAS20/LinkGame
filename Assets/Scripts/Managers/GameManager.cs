using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private LinkableGrid grid;
    private PoolManager pool;
    Vector2Int dimensions = Vector2Int.one * 8;
    private int moveCount = 20;

    void Start()
    {
        GridSetup();
        PoolSetup();
        StartCoroutine(grid.PopulateGrid());
    }

    private void GridSetup()
    {

        grid = (LinkableGrid)LinkableGrid.Instance;

        grid.gameObject.transform.position = new Vector2(-(dimensions.x - 1) / 2f, (-(dimensions.y - 1) / 2f) - 2);


        grid.InitializeGrid(dimensions);
        grid.SetValues(moveCount);

    }

    private void PoolSetup()
    {
        pool = PoolManager.Instance;

        pool.gameObject.transform.position = new Vector2(-(dimensions.x - 1) / 2f, (-(dimensions.y - 1) / 2f) - 2);

        pool.PoolAllObjects(dimensions.x * dimensions.y / 2);
    }

}
