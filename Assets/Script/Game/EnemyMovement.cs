using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public int hp = 3; // 敵のHPを設定（例: 3）
    public bool key_enemy;
    public EnemyAttack enemyattack;

    public GameObject destory;


    private void Start()
    {
        // 必要に応じて初期設定
        enemyattack = this.gameObject.GetComponent<EnemyAttack>();
    }

    private void Update()
    {
        // 敵の移動や他のロジックをここに記述
    }

    // PlayerAttackタグが付いたオブジェクトと衝突したときの処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack") || collision.CompareTag("Player") && !BackgroundMove.Instance.ismoving && enemyattack.isFullyVisible)
        {
            // HPを減らす
            hp -= (int)collision.GetComponent<Bullet>().attack;
            if (collision.GetComponent<Bullet>().disappear)
            {
                Destroy(collision.gameObject);
            }

            // HPが0以下になったら自身を消す
            if (hp <= 0)
            {
                if (key_enemy)
                {
                    Debug.Log("called");
                    if(!BackgroundMove.Instance.ismoving)
                    BackgroundMove.Instance.ismoveChange();

                }
                Destroy(gameObject);
            }
        }
    }
}
