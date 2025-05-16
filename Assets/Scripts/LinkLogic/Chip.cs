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
    [Header("Game Settings")]
    public GameSettings gameSettings;
    public Vector2Int Position { get; set; }
    protected SpriteRenderer spriteRenderer;
    //[SerializeField] protected ParticleSystem particles;

    private LinkableGrid grid;
    private InputManager cursor;
    private PoolManager pool;
    public ChipColor color;
    public bool isLinked = false;


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