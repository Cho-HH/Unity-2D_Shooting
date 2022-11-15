using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPos;

    [SerializeField] private float fireDelay;
    private float fireCurrent;

    private Vector3 followPos;
    [SerializeField] private int followDelay;
    [SerializeField] private Transform parent;
    private Queue<Vector3> parentPos;

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        fireCurrent += Time.deltaTime;
        Fire();

        UpdatePosition();
        Follow();
    }

    void UpdatePosition()
    {
        //Input Pos
        if (!parentPos.Contains(parent.position))
        {
            parentPos.Enqueue(parent.position);
        }
        

        //Output Pos
        if (parentPos.Count > followDelay)
        {
            followPos = parentPos.Dequeue();
        }        
        else if (parentPos.Count < followDelay)
        {
            followPos = parent.position;
        }
    }

    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (fireCurrent < fireDelay)
        {
            return;
        }

        BulletManager.instance.Spawn(4, bulletPos.position);

        fireCurrent = 0.0f;
    }
}
