using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 5f; // �ړ����x�𒲐��ł���p�����[�^

    private void Update()
    {
        if (Timemanager.Instance.stop)
        {
            return;
        }

        if (BackgroundMove.Instance.ismoving)
            // �������iX���̕��̕����j�Ɉړ���������
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

}
