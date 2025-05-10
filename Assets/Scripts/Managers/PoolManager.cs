using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : Singleton<PoolManager>
{
    [Header("Block Pools")]
    [SerializeField] private ChipPool bluePool;
    [SerializeField] private ChipPool greenPool;
    [SerializeField] private ChipPool redPool;
    [SerializeField] private ChipPool yellowPool;

    public void PoolAllObjects(int amount)
    {
        bluePool.PoolObjects(amount);
        greenPool.PoolObjects(amount);
        redPool.PoolObjects(amount);
        yellowPool.PoolObjects(amount);

    }

    public void ReturnBlock(Chip chip)
    {

        switch (chip.color)
        {
            case ChipColor.blue:

                StartCoroutine(bluePool.ReturnObjectToPool(chip));
                break;
            case ChipColor.green:

                StartCoroutine(greenPool.ReturnObjectToPool(chip));
                break;
            case ChipColor.red:

                StartCoroutine(redPool.ReturnObjectToPool(chip));
                break;
            case ChipColor.yellow:
                StartCoroutine(yellowPool.ReturnObjectToPool(chip));
                break;
            default:
                Debug.LogWarning("Unknown Chip type");
                break;
        }

    }

    public Chip GetGridBlock(ChipColor chipColor)
    {
        return chipColor switch
        {
            ChipColor.red => redPool.GetPooledObject(),
            ChipColor.green => greenPool.GetPooledObject(),
            ChipColor.blue => bluePool.GetPooledObject(),
            ChipColor.yellow => yellowPool.GetPooledObject(),
            _ => null,
        };
    }

    public Chip GetRandomGridBlock()
    {
        int randomIndex = UnityEngine.Random.Range(0, 4);

        return randomIndex switch
        {
            0 => bluePool.GetPooledObject(),
            1 => greenPool.GetPooledObject(),
            2 => redPool.GetPooledObject(),
            3 => yellowPool.GetPooledObject(),
            _ => null,
        };
    }
}
