using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject main;
    [SerializeField] private GameObject inGame;

    [SerializeField] private GameObject[] enemys;
    [SerializeField] private GameObject[] enemySpawnPoints;
    [SerializeField] private GameObject player;
    [SerializeField] private Text timeText;
    [SerializeField] private Image[] lifeImage;
    [SerializeField] private GameObject gameOverSet;
    [SerializeField] private GameObject gameClearSet;
    [SerializeField] private Boss boss;
    [SerializeField] private GameObject bossProgressBar;

    [SerializeField] private bool isSpawning;
    public bool IsSpawning
    {
        get { return isSpawning; }
        set { isSpawning = value; }
    }

    private float sec;
    private int min;
    private SpriteRenderer playerSprite;
    private int curLife;
    public int CurLife
    {
        get { return curLife; }
    }

    private Vector2 destination;
    private AudioSource audioSource;

     void Awake()
    {
        destination = new Vector2(0, -4.0f);
        isSpawning = true;
        min = 3;
        sec = 0;
        curLife = lifeImage.Length;

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.2f;
        playerSprite = player.GetComponent<SpriteRenderer>();
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main.SetActive(true);
        inGame.SetActive(false);       
    }

    void Update()
    {
        sec -= Time.deltaTime;
        timeText.text = "보스출현까지\n" + string.Format("{0:D1}:{1:D2}", min, (int)sec);
        if ((int)sec < 0)
        {
            if (min == 0)
            {
                boss.gameObject.SetActive(true);
                timeText.gameObject.SetActive(false);
                Boss b = boss.GetComponent<Boss>();
                b.IsAppear = true;
                if (b.IsCanAttack == true)
                {
                    bossProgressBar.SetActive(true);
                }
            }
            else
            {
                sec = 60;
                min--;
            }
        }

        if (isSpawning == true)
        {
            player.transform.position = Vector2.Lerp(player.transform.position, destination, Time.deltaTime * 2.0f);
            if (Mathf.Abs(player.transform.position.y - destination.y) <= 0.1f)
            {
                Animator anim = player.GetComponent<Animator>();
                anim.SetBool("isDie", false);
                isSpawning = false;
            }
        }
    }

    void SpawnEnemy()
    {
        if (boss.IsAppear == true)
        {
            return;
        }

        int spawnPointIdx = Random.Range(0, enemySpawnPoints.Length);
        Transform spawnArea = null;
        float spawnPointX = 0.0f;
        float spawnPointY = 0.0f;
        Quaternion rotate = Quaternion.identity;
        //center
        if (spawnPointIdx == 0)
        {
            spawnArea = enemySpawnPoints[0].transform;
            spawnPointX = Random.Range(-1 * spawnArea.localScale.x / 2, spawnArea.localScale.x / 2);
            spawnPointY = spawnArea.position.y;
        }
        //Right, Left
        else
        {
            spawnArea = enemySpawnPoints[spawnPointIdx].transform;

            //position
            spawnPointX = spawnArea.position.x;
            spawnPointY = Random.Range(spawnArea.position.y -1 * spawnArea.localScale.y / 2, spawnArea.position.y + spawnArea.localScale.y / 2);

            //rotation
            Vector3 lookVec = player.transform.position - new Vector3(spawnPointX, spawnPointY);
            float dot = Vector3.Dot(lookVec.normalized, Vector3.down);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            
            rotate = spawnPointIdx == 1 ? Quaternion.Euler(0, 0, -angle) : Quaternion.Euler(0, 0, angle);
        }

        //Select Enemy
        int enemyIdx = Random.Range(0, 10);
        if (enemyIdx < 6)
        {
            int count = Random.Range(2, 5);
            float callTime = 0;
            for (int i = 0; i < count; i++)
            {               
                StartCoroutine(SpawnsCoroutine(0, new Vector3(spawnPointX, spawnPointY), rotate, callTime));
                callTime += 0.1f;
            }
        }
        else if (enemyIdx < 9)
        {
            Spawn(1, new Vector3(spawnPointX, spawnPointY), rotate);
        }
        else
        {
            Spawn(2, new Vector3(spawnPointX, spawnPointY), rotate);
        }

        float spawnTime = Random.Range(0.5f, 2.0f);
        Invoke("SpawnEnemy", spawnTime);
    }

    void Spawn(int enemyNum, Vector3 pos, Quaternion rot)
    {
        EnemyManager.instance.Spawn(enemyNum, pos, rot);
    }

    IEnumerator SpawnsCoroutine(int enemyNum, Vector3 pos, Quaternion rot, float callTime)
    {
        yield return new WaitForSeconds(callTime);
        Spawn(enemyNum, pos, rot);        
    }

    public void UpdateLife()
    {
        lifeImage[--curLife].color = new Color(1, 1, 1, 0.4f);
    }
    public void RespawnPlayer()
    {
        isSpawning = true;
        playerSprite.color = new Color(1, 1, 1, 0.4f);
        player.transform.position = new Vector2(0, -8.0f);
        player.SetActive(true);
        Invoke("ChangeLayer", 5.0f);
    }

    public void ChangeLayer()
    {
        playerSprite.color = new Color(1, 1, 1, 1);
        player.layer = 6;
    }
    public void GameOver()
    {
        Time.timeScale = 0;
        isSpawning = true;
        gameOverSet.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        main.SetActive(false);
        inGame.SetActive(true);

        RespawnPlayer();
        Invoke("SpawnEnemy", 5.0f);
    }

    public void GameClear()
    {
        Time.timeScale = 0;
        isSpawning = true;
        gameClearSet.SetActive(true);
    }
    public void UpdateBossHP(int curHP, int maxHP)
    {
        Text hpText = bossProgressBar.GetComponentInChildren<Text>();
        hpText.text = string.Format("{0} / {1}", curHP, maxHP);        
        Image[] hpBar = bossProgressBar.GetComponentsInChildren<Image>();
        hpBar[1].fillAmount = (float)curHP / (float)maxHP;
    }
}
