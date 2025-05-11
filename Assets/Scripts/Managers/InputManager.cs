using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool cheatMode;

    private LinkableGrid grid;


    private List<Chip> selected; // use List now


    protected override void Init()
    {


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
            StartCoroutine(ScoreManager.Instance.ResolveLink(selected, 0));
        }
        foreach (Chip chip in selected)
        {
            chip.ReleaseLinked();
        }
        selected.Clear();

    }

    public void SelectFirst(Chip toSelect)
    {
        selected.Clear();

        if (!enabled || toSelect == null)
            return;

        selected.Add(toSelect);

    }

    public void SelectSecond(Chip toSelect)
    {
        if (!enabled || toSelect == null || selected.Count == 0)
            return;

        Chip last = selected[^1];

        // If re-selecting the previous chip in the path (backtracking)
        if (selected.Count >= 2 && toSelect == selected[^2])
        {
            // Backtrack one step
            selected[^1].ReleaseLinked();
            selected.RemoveAt(selected.Count - 1);
            return;
        }

        // Don't select the same chip twice unless it's for backtracking
        if (toSelect == last || selected.Contains(toSelect))
            return;

        if (AreAdjacent(last, toSelect) && last.color == toSelect.color)
        {
            selected.Add(toSelect);
            toSelect.SetLinked();
        }
    }


    private bool AreAdjacent(Chip a, Chip b)
    {
        int dx = Mathf.Abs(a.Position.x - b.Position.x);
        int dy = Mathf.Abs(a.Position.y - b.Position.y);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

}
