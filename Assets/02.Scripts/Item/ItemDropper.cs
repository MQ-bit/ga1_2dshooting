using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private int _dropChance = 30;
    [SerializeField] private Item[] _itemPrefabs;

    public void TryDrop()
    {
        if (_itemPrefabs == null || _itemPrefabs.Length == 0) return;

        // 0~99 중 0~29일 때만 드롭한다: 30%.
        if (Random.Range(0, 100) >= _dropChance) return;

        int index = Random.Range(0, _itemPrefabs.Length);
        Item itemPrefab = _itemPrefabs[index];
        if (itemPrefab == null) return;

        Instantiate(itemPrefab, transform.position, Quaternion.identity);
    }
}
