using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    public static BackgroundMove Instance { get; private set; }

    public GameObject pointsParent; // Points�I�u�W�F�N�g���w��
    public float moveSpeed = 2f; // ���̃J�����̈ړ����x
    private Transform[] points; // �ړ��|�C���g�̃��X�g
    private int currentPointIndex = 0;
    private Vector3 targetPosition;

    public bool ismoving;

    private void Start()
    {
        if (pointsParent == null)
        {
            Debug.LogError("Points�I�u�W�F�N�g���ݒ肳��Ă��܂���B");
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

        // Points�I�u�W�F�N�g�̎q�I�u�W�F�N�g���擾���ă��X�g�ɓo�^
        int childCount = pointsParent.transform.childCount;
        points = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            points[i] = pointsParent.transform.GetChild(i);
        }

        // �ŏ��̃^�[�Q�b�g�ʒu��ݒ�
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
            // �I�u�W�F�N�g���^�[�Q�b�g�ʒu�Ɍ����Ĉړ�
            Vector3 direction = (targetPosition - transform.position).normalized;

            // X���̈ړ��ʂ𔽓]���AY���͂��̂܂܁AZ����0��
            direction = new Vector3(-direction.x, direction.y, 0);

            // �ړ�
            transform.position += direction * moveSpeed * Time.deltaTime;

            // �^�[�Q�b�g�ʒu�ɓ��B�������`�F�b�N
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // ���̃|�C���g��
                currentPointIndex++;
                if (currentPointIndex >= points.Length)
                {
                    // �|�C���g���ꏄ�������~�܂��̓��[�v����
                    currentPointIndex = points.Length - 1; // �Ō�̃|�C���g�Œ�~
                    // ���[�v���������ꍇ�͈ȉ����g�p
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
