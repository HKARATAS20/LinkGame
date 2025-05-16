using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{

    private LinkableGrid grid;
    private Link currentLink;

    protected override void Init()
    {
        currentLink = new Link();
    }

    private void Start()
    {
        grid = (LinkableGrid)LinkableGrid.Instance;
    }

    public void Reset()
    {
        if (currentLink.Count > 2)
        {
            StartCoroutine(ScoreManager.Instance.ResolveLink(currentLink.Chips, 0));
        }

        currentLink.ReleaseAllChips();
    }

    public void SelectFirst(Chip toSelect)
    {
        if (!enabled || toSelect == null)
            return;

        currentLink = new Link(toSelect)
        {
            Color = toSelect.color
        };
    }

    public void SelectSecond(Chip toSelect)
    {
        if (!enabled || toSelect == null || currentLink.Count == 0)
            return;


        if (currentLink.Count >= 2 && toSelect == currentLink.Chips[^2])
        {
            currentLink.ReleaseLastChip();//we are backtracking so release the last chip
            return;
        }


        if (currentLink.CanBeLinked(toSelect))
        {
            currentLink.AddChip(toSelect);
            toSelect.SetLinked();
        }
    }



}
