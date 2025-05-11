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
    public GameObject gridBackgroundTile;
    [SerializeField] private List<Chip> possibleMoves;


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
        for (int y = 0; y < Dimensions.y; ++y)
        {
            for (int x = 0; x < Dimensions.x; ++x)
            {

                if (IsEmpty(x, y))
                {
                    newBlocks.Add(PutChipOnGrid(x, y, order));
                    PutBackgroundTile(x, y);
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
        CheckPossibleMoves();

    }

    private Chip PutChipOnGrid(int x, int y, int order)
    {
        Chip newChip = pool.GetRandomGridBlock();
        newChip.gameObject.SetActive(true);
        newChip.transform.position = transform.position + new Vector3(x, order) + offScreenOffset;
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
            Vector3 basePosition = transform.position;
            GameObject backgroundTile = Instantiate(gridBackgroundTile, basePosition + new Vector3(x, y), Quaternion.identity, childTransform);
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

    /// <summary>
    /// Moves the given blastable to the given x and y coordinates on the grid.
    /// Should only be called if (x,y) on the grid is empty
    /// </summary>
    /// <param name="toMove">Blastable to be moved</param>
    /// <param name="x">X coordinate of the desired point</param>
    /// <param name="y">Y coordinate of the desired point</param>
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

        StartCoroutine(toMove.MoveToPosition(transform.position + new Vector3(x, y)));
    }


    private Link GetBlastedArea(Chip startChip)
    {
        Link link = new();
        HashSet<Vector2Int> visited = new();
        Queue<Chip> toCheck = new();

        toCheck.Enqueue(startChip);
        visited.Add(startChip.Position);
        link.AddChip(startChip);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
                    }
                }
            }
        }

        return link;
    }


    private bool ValidMoveExists()
    {
        possibleMoves = new List<Chip>();

        for (int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
            {


                if (BoundsCheck(x, y) && !IsEmpty(x, y))
                {
                    Link link = GetBlastedArea(GetItemAt(x, y));
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
                //trigger game over here

                yield break;
            }
            PerformOneLogicalShuffleIteration();
            yield return null;
        }
        Debug.Log($"Valid grid found after {attempts} logical shuffles.");


        Debug.Log("Animating grid to reflect new logical state.");
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

            Chip firstBlastable = GetItemAt(firstHalfPos.x, firstHalfPos.y);
            Chip secondBlastable = GetItemAt(secondHalfPos.x, secondHalfPos.y);

            if (firstBlastable != null && secondBlastable != null)
            {
                LogicalSwap(firstBlastable, secondBlastable);
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
                    Vector3 targetWorldPosition = transform.position + new Vector3(x, y);
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

}




