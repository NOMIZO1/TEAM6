using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItem
{
    public ItemData item;
    [Range(0, 100)]
    public float dropChance;
}

public class DropTable : MonoBehaviour
{
    public List<DropItem> dropItems;

    public void Drop()
    {
        foreach (DropItem drop in dropItems)
        {
            float random = Random.Range(0f, 100f);

            if (random < drop.dropChance)
            {
                Instantiate(drop.item.dropPrefab,transform.position,Quaternion.identity);
            }
        }
    }
}
