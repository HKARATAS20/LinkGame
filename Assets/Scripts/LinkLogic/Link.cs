using System.Collections.Generic;
public class Link
{
    public bool fromPowerup;

    private readonly List<Chip> chips;
    public List<Chip> Chips
    {
        get { return chips; }
    }
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
}
