using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    public static BackgroundMove Instance { get; private set; }

    public GameObject pointsParent; // Pointsオブジェクトを指定
    public float moveSpeed = 2f; // 元のカメラの移動速度
    private Transform[] points; // 移動ポイントのリスト
    private int currentPointIndex = 0;
    private Vector3 targetPosition;

    public bool ismoving;

    private void Start()
    {
        if (pointsParent == null)
        {
            Debug.LogError("Pointsオブジェクトが設定されていません。");
            return;
        }
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Pointsオブジェクトの子オブジェクトを取得してリストに登録
        int childCount = pointsParent.transform.childCount;
        points = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            points[i] = pointsParent.transform.GetChild(i);
        }

        // 最初のターゲット位置を設定
        currentPointIndex = 0;
        targetPosition = points[currentPointIndex].position;
    }

    private void Update()
    {
        if (Timemanager.Instance.stop)
        {
            return;
        }

        if (ismoving)
        {
            // オブジェクトをターゲット位置に向けて移動
            Vector3 direction = (targetPosition - transform.position).normalized;

            // X軸の移動量を反転し、Y軸はそのまま、Z軸は0に
            direction = new Vector3(-direction.x, direction.y, 0);

            // 移動
            transform.position += direction * moveSpeed * Time.deltaTime;

            // ターゲット位置に到達したかチェック
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // 次のポイントへ
                currentPointIndex++;
                if (currentPointIndex >= points.Length)
                {
                    // ポイントを一巡したら停止またはループ処理
                    currentPointIndex = points.Length - 1; // 最後のポイントで停止
                    // ループさせたい場合は以下を使用
                    // currentPointIndex = 0;
                }
                targetPosition = points[currentPointIndex].position;
            }
        }
    }

    public void ismoveChange()
    {
        ismoving = !ismoving;
    }
}
