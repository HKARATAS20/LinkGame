using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LinkableGrid : GridSystem<Chip>
{

    private int moves;
    private bool initialPopulation = true;
    private Vector3 offScreenOffset;
    private PoolManager pool;




    public void SetValues(int maxMoves)
    {
        pool = PoolManager.Instance;
        offScreenOffset = new Vector3(0, 16, 0);
        moves = maxMoves;
        //movesText.text = "" + moves;
        //SetGridBackground();
    }
    /*
        private void SetGridBackground()
        {
            GameObject gridBackground = transform.Find("grid_background").gameObject;
            SpriteRenderer gridBackgroundRenderer = gridBackground.GetComponent<SpriteRenderer>();
            gridBackgroundRenderer.size = new Vector2(Dimensions.x + 0.3f, Dimensions.y + 0.5f);
            gridBackgroundRenderer.transform.position += new Vector3(-transform.position.x, -transform.position.y, 0);
            gridBackground.SetActive(true);
        }
    */

    public IEnumerator PopulateGrid()
    {
        List<Chip> newBlocks = new();

        Vector3 onScreenPosition;

        int order = 0;
        for (int y = 0; y < Dimensions.x; ++y)
        {
            for (int x = 0; x < Dimensions.x; ++x)
            {

                if (IsEmpty(x, y))
                {
                    newBlocks.Add(PutChipOnGrid(x, y, order));
                }
            }
            order++;
        }
        for (int i = 0; i != newBlocks.Count; i++)
        {
            onScreenPosition = transform.position + new Vector3(newBlocks[i].Position.x, newBlocks[i].Position.y);

            if (i == newBlocks.Count - 1)
            {
                yield return StartCoroutine(newBlocks[i].MoveToPosition(onScreenPosition));
            }
            else
            {
                StartCoroutine(newBlocks[i].MoveToPosition(onScreenPosition));
            }
            if (initialPopulation)
                yield return new WaitForSeconds(0.1f);
        }
        initialPopulation = false;

    }

    private Chip PutChipOnGrid(int x, int y, int order)
    {
        Debug.Log("Putting chip on grid at: " + x + ", " + y);
        Chip newChip = pool.GetRandomGridBlock();
        newChip.gameObject.SetActive(true);
        newChip.transform.position = transform.position + new Vector3(x, order) + offScreenOffset;
        newChip.transform.SetParent(transform, true);
        newChip.Position = new Vector2Int(x, y);
        PutItemAt(newChip, x, y);
        return newChip;
    }


}