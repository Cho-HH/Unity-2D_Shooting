using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string type;
    public string Type
    {
        get { return type; }
    }

    private Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + Vector2.down * 2 * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BulletAndEnemyBorder")
        {
            Destroy(gameObject);
        }
    }
}
