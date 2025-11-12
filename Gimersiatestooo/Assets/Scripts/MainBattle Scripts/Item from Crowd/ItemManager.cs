using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public GameObject prefab;
        public Transform spawnPoint;
        [HideInInspector] public GameObject instance;
    }

    [Header("Item Prefabs dan Spawn Points")]
    public List<ItemData> itemList = new List<ItemData>();

    [Header("Turn Settings")]
    public int minTurnSpawn = 2;
    public int maxTurnSpawn = 5;

    private bool itemsSpawned = false;
    private int turnToSpawn;

    private BattleManager battleManager;

    void Start()
    {
        battleManager = Object.FindFirstObjectByType<BattleManager>();

        turnToSpawn = Random.Range(minTurnSpawn, maxTurnSpawn + 1);
        Debug.Log($"Item akan muncul di turn ke-{turnToSpawn}");
    }

    public void CheckSpawnItems(int currentTurn)
    {
        if (!itemsSpawned && currentTurn >= turnToSpawn)
        {
            foreach (var item in itemList)
            {
                if (item.prefab != null && item.spawnPoint != null)
                {
                    item.instance = Instantiate(item.prefab, item.spawnPoint.position, Quaternion.identity);
                    item.instance.SetActive(true);
                }
            }
            itemsSpawned = true;
        }
    }
}
