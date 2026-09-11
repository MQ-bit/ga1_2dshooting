// 데이터 클래스: 슨수하게 데이터(값)을 보관하고 전달하는 목적으로 만든 특별한 클래스


using UnityEngine;

[System.Serializable]

public class EnemySpawnData
{
    public GameObject EnemyPrefab;
    [Range(0, 100)]
    public int Percent;
}
