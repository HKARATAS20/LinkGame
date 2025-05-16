using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LinkableGrid : GridSystem<Chip>
{

    private bool initialPopulation = true;
    private Vector3 offScreenOffset;
    private PoolManager pool;
    public GameObject gridBackgroundTile;

    public void InitializeGridValues(Vector2Int dimensions)
    {
        base.InitializeGrid(dimensions);
        pool = PoolManager.Instance;
        offScreenOffset = new Vector3(0, 17, 0);
    }

    public IEnumerator PopulateGrid()
    {
        List<Chip> newBlocks = new();

        Vector3 onScreenPosition;


        for (int y = 0; y < Dimensions.y; ++y)
        {
            for (int x = 0; x < Dimensions.x; ++x)
            {

                if (IsEmpty(x, y))
                {
                    newBlocks.Add(PutChipOnGrid(x, y));
                    PutBackgroundTile(x, y);
                }
            }

        }
        for (int i = 0; i != newBlocks.Count; i++)
        {
            onScreenPosition = GridToWorldPosition(newBlocks[i].Position);

            if (i == newBlocks.Count - 1)
            {
                yield return StartCoroutine(newBlocks[i].MoveToPosition(onScreenPosition));
            }
            else
            {
                StartCoroutine(newBlocks[i].MoveToPosition(onScreenPosition));
            }
            if (initialPopulation)
                yield return new WaitForSeconds(0.05f);
        }
        initialPopulation = false;
        CheckPossibleMoves();

    }

    private Chip PutChipOnGrid(int x, int y)
    {
        Chip newChip = pool.GetRandomGridBlock();
        newChip.gameObject.SetActive(true);
        newChip.transform.position = GridToWorldPosition(x, y, offScreenOffset);
        newChip.transform.SetParent(transform, true);
        newChip.Position = new Vector2Int(x, y);
        PutItemAt(newChip, x, y);

        return newChip;
    }

    private void PutBackgroundTile(int x, int y)
    {
        Transform childTransform = transform.Find("Background");
        if (childTransform != null)
        {
            GameObject backgroundTile = Instantiate(
              gridBackgroundTile,
              GridToWorldPosition(x, y),
              Quaternion.identity,
              childTransform
          );
            backgroundTile.name = "BackgroundTile_" + x + "_" + y;
        }
        else
        {
            Debug.LogError("Child object not found!");
        }
    }

    public IEnumerator CollapseAndFillGrid()
    {
        CollapseGrid();
        yield return StartCoroutine(PopulateGrid());
    }

    private void CollapseGrid()
    {
        for (int x = 0; x < Dimensions.x; ++x)
        {
            for (int yEmpty = 0; yEmpty < Dimensions.y - 1; ++yEmpty)
            {
                if (IsEmpty(x, yEmpty))
                {
                    for (int yNotEmpty = yEmpty + 1; yNotEmpty < Dimensions.y; ++yNotEmpty)
                    {
                        var item = GetItemAt(x, yNotEmpty);

                        if (item != null)
                        {

                            if (item.Idle)
                            {
                                MoveChipToPosition(item, x, yEmpty);
                                break;
                            }
                        }

                    }
                }
            }
        }
    }

    private void MoveChipToPosition(Chip toMove, int x, int y)
    {
        if (!BoundsCheck(toMove.Position.x, toMove.Position.y))
            Debug.LogError("(" + toMove.Position.x + ", " + toMove.Position.y + ") is not on the grid.");

        if (!BoundsCheck(x, y))
            Debug.LogError("(" + x + ", " + y + ") is not on the grid.");

        if (!IsEmpty(x, y))
            return;

        Chip temp = RemoveItemAt(toMove.Position.x, toMove.Position.y);
        PutItemAt(temp, x, y);

        toMove.Position = new Vector2Int(x, y);

        StartCoroutine(toMove.MoveToPosition(GridToWorldPosition(x, y)));
    }


    private Link GetLinkedArea(Chip startChip)
    {
        Link link = new();
        HashSet<Vector2Int> visited = new();
        Queue<Chip> toCheck = new();

        toCheck.Enqueue(startChip);
        visited.Add(startChip.Position);
        link.AddChip(startChip);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        //Comment this direction array and uncomment the one below to allow diagonal linking

        //this doesnt allow diagonal links just finds them when checking for valid moves
        // Uncomment line 82 of Link.cs to allow diagonal linking

        // Vector2Int[] directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        // new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)};

        while (toCheck.Count > 0)
        {
            Chip current = toCheck.Dequeue();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPosition = current.Position + direction;

                if (BoundsCheck(neighborPosition) && !IsEmpty(neighborPosition) && !visited.Contains(neighborPosition))
                {
                    Chip neighbor = GetItemAt(neighborPosition);

                    if (neighbor.color == startChip.color && neighbor.Idle)
                    {
                        toCheck.Enqueue(neighbor);
                        visited.Add(neighborPosition);
                        link.AddChip(neighbor);
                        if (link.Count >= 3)
                        {
                            return link;
                        }
                    }
                }
            }
        }

        return link;
    }


    private bool ValidMoveExists()
    {
        for (int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
            {


                if (BoundsCheck(x, y) && !IsEmpty(x, y))
                {
                    Link link = GetLinkedArea(GetItemAt(x, y));
                    if (link.Count >= 3)
                    {
                        return true;
                    }
                }
            }
        return false;
    }

    public void CheckPossibleMoves()
    {
        if (!ValidMoveExists())
        {
            Debug.Log("No valid moves. Initiating shuffle sequence.");
            StartCoroutine(ShuffleGrid());
        }

    }

    private IEnumerator ShuffleGrid()
    {
        yield return new WaitForSeconds(0.2f);
        while (!WholeGridIdle())
        {
            yield return new WaitForSeconds(0.1f);
        }

        int attempts = 0;
        const int maxAttempts = 20;


        while (!ValidMoveExists())
        {
            attempts++;
            if (attempts > maxAttempts)
            {
                Debug.LogError("Max shuffle attempts reached! Board might be unsolvable.");
                ScoreManager.Instance.TriggerGameOver();
                yield break;
            }
            PerformOneLogicalShuffleIteration();
            yield return null;
        }
        yield return StartCoroutine(AnimateGridToCurrentLogicalState());

    }

    private void LogicalSwap(Chip chip1, Chip chip2)
    {
        if (chip1 == null || chip2 == null || chip1 == chip2) return;

        Vector2Int pos1 = chip1.Position;
        Vector2Int pos2 = chip2.Position;


        base.SwapItemsAt(pos1, pos2);


        chip1.Position = pos2;
        chip2.Position = pos1;
    }

    private void PerformOneLogicalShuffleIteration()
    {


        List<Vector2Int> firstHalfPositions = new();
        List<Vector2Int> secondHalfPositions = new();


        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                Vector2Int position = new(x, y);
                if (y < Dimensions.y / 2)
                {
                    firstHalfPositions.Add(position);
                }
                else
                {
                    secondHalfPositions.Add(position);
                }
            }
        }


        secondHalfPositions = secondHalfPositions.OrderBy(pos => UnityEngine.Random.value).ToList();
        int swapCount = Mathf.Min(firstHalfPositions.Count, secondHalfPositions.Count);
        for (int i = 0; i < swapCount; i++)
        {
            Vector2Int firstHalfPos = firstHalfPositions[i];
            Vector2Int secondHalfPos = secondHalfPositions[i];

            Chip firstChip = GetItemAt(firstHalfPos.x, firstHalfPos.y);
            Chip secondChip = GetItemAt(secondHalfPos.x, secondHalfPos.y);

            if (firstChip != null && secondChip != null)
            {
                LogicalSwap(firstChip, secondChip);
            }
        }
    }

    private IEnumerator AnimateGridToCurrentLogicalState()
    {
        List<Coroutine> moveCoroutines = new();
        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                Chip chip = GetItemAt(x, y);
                if (chip != null)
                {
                    Vector3 targetWorldPosition = GridToWorldPosition(x, y);
                    moveCoroutines.Add(StartCoroutine(chip.MoveToPosition(targetWorldPosition, false)));


                }
            }
        }
        foreach (Coroutine coroutine in moveCoroutines)
        {
            yield return coroutine;
        }
    }
    private bool WholeGridIdle()
    {
        for (int y = 0; y < Dimensions.y; y++)
        {
            for (int x = 0; x < Dimensions.x; x++)
            {
                if (!GetItemAt(x, y).Idle)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Vector3 GridToWorldPosition(int x, int y)
    {
        return transform.position + new Vector3(x, y);
    }

    public Vector3 GridToWorldPosition(int x, int y, Vector3 offset)
    {
        return transform.position + new Vector3(x, y) + offset;
    }

    public Vector3 GridToWorldPosition(Vector2Int pos)
    {
        return transform.position + new Vector3(pos.x, pos.y);
    }


}




