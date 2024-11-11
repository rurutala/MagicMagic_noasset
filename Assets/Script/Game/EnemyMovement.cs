using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public int hp = 3; // �G��HP��ݒ�i��: 3�j
    public bool key_enemy;
    public EnemyAttack enemyattack;

    public GameObject destory;


    private void Start()
    {
        // �K�v�ɉ����ď����ݒ�
        enemyattack = this.gameObject.GetComponent<EnemyAttack>();
    }

    private void Update()
    {
        // �G�̈ړ��⑼�̃��W�b�N�������ɋL�q
    }

    // PlayerAttack�^�O���t�����I�u�W�F�N�g�ƏՓ˂����Ƃ��̏���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack") || collision.CompareTag("Player") && !BackgroundMove.Instance.ismoving && enemyattack.isFullyVisible)
        {
            // HP�����炷
            hp -= (int)collision.GetComponent<Bullet>().attack;
            if (collision.GetComponent<Bullet>().disappear)
            {
                Destroy(collision.gameObject);
            }

            // HP��0�ȉ��ɂȂ����玩�g������
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
