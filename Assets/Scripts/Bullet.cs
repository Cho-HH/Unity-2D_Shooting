using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Bullet : MonoBehaviour, IPoolObject
{

    [SerializeField] private string idName;
    public string IdName
    {
        get { return idName; }
    }

    [SerializeField] protected float bulletSpeed;   

    protected Rigidbody2D rigid;
    protected bool isReturned;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void OnCreatedInPool()
    {
        //처음 생성시 호출되는 함수
    }

    public void OnGettingFromPool()
    {
        //재사용시 호출되는 함수
        isReturned = false;
    }
}
