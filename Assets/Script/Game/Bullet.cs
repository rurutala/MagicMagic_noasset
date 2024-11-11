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
    public float lifetime = 2f; // �e��������܂ł̎���
    public float attack;
    public bool disappear = true;
    public TYPE bullet_type;

    private Camera mainCamera;
    private Vector2 storedVelocity;
    private Rigidbody2D rb; // Rigidbody2D�̎Q��
    private bool isPaused = false;
    private float remainingLifetime; // �c��̎�����ێ�

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        remainingLifetime = lifetime; // �c��̎�����ݒ�
        ColorChange();
        StartCoroutine(LifetimeCoroutine()); // �����̃J�E���g�_�E�����J�n
    }

    private void Update()
    {
        HandlePause(); // �ꎞ��~�̊Ǘ�

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);

        // ��ʊO���ǂ����𔻒�
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            Destroy(gameObject); // ��ʊO�ɏo����I�u�W�F�N�g���폜
        }
    }

    private void HandlePause()
    {
        if (Timemanager.Instance.stop && !isPaused)
        {
            // �ꎞ��~����
            storedVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
            isPaused = true;
        }
        else if (!Timemanager.Instance.stop && isPaused)
        {
            // �ꎞ��~��������
            rb.velocity = storedVelocity;
            isPaused = false;
        }
    }

    private IEnumerator LifetimeCoroutine()
    {
        while (remainingLifetime > 0)
        {
            // �ꎞ��~����Ă��Ȃ��ꍇ�̂ݎ��Ԃ����炷
            if (!isPaused)
            {
                remainingLifetime -= Time.deltaTime;
            }

            yield return null;
        }

        Destroy(gameObject); // �c�莞�Ԃ�0�ɂȂ�����e���폜
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
