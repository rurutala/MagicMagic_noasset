using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    
    public GameObject target; // ターゲット（プレイヤー）
    public GameObject projectilePrefab; // 発射するオブジェクトのプレハブ
    public float attackInterval = 2f; // 攻撃の間隔
    public float projectileSpeed = 10f; // 飛ばすオブジェクトの速度

    private float attackTimer; // 攻撃間隔を計るタイマー



    public float fadeInDuration = 2f; // フェードインにかける時間
    private SpriteRenderer spriteRenderer; // スプライトレンダラーへの参照
    public bool isFullyVisible = false; // スプライトが完全に表示されるとtrueになる

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        attackTimer = attackInterval; // 初期化時にタイマーを設定
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0); // 最初は透明
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            // 経過時間に応じてアルファ値を増加
            float newAlpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 完全に表示されきったらアルファを1に設定し、フラグを更新
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        isFullyVisible = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Timemanager.Instance.stop && isFullyVisible)
        {
            return;
        }
        if (target != null)
        {
            // タイマーを減らしていく
            attackTimer -= Time.deltaTime;

            // 攻撃間隔に達したら発射
            if (attackTimer <= 0f)
            {
                FireProjectile();
                attackTimer = attackInterval; // タイマーをリセット
            }
        }
    }

    private void FireProjectile()
    {
        // ターゲットの方向を計算
        Vector3 direction = (target.transform.position - transform.position).normalized;

        // 弾を生成
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // 弾に方向と速度を設定
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }
}
