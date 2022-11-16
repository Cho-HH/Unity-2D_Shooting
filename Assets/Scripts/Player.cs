using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;    
    [SerializeField] private GameObject bulletA;
    [SerializeField] private GameObject bulletB;
    [SerializeField] private float fireDelay;
    [SerializeField] private GameObject boomEffect;
    [SerializeField] private Transform bulletPos;
    [SerializeField] private Text boomCountText;
    [SerializeField] private GameObject[] followers;
    [SerializeField] private VariableJoystick joystick;

    private Animator animator;        
    private Rigidbody2D rigid;

    private bool isTouchTop;
    private bool isTouchBottom;
    private bool isTouchLeft;
    private bool isTouchRight;

    private int maxPower;
    private int curPower;
    private int hp;
    private int curBombCount;
    private bool isBooming;
    private float fireCurrent;
    private bool isStealth;

    private bool isFire;

    float x;
    float y;

    float stealthTime;
    void Awake()
    {
        isBooming = false;
        curBombCount = 3;
        maxPower = 3;
        curPower = 1;
        hp = GameManager.instance.CurLife;
        boomCountText.text = "X " + curBombCount;

        isStealth = false;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();        
    }

    void Update()
    {              
        if (GameManager.instance.IsSpawning)
        {
            return;
        }

        if (animator.GetBool("isDie") == true)
        {
            return;
        }

        x = Input.GetAxisRaw("Horizontal");
        if ((isTouchLeft && x == -1) || (isTouchRight && x == 1))
        {
            x = 0;
        }

        y = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && y == 1) || (isTouchBottom && y == -1))
        {
            y = 0;
        }

       //x = joystick.Horizontal;
       //if ((isTouchLeft && x < 0) || (isTouchRight && x > 0))
       //{
       //    x = 0;
       //}
       //
       //y = joystick.Vertical;
       //if ((isTouchTop && y > 0) || (isTouchBottom && y < 0))
       //{
       //    y = 0;
       //}
        
        animator.SetInteger("x", (int)x);
        fireCurrent += Time.deltaTime;
        
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }

        if (isFire)
        {
            Fire();
        }

        if (Input.GetButtonDown("Jump"))
        {

            Boom();
        }

        if (isStealth == true)
        {
            stealthTime += Time.deltaTime;
            if (stealthTime >= 5.0f)
            {
                ChangeLayer();
                stealthTime = 0.0f;
            }
        }
    }

    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + new Vector2(x, y) * speed * Time.deltaTime);
    }
    public void Boom()
    {
        if (isBooming == true)
        {
            return;
        }
        if (curBombCount <= 0)
        {
            return;
        }

        isBooming = true;
        boomEffect.SetActive(true);
        curBombCount--;
        boomCountText.text = "X " + curBombCount;

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in bullets)
        {
            bullet.SetActive(false);
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.name == "Enemy Boss")
            {
                Boss boss = enemy.GetComponent<Boss>();
                boss.OnHit(100);
            }
            else
            {
                Enemy enemy1 = enemy.GetComponent<Enemy>();
                enemy1.OnHit(100);
            }
        }
        SoundManager.instance.BoomSoundPlay();

        Invoke("OffBoomEffect", 1.0f);
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBooming = false;
    }
    public void Fire()
    {
        if (fireCurrent < fireDelay)
        {
            return;
        }

        if (curPower == 1)
        {
            BulletManager.instance.Spawn(0, bulletPos.position);
        }
        else if (curPower == 2)
        {
            BulletManager.instance.Spawn(0, bulletPos.position + Vector3.left * 0.1f);
            BulletManager.instance.Spawn(0, bulletPos.position + Vector3.right * 0.1f);
        }
        else
        {
            BulletManager.instance.Spawn(0, bulletPos.position + Vector3.left * 0.2f);
            BulletManager.instance.Spawn(0, bulletPos.position + Vector3.right * 0.2f);
            BulletManager.instance.Spawn(1, bulletPos.position + Vector3.up * 0.2f);
        }

        fireCurrent = 0.0f;
        SoundManager.instance.AttackSoundPlay();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                default:
                    break;
            }
        }
    
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            hp--;
            SoundManager.instance.DieSoundPlay();
            if (hp < 0)
            {
                GameManager.instance.GameOver();
            }
            else
            {
                animator.SetBool("isDie", true);
                gameObject.layer = 8;
                GameManager.instance.UpdateLife();
                curPower = 1;
                foreach (GameObject follower in followers)
                {
                    follower.SetActive(false);
                }
                Invoke("Respawn", 2.0f);
            }
        }             
        
        if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.Type)
            {
                case "Stealth":
                    {
                        gameObject.layer = 8;
                        SpriteRenderer sp = GetComponent<SpriteRenderer>();
                        sp.color = new Color(1, 1, 1, 0.4f);
                        isStealth = true;
                        stealthTime = 0.0f;
                    }
                    break;
                case "Power":
                    curPower++;
                    if (maxPower <= curPower)
                    {
                        AddFollower();
                    }
                    break;
                case "Boom":
                    curBombCount++;
                    boomCountText.text = "X " + curBombCount;
                    break;
                default:
                    break;
            }
            Destroy(collision.gameObject);
        }
    }

    void ChangeLayer()
    {
        isStealth = false;
        GameManager.instance.ChangeLayer();
    }

    void AddFollower()
    {
        if (curPower == 4)
        {
            followers[0].SetActive(true);
        }
        else if (curPower == 5)
        {
            followers[1].SetActive(true);
        }
        else if (curPower == 6)
        {
            followers[2].SetActive(true);
        }
    }

    void Respawn()
    {        
        GameManager.instance.RespawnPlayer();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                default:
                    break;
            }
        }
    }

    void Deactive()
    {
        gameObject.SetActive(false);
    }

    public void FireDown()
    {
        isFire = true;
    }

    public void FireUp()
    {
        isFire = false;
    }
}
