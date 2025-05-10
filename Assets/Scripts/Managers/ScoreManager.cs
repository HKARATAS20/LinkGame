using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    private LinkableGrid grid;
    private int score = 0;
    public int Score { get { return score; } }
    private void Start()
    {
        grid = (LinkableGrid)LinkableGrid.Instance;
    }
    internal IEnumerator ResolveBlast(List<Chip> toResolve, int atMove)
    {
        AddScore(toResolve.Count);

        for (int i = 0; i < toResolve.Count; i++)
        {
            Chip block = toResolve[i];
            block.GetComponent<BoxCollider2D>().enabled = false;

            if (i == toResolve.Count - 1)
            {
                yield return StartCoroutine(block.Resolve(block.transform));
            }
            else
            {

                StartCoroutine(block.Resolve(block.transform));
            }
        }

        StartCoroutine(grid.CollapseAndFillGrid());

    }

    public void AddScore(int amount)
    {
        score += amount;

    }

}
