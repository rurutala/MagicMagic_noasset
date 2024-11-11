using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度を調整できるパラメータ

    private void Update()
    {
        if (Timemanager.Instance.stop)
        {
            return;
        }

        if (BackgroundMove.Instance.ismoving)
            // 左方向（X軸の負の方向）に移動し続ける
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

}
