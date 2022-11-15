using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    [SerializeField] private int bulletDamage;

    public int BulletDamage
    {
        get { return bulletDamage; }
    }
    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + new Vector2(transform.up.x, transform.up.y) * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturned == true)
        {
            return;
        }

        if (collision.gameObject.tag == "BulletAndEnemyBorder" || collision.gameObject.tag == "Enemy")
        {
            BulletManager.instance.ReturnPool(this);
            isReturned = true;
        }
    }
}
