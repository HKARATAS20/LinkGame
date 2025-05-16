using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all items in the grid. It was not necessary with only chips 
/// for now but if we want to add more items such as powerups or obstacles 
/// we can inherit from this class and reuse the common functionality.
/// </summary>

public abstract class GridItem : Movable
{
    public Vector2Int Position { get; set; }
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected ParticleSystem particles;
    public Sprite particleSprite;

    public LinkableGrid grid;
    public PoolManager pool;

    public virtual bool CanFall() => true;
    public virtual bool CanBeLinked() => true;
    public virtual bool IsObstacle() => false;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetupParticles();
        grid = (LinkableGrid)LinkableGrid.Instance;
        pool = PoolManager.Instance;
    }

    public virtual void SetupParticles()
    {
        if (particles != null)
        {
            particles = Instantiate(particles, transform);
            particles.gameObject.SetActive(false);
            if (particleSprite != null)
            {
                var textureSheetAnimation = particles.textureSheetAnimation;
                if (textureSheetAnimation.spriteCount > 0)
                    textureSheetAnimation.RemoveSprite(0);
                textureSheetAnimation.AddSprite(particleSprite);
            }
        }
    }

    public virtual void PlayParticleEffects()
    {
        if (particles == null) return;
        particles.transform.position = transform.position;
        particles.gameObject.SetActive(true);
        particles.Play();
    }

    public virtual IEnumerator Resolve(Transform collectionPoint, bool shouldAnimate = false)
    {

        if (shouldAnimate)
        {
            if (collectionPoint != transform)
            {  //We can use this to animate the block to a collection point like in wonder link 
               // Right now we never go into this condition so the chips dont move
                yield return StartCoroutine(MoveToTransform(collectionPoint));
            }
        }
        else
        {
            PlayParticleEffects();
        }
        grid.RemoveItemAt(this.Position);
        pool.ReturnBlock(this);
    }
}