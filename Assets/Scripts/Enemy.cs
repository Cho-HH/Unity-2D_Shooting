using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using System.ComponentModel.Design;

public class Enemy : MonoBehaviour, IPoolObject
{
    [SerializeField] private float speed;
    [SerializeField] private float hp;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject bulletA;
    [SerializeField] private GameObject bulletB;
    [SerializeField] private Transform[] bulletPos;
    [SerializeField] private GameObject[] items;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private float maxHp;
    [SerializeField] private float fireDelay;
    private float fireCurrent;

    private GameObject player;

    public GameObject Player
    {
        set { player = value; }
    }

    [SerializeField] private string idName;
    public string IdName
    {
        get { return idName; }
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxHp = hp;
    }

    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position - new Vector2(transform.up.x, transform.up.y) * speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        fireCurrent += Time.deltaTime;
        if (player != null)
        {
            if (CheckFront(out float degree, out Vector3 cross))
            {
                Fire(in degree, in cross);
            }
        }
    }

    bool CheckFront(out float degree, out Vector3 cross)
    {
        Vector3 lookVec = player.transform.position - transform.position;
        float dot = Vector3.Dot(lookVec.normalized, -transform.up);       
        if (dot > 0.3f)
        {
            degree = Mathf.Rad2Deg * Mathf.Acos(dot);
            //외적으로 왼쪽, 오른쪽 구하기
            cross = Vector3.Cross(lookVec.normalized, -transform.up);
            return true;
        }
        degree = 0;
        cross = Vector3.zero;
        return false;        
    }
    void Fire(in float degree, in Vector3 cross)
    {
        if (fireCurrent < fireDelay)
        {
            return;
        }

        if (gameObject.name != "Enemy A(Clone)")
        {            
            //Debug.Log("cross : " + cross);            
            float rotZ = Mathf.Rad2Deg * transform.rotation.z * 2;            
            Quaternion rotation = cross.z <= 0 ? Quaternion.Euler(0, 0, rotZ + degree) : Quaternion.Euler(0, 0, rotZ - degree);
            if (gameObject.name == "Enemy B(Clone)")
            {
                BulletManager.instance.Spawn(2, bulletPos[0].position, rotation);
            }
            else if (gameObject.name == "Enemy C(Clone)")
            {
                BulletManager.instance.Spawn(3, bulletPos[0].position, rotation);
                BulletManager.instance.Spawn(3, bulletPos[1].position, rotation);
            }
            
        }        

        fireCurrent = 0.0f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            PlayerBullet bullet = collision.GetComponent<PlayerBullet>();
            OnHit(bullet.BulletDamage);
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.05f);
            
        }
        else if (collision.gameObject.tag == "BulletAndEnemyBorder")
        {
            EnemyManager.instance.ReturnPool(this); 
        }
    }

    public void OnHit(int damage)
    {
        if (hp <= 0)
        {
            return;
        }

        hp -= damage;
        if (hp <= 0)
        {
            DropItem();
            SoundManager.instance.HitSoundPlay();
            EnemyManager.instance.ReturnPool(this);
        }
    }

    void DropItem()
    {
       int num = Random.Range(1, 31);
       if (num <= 27)
       {
           return;
       }
       else if (num <= 28) //Stealth
       {
           Instantiate(items[2], transform.position, items[1].transform.rotation);
       }
       else if (num <= 29) // Bomb
       {
           Instantiate(items[0], transform.position, items[1].transform.rotation);
       }
       else // Power
       {
           Instantiate(items[1], transform.position, items[1].transform.rotation);
       }
    }
    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    public void OnCreatedInPool()
    {        
        //처음 생성시 호출되는 함수
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnGettingFromPool()
    {
        //재사용시 호출되는 함수
        hp = maxHp;
    }
}
