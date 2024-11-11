using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // 通常の移動速度
    public float dashForce = 300f; // ダッシュ時のインパルスの力
    public float doubleTapTime = 0.3f; // ダブルタップの時間間隔
    public float dashDuration = 0.1f; // ダッシュの持続時間

    public GameObject bulletPrefab; // 通常ショットのプレハブ
    public GameObject chargeShotPrefabLevel1; // チャージレベル1の弾のプレハブ
    public GameObject chargeShotPrefabLevel2; // チャージレベル2の弾のプレハブ
    public float minBulletSpeed = 5f; // 弾の最小速度
    public float maxBulletSpeed = 15f; // 弾の最大速度
    public float maxDistance = 10f; // 最大距離（この距離で maxBulletSpeed になる）
    public Transform bulletSpawnPoint; // 弾の発射位置

    public int life;
    public int life_max;
    public List<GameObject> Life_image;
    public List<Image> Magic_Image;

    private Camera mainCamera;
    private int currentIndex;

    public bool invincible;

    public float invincible_time_max;
    private float invincible_time;

    // チャージ関連の変数
    private float chargeTime = 0f; // チャージ時間
    public float chargeLevel1Time = 1f; // チャージレベル1に必要な時間
    public float chargeLevel2Time = 2f; // チャージレベル2に必要な時間
    private bool isCharging = false; // チャージ中かどうか

    public TYPE player_type = TYPE.Normal;
    public List<Sprite> type;
    public int type_place;

    public int Stopitem = 0;

    // ダッシュ関連
    private Vector2 lastMoveDirection = Vector2.zero; // 最後の移動方向
    private float lastTapTime = 0; // 最後に入力された時間
    private bool isDashing = false; // 現在ダッシュ中かどうか
    private float dashEndTime = 0; // ダッシュ終了時間

    private Rigidbody2D rb; // Rigidbody2Dコンポーネントへの参照

    public float blinkInterval = 0.2f; // 点滅の間隔（秒単位）

    private SpriteRenderer spriteRenderer; // スプライトレンダラーへの参照
    private float blinkTimer = 0f; // 点滅のためのタイマー

    private Color originalColor; // 元のスプライトの色
    public float transparentAlpha = 0.5f; // 半透明時のアルファ値

    public float damegeinvincible;
    public float Stoptime;

    public TextMeshProUGUI stop_item_text;

    private void Start()
    {
        mainCamera = Camera.main; // メインカメラの参照を取得
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dコンポーネントを取得
        Magic_Image[0].sprite = type[0];
        Magic_Image[1].sprite = type[1];
        Magic_Image[2].sprite = type[2];

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // スプライトの元の色を保存

        stop_item_text.text = Stopitem.ToString();
    }

    private void Update()
    {
        HandleMovement();
        HandleTypeChange();
        HandleCharging();

        if (Input.GetKeyDown(KeyCode.T) && !Timemanager.Instance.stop && Stopitem >= 1)
        {
            Stopitem -= 1;
            stop_item_text.text = Stopitem.ToString();
            Time_Stop();
        }
    }

    private void HandleMovement()
    {
        // ダッシュ中の場合、通常移動をスキップ
        if (isDashing)
        {
            // ダッシュが終了するかどうかを確認
            if (Time.time >= dashEndTime)
            {
                isDashing = false;
                rb.velocity = Vector2.zero; // ダッシュ終了後に速度をリセット
            }
            else
            {
                return; // ダッシュ中は通常移動を無視
            }
        }

        // 入力された移動方向を取得（キーごとに確認）
        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector2.right;

        // ダッシュ判定
        if (moveDirection != Vector2.zero)
        {
            if (moveDirection == lastMoveDirection && Time.time - lastTapTime < doubleTapTime && !isDashing)
            {
                Dash(moveDirection); // ダッシュを実行
                isDashing = true;
                dashEndTime = Time.time + dashDuration; // ダッシュ終了時間を設定
            }

            // 最後の入力方向と時間を更新
            lastMoveDirection = moveDirection;
            lastTapTime = Time.time;
        }

        // 通常移動
        if (!isDashing) // ダッシュ中でない場合にのみ通常の移動を適用
        {
            moveDirection = Vector2.zero; // 移動方向のリセット

            if (Input.GetKey(KeyCode.W)) moveDirection = Vector2.up;
            if (Input.GetKey(KeyCode.S)) moveDirection = Vector2.down;
            if (Input.GetKey(KeyCode.A)) moveDirection = Vector2.left;
            if (Input.GetKey(KeyCode.D)) moveDirection = Vector2.right;

            rb.velocity = moveDirection * speed;
        }
        // カメラの範囲内にプレイヤーを制限
        ClampPlayerPosition();
        HandleInvincibility();
        HandleBlinking(); // 点滅処理を呼び出し
    }



    private void Dash(Vector2 direction)
    {
        // ダッシュの瞬間的な加速をインパルスで加える
        rb.velocity = Vector2.zero; // ダッシュ前に速度をリセット
        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);
        ActivateInvincibility(dashDuration); // 無敵状態を管理
    }

    private void HandleTypeChange()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Change_Type(1);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Change_Type(0);
        }
    }

    public void ActivateInvincibility(float duration)
    {
        invincible = true;
        invincible_time = duration; // 引数で渡された時間を無敵時間に設定
    }

    private void HandleInvincibility()
    {
        if (invincible)
        {
            // 無敵時間を減らす
            invincible_time -= Time.deltaTime;

            // 無敵時間が終了したら無敵状態を解除
            if (invincible_time <= 0)
            {
                invincible = false;
                spriteRenderer.color = originalColor; // 色を元に戻す
                spriteRenderer.enabled = true; // スプライトを表示
            }
        }
    }

    private void HandleBlinking()
    {
        // invincibleがtrueの間だけ点滅処理を行う
        if (invincible)
        {
            // タイマーを更新
            blinkTimer += Time.deltaTime;

            // タイマーが点滅間隔を超えたらスプライトの表示/非表示を切り替える
            if (blinkTimer >= blinkInterval)
            {
                // スプライトの表示/非表示を反転
                spriteRenderer.enabled = !spriteRenderer.enabled;

                // タイマーをリセット
                blinkTimer = 0f;
            }

            // スプライトが表示されているときに半透明にする
            if (spriteRenderer.enabled)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, transparentAlpha);
            }
        }
        else
        {
            // invincibleがfalseのときスプライトを常に表示し、色を元に戻す
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
    }

    private void HandleCharging()
    {
        // チャージ処理
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            chargeTime = 0f;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            if (isCharging)
            {
                chargeTime += Time.deltaTime;
                if (chargeTime >= chargeLevel2Time)
                {
                    GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 255 / 255f, 0 / 255f);
                }
                else if (chargeTime >= chargeLevel1Time)
                {
                    GetComponent<SpriteRenderer>().color = new Color(255 / 255f, 255 / 255f, 200 / 255f);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isCharging)
            {
                Shoot();
                isCharging = false;
                chargeTime = 0f;
            }
        }
    }

    private void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - bulletSpawnPoint.position).normalized;
        float distance = Vector2.Distance(mousePosition, bulletSpawnPoint.position);
        float bulletSpeed = Mathf.Lerp(minBulletSpeed, maxBulletSpeed, Mathf.Clamp01(distance / maxDistance));

        GameObject selectedBulletPrefab = bulletPrefab;
        if (chargeTime >= chargeLevel2Time)
        {
            selectedBulletPrefab = chargeShotPrefabLevel2;
        }
        else if (chargeTime >= chargeLevel1Time)
        {
            selectedBulletPrefab = chargeShotPrefabLevel1;
        }

        GameObject bullet = Instantiate(selectedBulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed;

        GetComponent<SpriteRenderer>().color = Color.white;
        bullet.GetComponent<Bullet>().Change_BulletType(player_type);
    }

    private void ClampPlayerPosition()
    {
        Vector3 playerPosition = transform.position;
        Vector3 minBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 maxBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        playerPosition.x = Mathf.Clamp(playerPosition.x, minBounds.x, maxBounds.x);
        playerPosition.y = Mathf.Clamp(playerPosition.y, minBounds.y, maxBounds.y);
        transform.position = playerPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Enemy" && !invincible)
        {
            Life_dec(1);
            ActivateInvincibility(damegeinvincible); // 無敵状態を管理
        }
        if (collision.tag == "cure")
        {
            Life_inc(1);
            Destroy(collision.gameObject);
        }
        if(collision.tag == "stopitem")
        {
            Stopitem += 1;
            Destroy(collision.gameObject);
            stop_item_text.text = Stopitem.ToString();
        }
    }

    public void Life_dec(int dec)
    {
        for (int i = 1; i <= dec; i++)
        {
            Life_image[life - i].SetActive(false);
        }
        life -= dec;

        if (life <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Life_inc(int inc)
    {
        for (int i = 1; i <= inc; i++)
        {
            if (life < life_max && life < Life_image.Count)
            {
                Life_image[life].SetActive(true);
                life++;
            }
            else
            {
                break;
            }
        }
    }

    public void Change_Type(int direction)
    {
        if (direction == 0)
        {
            NextImage();
        }
        else
        {
            PreviousImage();
        }
        player_type = current_Type(currentIndex);
    }
    private void NextImage()
    {
        currentIndex = (currentIndex + 1) % type.Count;
        UpdateImages();
    }

    private void PreviousImage()
    {
        currentIndex = (currentIndex - 1 + type.Count) % type.Count;
        UpdateImages();
    }

    private void UpdateImages()
    {
        Magic_Image[1].sprite = type[currentIndex];
        int leftIndex = (currentIndex - 1 + type.Count) % type.Count;
        Magic_Image[0].sprite = type[leftIndex];
        int rightIndex = (currentIndex + 1) % type.Count;
        Magic_Image[2].sprite = type[rightIndex];
    }

    public TYPE current_Type(int typeIndex)
    {
        switch (typeIndex)
        {
            case 1: return TYPE.Normal;
            case 0: return TYPE.Water;
            case 2: return TYPE.Fire;
        }
        return TYPE.Normal;
    }


    public void Time_Stop()
    {
        Timemanager.Instance.Stop(Stoptime);
    }
}
