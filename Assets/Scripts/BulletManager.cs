using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Unity.VisualScripting;

public class BulletManager : MonoBehaviour
{
    public static BulletManager instance;

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

    public void Spawn(int poolsNum, Vector3 spawnPosition)
    {
        Bullet bullet = poolManager.GetFromPool<Bullet>(poolsNum);
        bullet.transform.position = spawnPosition;
    }

    public void Spawn(int poolsNum, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        Bullet bullet = poolManager.GetFromPool<Bullet>(poolsNum);
        bullet.transform.position = spawnPosition;
        bullet.transform.rotation = spawnRotation;
    }

    public void ReturnPool(Bullet clone)
    {
        poolManager.TakeToPool<Bullet>(clone.IdName, clone);
    }
}
