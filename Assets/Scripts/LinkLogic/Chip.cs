using System.Collections;
using UnityEngine;

public enum ChipColor
{
    blue,
    green,
    red,
    yellow
}

public class Chip : GridItem
{
    [Header("Game Settings")]
    public GameSettings gameSettings;
    private InputManager cursor;
    public ChipColor color;
    public bool isLinked = false;

    protected override void Awake()
    {
        base.Awake();
        cursor = InputManager.Instance;
    }

    public void SetLinked()
    {
        spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, spriteRenderer.color.a);
        this.Grow();
        isLinked = true;
    }
    public void ReleaseLinked()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
        this.Shrink();
        isLinked = false;
    }


    private void OnMouseDown()
    {
        if (gameSettings.isGameActive)
        {
            cursor.SelectFirst(this);
            SetLinked();
        }
    }

    private void OnMouseUp()
    {
        cursor.Reset();
        ReleaseLinked();
    }

    private void OnMouseEnter()
    {
        cursor.SelectSecond(this);
    }
}