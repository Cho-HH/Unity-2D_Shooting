using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    PoolManager poolManager;

    void Awake()
    {
        poolManager = GetComponent<PoolManager>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(int poolsNum, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        Enemy enemy = poolManager.GetFromPool<Enemy>(poolsNum);
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = spawnRotation;
    }

    public void ReturnPool(Enemy clone)
    {
        poolManager.TakeToPool<Enemy>(clone.IdName, clone);
    }
}
