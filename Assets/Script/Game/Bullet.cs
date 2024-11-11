using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TYPE
{
    Normal,
    Fire,
    Water,
}

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f; // 弾が消えるまでの時間
    public float attack;
    public bool disappear = true;
    public TYPE bullet_type;

    private Camera mainCamera;
    private Vector2 storedVelocity;
    private Rigidbody2D rb; // Rigidbody2Dの参照
    private bool isPaused = false;
    private float remainingLifetime; // 残りの寿命を保持

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        remainingLifetime = lifetime; // 残りの寿命を設定
        ColorChange();
        StartCoroutine(LifetimeCoroutine()); // 寿命のカウントダウンを開始
    }

    private void Update()
    {
        HandlePause(); // 一時停止の管理

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

        // 画面外かどうかを判定
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            Destroy(gameObject); // 画面外に出たらオブジェクトを削除
        }
    }

    private void HandlePause()
    {
        if (Timemanager.Instance.stop && !isPaused)
        {
            // 一時停止処理
            storedVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
            isPaused = true;
        }
        else if (!Timemanager.Instance.stop && isPaused)
        {
            // 一時停止解除処理
            rb.velocity = storedVelocity;
            isPaused = false;
        }
    }

    private IEnumerator LifetimeCoroutine()
    {
        while (remainingLifetime > 0)
        {
            // 一時停止されていない場合のみ時間を減らす
            if (!isPaused)
            {
                remainingLifetime -= Time.deltaTime;
            }

            yield return null;
        }

        Destroy(gameObject); // 残り時間が0になったら弾を削除
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.tag == "Enemy" && collision.tag == "PlayerAttack" && disappear)
        {
            Destroy(this.gameObject);
        }
        else if (this.gameObject.tag == "PlayerAttack" && collision.tag == "Enemy" && disappear)
        {
            Destroy(this.gameObject);
        }
    }

    public void Change_BulletType(TYPE bullettype)
    {
        bullet_type = bullettype;
    }

    public void ColorChange()
    {
        switch (bullet_type)
        {
            case TYPE.Normal:
                break;
            case TYPE.Water:
                GetComponent<SpriteRenderer>().color = new Color(200 / 255f, 255 / 255f, 255 / 255f);
                break;
            case TYPE.Fire:
                GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 125 / 255f, 125 / 255f);
                break;
        }
    }

    public TYPE getType()
    {
        return bullet_type;
    }
}
