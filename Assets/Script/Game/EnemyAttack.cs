using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    
    public GameObject target; // �^�[�Q�b�g�i�v���C���[�j
    public GameObject projectilePrefab; // ���˂���I�u�W�F�N�g�̃v���n�u
    public float attackInterval = 2f; // �U���̊Ԋu
    public float projectileSpeed = 10f; // ��΂��I�u�W�F�N�g�̑��x

    private float attackTimer; // �U���Ԋu���v��^�C�}�[



    public float fadeInDuration = 2f; // �t�F�[�h�C���ɂ����鎞��
    private SpriteRenderer spriteRenderer; // �X�v���C�g�����_���[�ւ̎Q��
    public bool isFullyVisible = false; // �X�v���C�g�����S�ɕ\��������true�ɂȂ�

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        attackTimer = attackInterval; // ���������Ƀ^�C�}�[��ݒ�
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0); // �ŏ��͓���
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            // �o�ߎ��Ԃɉ����ăA���t�@�l�𑝉�
            float newAlpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���S�ɕ\�����ꂫ������A���t�@��1�ɐݒ肵�A�t���O���X�V
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
            // �^�C�}�[�����炵�Ă���
            attackTimer -= Time.deltaTime;

            // �U���Ԋu�ɒB�����甭��
            if (attackTimer <= 0f)
            {
                FireProjectile();
                attackTimer = attackInterval; // �^�C�}�[�����Z�b�g
            }
        }
    }

    private void FireProjectile()
    {
        // �^�[�Q�b�g�̕������v�Z
        Vector3 direction = (target.transform.position - transform.position).normalized;

        // �e�𐶐�
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // �e�ɕ����Ƒ��x��ݒ�
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }
}
