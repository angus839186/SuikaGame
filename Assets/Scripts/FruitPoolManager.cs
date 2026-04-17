using System.Collections.Generic;
using UnityEngine;

public class FruitPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class FruitPoolEntry
    {
        public GameObject prefab;
        public int initialSize;
    }

    public static FruitPoolManager Instance { get; private set; }

    [Header("Pool Config")]
    [SerializeField] private FruitPoolEntry[] pools;

    private Dictionary<int, Queue<Fruit>> fruitPools = new Dictionary<int, Queue<Fruit>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePools();
    }

    private void InitializePools()
    {
        fruitPools.Clear();

        for (int i = 0; i < pools.Length; i++)
        {
            Queue<Fruit> queue = new Queue<Fruit>();
            fruitPools.Add(i, queue);

            for (int j = 0; j < pools[i].initialSize; j++)
            {
                Fruit fruit = CreateFruit(i);
                fruit.gameObject.SetActive(false);
                queue.Enqueue(fruit);
            }
        }
    }

    private Fruit CreateFruit(int tierIndex)
    {
        GameObject obj = Instantiate(pools[tierIndex].prefab, transform);
        Fruit fruit = obj.GetComponent<Fruit>();

        if (fruit == null)
        {
            Debug.LogError($"Prefab at tier {tierIndex} does not have a Fruit component.");
            return null;
        }

        fruit.SetPool(this, tierIndex);
        return fruit;
    }

    public Fruit GetFruit(int tierIndex, Vector3 position)
    {
        if (!fruitPools.ContainsKey(tierIndex))
        {
            Debug.LogError($"No fruit pool found for tier {tierIndex}");
            return null;
        }

        Fruit fruit;

        if (fruitPools[tierIndex].Count > 0)
        {
            fruit = fruitPools[tierIndex].Dequeue();
        }
        else
        {
            fruit = CreateFruit(tierIndex);
        }

        if (fruit == null) return null;

        fruit.transform.position = position;
        fruit.transform.rotation = Quaternion.identity;
        fruit.gameObject.SetActive(true);

        return fruit;
    }

    public void ReturnFruit(Fruit fruit)
    {
        if (fruit == null) return;

        int tierIndex = fruit.PoolTierIndex;

        if (!fruitPools.ContainsKey(tierIndex))
        {
            Debug.LogWarning($"Trying to return fruit to missing pool tier {tierIndex}");
            fruit.gameObject.SetActive(false);
            return;
        }

        fruit.transform.SetParent(transform);
        fruit.gameObject.SetActive(false);
        fruitPools[tierIndex].Enqueue(fruit);
    }
}
