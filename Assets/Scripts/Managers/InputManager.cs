using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool cheatMode;

    private LinkableGrid grid;
    private SpriteRenderer spriteRenderer;

    private List<Chip> selected; // use List now

    [SerializeField]
    private Vector2 verticalStretch = new Vector2Int(1, 2),
                    horizontalStretch = new Vector2Int(2, 1);

    [SerializeField]
    private Vector3 halfUp = Vector3.up / 2,
                    halfDown = Vector3.down / 2,
                    halfLeft = Vector3.left / 2,
                    halfRight = Vector3.right / 2;

    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        selected = new List<Chip>();
    }

    private void Start()
    {
        grid = (LinkableGrid)LinkableGrid.Instance;
    }

    public void Reset()
    {
        if (selected.Count > 2)
        {
            StartCoroutine(ScoreManager.Instance.ResolveBlast(selected, 0));
        }
        selected.Clear();
        spriteRenderer.enabled = false;
    }

    public void SelectFirst(Chip toSelect)
    {
        selected.Clear();

        if (!enabled || toSelect == null)
            return;

        selected.Add(toSelect);

        transform.position = toSelect.transform.position;
        spriteRenderer.size = Vector2.one;
        spriteRenderer.enabled = true;
    }

    public void SelectSecond(Chip toSelect)
    {
        if (!enabled || toSelect == null || selected.Count == 0)
            return;

        Chip last = selected[selected.Count - 1];

        // If re-selecting the previous chip in the path (backtracking)
        if (selected.Count >= 2 && toSelect == selected[selected.Count - 2])
        {
            // Backtrack one step
            selected.RemoveAt(selected.Count - 1);
            spriteRenderer.enabled = selected.Count > 1; // Hide if not enough to show link
            return;
        }

        // Don't select the same chip twice unless it's for backtracking
        if (toSelect == last || selected.Contains(toSelect))
            return;

        if (AreAdjacent(last, toSelect) && last.color == toSelect.color)
        {
            selected.Add(toSelect);
            StretchCursorBetween(last, toSelect);
        }
    }


    private bool AreAdjacent(Chip a, Chip b)
    {
        int dx = Mathf.Abs(a.Position.x - b.Position.x);
        int dy = Mathf.Abs(a.Position.y - b.Position.y);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    private void StretchCursorBetween(Chip from, Chip to)
    {
        transform.position = from.transform.position;

        if (from.Position.x == to.Position.x)
        {
            spriteRenderer.size = verticalStretch;
            transform.position += (from.Position.y > to.Position.y) ? halfDown : halfUp;
        }
        else if (from.Position.y == to.Position.y)
        {
            spriteRenderer.size = horizontalStretch;
            transform.position += (from.Position.x > to.Position.x) ? halfLeft : halfRight;
        }
    }

}
