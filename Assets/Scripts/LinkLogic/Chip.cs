using System.Collections;
using UnityEngine;

public enum ChipColor
{
    blue,
    green,
    red,
    yellow
}
public class Chip : Movable
{
    public Vector2Int Position { get; set; }
    protected SpriteRenderer spriteRenderer;
    //[SerializeField] protected ParticleSystem particles;

    private LinkableGrid grid;
    private InputManager cursor;

    private PoolManager pool;
    public ChipColor color;


    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //SetupParticles();
        grid = (LinkableGrid)LinkableGrid.Instance;
        cursor = InputManager.Instance;
        pool = PoolManager.Instance;
    }
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }


    public virtual IEnumerator Resolve(Transform collectionPoint, bool shouldAnimate = false)
    {
        if (shouldAnimate)
        {
            if (collectionPoint != transform)
            {
                yield return StartCoroutine(MoveToTransform(collectionPoint));
            }
        }
        else
        {
            //PlayParticleEffects();
        }
        Chip thisChip = grid.RemoveItemAt(this.Position);
        pool.ReturnBlock(thisChip);

    }

    private void OnMouseDown()
    {
        cursor.SelectFirst(this);
    }

    //  when the player releases the click, select nothing
    private void OnMouseUp()
    {
        cursor.Reset();
    }

    //  when the player drags the mouse, select this as the second selected
    //  (if using a mouse, this is actually called on every entry, even if they're not dragging, but cursor will filter this behaviour out)
    private void OnMouseEnter()
    {
        cursor.SelectSecond(this);
    }



}

/*
    public virtual void SetupParticles()
    {
        if (particles != null)
        {
            particles = Instantiate(particles, transform);
            particles.gameObject.SetActive(false);
        }
    }
    */

/*
    public virtual void PlayParticleEffects()
    {

        particles.transform.position = transform.position;
        particles.gameObject.SetActive(true);
        particles.Play();

    }
    */