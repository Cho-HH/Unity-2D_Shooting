using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position - new Vector2(transform.up.x, transform.up.y) * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturned == true)
        {
            return;
        }

        if (collision.gameObject.tag == "BulletAndEnemyBorder" || collision.gameObject.tag == "Player")
        {
            BulletManager.instance.ReturnPool(this);
            isReturned = true;
        }
    }
}
