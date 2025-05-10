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
    public ChipColor color;


    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //SetupParticles();
        grid = (LinkableGrid)LinkableGrid.Instance;
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

        Destroy(gameObject);

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