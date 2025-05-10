using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipPool : ObjectPool<Chip>
{
    public override Chip GetPooledObject()
    {
        Chip chip = base.GetPooledObject();
        chip.GetComponent<SpriteRenderer>().enabled = true;
        chip.GetComponent<BoxCollider2D>().enabled = true;
        return chip;
    }

    public override IEnumerator ReturnObjectToPool(Chip chip)
    {
        chip.GetComponent<SpriteRenderer>().enabled = false;
        chip.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(4f);
        chip.transform.SetParent(transform, true);
        StartCoroutine(base.ReturnObjectToPool(chip));
    }
}
