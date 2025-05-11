using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI movesText;
    private LinkableGrid grid;
    private int score = 0;
    private int goalScore;
    private int moves;
    public int Score { get { return score; } }

    public event System.Action OnGameOver;
    public event System.Action OnLevelComplete;
    public event System.Action LevelEndBanner;
    private void Start()
    {
        grid = (LinkableGrid)LinkableGrid.Instance;
    }
    internal IEnumerator ResolveLink(List<Chip> toResolve, int atMove)
    {

        int addToScore = toResolve.Count;
        for (int i = 0; i < toResolve.Count; i++)
        {
            Chip block = toResolve[i];
            block.GetComponent<CircleCollider2D>().enabled = false;

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
        AddScore(addToScore);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
        moves--;
        movesText.text = moves.ToString();
        if (moves <= 0)
        {
            if (score >= goalScore)
            {
                OnLevelComplete?.Invoke();
            }
            else
            {
                OnGameOver?.Invoke();
            }
        }
        //trigger win and lose conditions here

    }

    internal void SetMaxMovesAndGoalScore(int moveCount, int goalScore)
    {
        moves = moveCount;
        movesText.text = moves.ToString();
        this.goalScore = goalScore;

    }
}
