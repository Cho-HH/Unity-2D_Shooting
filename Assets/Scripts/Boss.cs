using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    enum EATTACK_TYPE
    {
        FORWARD = 0,
        SHOT,
        AROUND,
        ARC
    }

    [SerializeField] private int maxHp;
    [SerializeField] private GameObject bulletA;
    [SerializeField] private GameObject bulletB;
    [SerializeField] private GameObject player;

    private GameObject[] bulletPos;

    private int hp;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 dest;

    private bool isAppear;
    public bool IsAppear
    {
        get { return isAppear; }
        set { isAppear = value; }
    }

    private bool isCanAttack;
    public bool IsCanAttack
    {
        get { return isCanAttack; }
    }

    private int curPattern;
    private int curPatternCount;
    private int[] maxPatternCount;
    private bool isAttacking;
    private bool isBossDie;
    private float z;
    void Awake()
    {
        curPattern = 0;
        hp = maxHp;
        isCanAttack = false;
        dest = new Vector2(0, 3.0f);
        isAppear = false;
        isAttacking = false;
        curPatternCount = 0;
        isBossDie = false;

        maxPatternCount = new int[] { 3, 1, 15, 100 };
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();        
        bulletPos = GameObject.FindGameObjectsWithTag("BulletPos");
    }


    // Update is called once per frame
    void Update()
    {
        if (isAppear == true)
        {
            rb.MovePosition(Vector2.Lerp(rb.position, dest, Time.deltaTime * 2.0f));
            if (Mathf.Abs(rb.position.y - dest.y) <= 0.2f)
            {
                isAppear = false;
                isCanAttack = true;
            }
        }
        
        if (isCanAttack == true && hp > 0)
        {

            Invoke("Attack", 2.0f);            
        }

        if (hp <= 0)
        {
            isBossDie = true;
        }               
    }

    void Attack()
    {
        z = Mathf.Sin(Time.time) * 0.4f;
        if (isAttacking == true)
        {
            return;
        }

        isAttacking = true;
        curPatternCount = 0;
        curPattern = Random.Range(0, 10);
        if (curPattern <= 2)
        {
            FireForward();
        }
        else if (curPattern <= 5)
        {
            FireShot();
        }
        else if (curPattern <= 7)
        {
            FireAround();
        }
        else
        {
            FireArc();
        }
    }

    void FireForward()
    {
        //앞으로 발사        
        foreach (GameObject pos in bulletPos)
        {
            BulletManager.instance.Spawn(5, pos.transform.position, Quaternion.identity);
        }

        Pattern("FireForward", (int)EATTACK_TYPE.FORWARD, 0.25f);                
    }

    void FireShot()
    {
        //샷건 발사
        Vector2 shootVec = (player.transform.position - transform.position).normalized;
        float rad = Vector2.Dot(shootVec, Vector2.down);
        rad = Mathf.Acos(rad);
        float deg = rad * Mathf.Rad2Deg;
        Vector3 cross = Vector3.Cross(shootVec, Vector2.down);
      
        for (int i = -2; i <= 2; i++)
        {
            Quaternion rotation = cross.z <= 0 ? Quaternion.Euler(0, 0, deg + i * 10) : Quaternion.Euler(0, 0, -deg + i * 10);
            BulletManager.instance.Spawn(6, bulletPos[4].transform.position, rotation);
        }

        Pattern("FireShot", (int)EATTACK_TYPE.SHOT, 0.5f);
    }

    void FireAround()
    {
        //원 형태로 공격
        int bulletCnt = 20;
        float rad = Random.Range(1.0f, 20.0f);
        for (int i = 1; i <= bulletCnt; i++)
        {
            float deg = Mathf.Rad2Deg * 2 * Mathf.PI / bulletCnt;
            Quaternion rotate = Quaternion.Euler(0, 0, rad + deg * i);
            BulletManager.instance.Spawn(6, bulletPos[4].transform.position, rotate);
        }

        Pattern("FireAround", (int)EATTACK_TYPE.AROUND, 0.25f);
    }

    void FireArc()
    {
        // 부채꼴모양?으로 발사
        Quaternion rotate = Quaternion.identity;
        rotate.z += z;
        BulletManager.instance.Spawn(5, bulletPos[4].transform.position, rotate);

        Pattern("FireArc", (int)EATTACK_TYPE.ARC ,0.25f);
    }

    void Pattern(string funcName, int patternIdx, float invokeTime)
    {
        curPatternCount++;
        if (curPatternCount > maxPatternCount[patternIdx])
        {
            StartCoroutine(AttackCoroutine(2.0f));
        }
        else
        {
            Invoke(funcName, invokeTime);
        }
    }

    IEnumerator AttackCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isAttacking = false;
        Attack();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet" && isCanAttack == true)
        {
            PlayerBullet pb = collision.gameObject.GetComponent<PlayerBullet>();    
            OnHit(pb.BulletDamage);
        }
    }

    public void OnHit(int damage)
    {
        hp -= damage;
        GameManager.instance.UpdateBossHP(hp, maxHp);
        anim.SetTrigger("hit");
    }
}
