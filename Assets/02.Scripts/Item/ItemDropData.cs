using UnityEngine;

[System.Serializable]

public class ItemDropData
{
    public GameObject ItemPrefab;
    [Range(0, 100)]
    public int Percent;
}
