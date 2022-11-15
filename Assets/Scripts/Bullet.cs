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
        //ó�� ������ ȣ��Ǵ� �Լ�
    }

    public void OnGettingFromPool()
    {
        //����� ȣ��Ǵ� �Լ�
        isReturned = false;
    }
}
