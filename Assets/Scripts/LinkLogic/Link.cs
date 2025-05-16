using System.Collections.Generic;
using UnityEngine;
public class Link
{
    private readonly List<Chip> chips;

    public List<Chip> Chips
    {
        get { return chips; }
    }
    public ChipColor Color { get; set; }
    public int Count
    {
        get { return chips.Count; }
    }

    public bool Contains(Chip toCompare)
    {
        return chips.Contains(toCompare);
    }

    public Link()
    {
        chips = new List<Chip>(10);
    }

    public Link(Chip original) : this()
    {
        AddChip(original);
    }
    public void AddChip(Chip toAdd)
    {
        chips.Add(toAdd);
    }

    public void ReleaseLastChip()
    {
        if (chips.Count < 2) return;

        Chip last = chips[^1];
        last.ReleaseLinked();
        chips.RemoveAt(chips.Count - 1);
    }

    public void ReleaseAllChips()
    {
        foreach (Chip chip in chips)
        {
            chip.ReleaseLinked();
        }
        chips.Clear();
    }

    /// <summary>
    /// Checks if the given chip can be added to the link.
    /// For now we only check if it is adjacent but addditional logic can be added to allow
    /// for diagonal linking as well.
    /// </summary>
    /// <param name="toSelect"></param>
    /// <returns></returns>
    public bool CanBeLinked(Chip toSelect)
    {
        if (toSelect == Chips[^1] || Chips.Contains(toSelect))
        {
            return false;
        }
        return Color == toSelect.color && AreAdjacent(toSelect, Chips[^1]);
    }


    private bool AreAdjacent(Chip a, Chip b)
    {
        int dx = Mathf.Abs(a.Position.x - b.Position.x);
        int dy = Mathf.Abs(a.Position.y - b.Position.y);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        //uncomment this line to allow diagonal linking
        //also modification is needed when calculating valid moves to shuffle the grid
        //return (dx == 1 && dy == 0) || (dx == 0 && dy == 1) || (dx == 1 && dy == 1);
    }


}
