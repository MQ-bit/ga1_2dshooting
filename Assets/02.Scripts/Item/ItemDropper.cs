using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] private ItemDropDataTableSO _dropDataTable;

    public void DropItem(Vector3 dropPosition)
    {
        if (_dropDataTable == null ||
            _dropDataTable.Datas == null ||
            _dropDataTable.Datas.Length == 0)
        {
            return;
        }

        // 전체 가중치 범위에서 랜덤한 정수를 뽑는다.
        int totalWeight = 0;
        foreach (ItemDropData data in _dropDataTable.Datas)
        {
            if (data == null || data.ItemPrefab == null || data.Percent <= 0)
            {
                continue;
            }

            totalWeight += data.Percent;
        }

        if (totalWeight <= 0)
        {
            return;
        }

        int randomWeight = Random.Range(0, totalWeight);

        // 가중치를 누적하면서 선택된 구간을 찾는다.
        int cumulativeWeight = 0;
        foreach (ItemDropData data in _dropDataTable.Datas)
        {
            if (data == null || data.ItemPrefab == null || data.Percent <= 0)
            {
                continue;
            }

            cumulativeWeight += data.Percent;
            if (randomWeight < cumulativeWeight)
            {
                Instantiate(
                    data.ItemPrefab,
                    dropPosition,
                    Quaternion.identity
                );

                return;
            }
        }
    }
}
