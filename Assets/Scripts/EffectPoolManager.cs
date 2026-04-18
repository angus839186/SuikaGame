using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolManager : MonoBehaviour
{
    public static EffectPoolManager Instance { get; private set; }

    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private int initialSize = 6;
    [SerializeField] private float effectLifetime = 1f;

    private Queue<GameObject> effectPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateEffect();
            obj.SetActive(false);
            effectPool.Enqueue(obj);
        }
    }

    private GameObject CreateEffect()
    {
        GameObject obj = Instantiate(effectPrefab, transform);
        return obj;
    }

    public void PlayEffect(Vector3 position)
    {
        GameObject effect;

        if (effectPool.Count > 0)
        {
            effect = effectPool.Dequeue();
        }
        else
        {
            effect = CreateEffect();
        }

        effect.transform.position = position;
        effect.transform.rotation = Quaternion.identity;
        effect.SetActive(true);

        StartCoroutine(ReturnAfterDelay(effect, effectLifetime));
    }

    private IEnumerator ReturnAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);

        effect.SetActive(false);
        effect.transform.SetParent(transform);
        effectPool.Enqueue(effect);
    }
}
