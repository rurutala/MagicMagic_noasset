using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // �ʏ�̈ړ����x
    public float dashForce = 300f; // �_�b�V�����̃C���p���X�̗�
    public float doubleTapTime = 0.3f; // �_�u���^�b�v�̎��ԊԊu
    public float dashDuration = 0.1f; // �_�b�V���̎�������

    public GameObject bulletPrefab; // �ʏ�V���b�g�̃v���n�u
    public GameObject chargeShotPrefabLevel1; // �`���[�W���x��1�̒e�̃v���n�u
    public GameObject chargeShotPrefabLevel2; // �`���[�W���x��2�̒e�̃v���n�u
    public float minBulletSpeed = 5f; // �e�̍ŏ����x
    public float maxBulletSpeed = 15f; // �e�̍ő呬�x
    public float maxDistance = 10f; // �ő勗���i���̋����� maxBulletSpeed �ɂȂ�j
    public Transform bulletSpawnPoint; // �e�̔��ˈʒu

    public int life;
    public int life_max;
    public List<GameObject> Life_image;
    public List<Image> Magic_Image;

    private Camera mainCamera;
    private int currentIndex;

    public bool invincible;

    public float invincible_time_max;
    private float invincible_time;

    // �`���[�W�֘A�̕ϐ�
    private float chargeTime = 0f; // �`���[�W����
    public float chargeLevel1Time = 1f; // �`���[�W���x��1�ɕK�v�Ȏ���
    public float chargeLevel2Time = 2f; // �`���[�W���x��2�ɕK�v�Ȏ���
    private bool isCharging = false; // �`���[�W�����ǂ���

    public TYPE player_type = TYPE.Normal;
    public List<Sprite> type;
    public int type_place;

    public int Stopitem = 0;

    // �_�b�V���֘A
    private Vector2 lastMoveDirection = Vector2.zero; // �Ō�̈ړ�����
    private float lastTapTime = 0; // �Ō�ɓ��͂��ꂽ����
    private bool isDashing = false; // ���݃_�b�V�������ǂ���
    private float dashEndTime = 0; // �_�b�V���I������

    private Rigidbody2D rb; // Rigidbody2D�R���|�[�l���g�ւ̎Q��

    public float blinkInterval = 0.2f; // �_�ł̊Ԋu�i�b�P�ʁj

    private SpriteRenderer spriteRenderer; // �X�v���C�g�����_���[�ւ̎Q��
    private float blinkTimer = 0f; // �_�ł̂��߂̃^�C�}�[

    private Color originalColor; // ���̃X�v���C�g�̐F
    public float transparentAlpha = 0.5f; // ���������̃A���t�@�l

    public float damegeinvincible;
    public float Stoptime;

    public TextMeshProUGUI stop_item_text;

    private void Start()
    {
        mainCamera = Camera.main; // ���C���J�����̎Q�Ƃ��擾
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D�R���|�[�l���g���擾
        Magic_Image[0].sprite = type[0];
        Magic_Image[1].sprite = type[1];
        Magic_Image[2].sprite = type[2];

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // �X�v���C�g�̌��̐F��ۑ�

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
        // �_�b�V�����̏ꍇ�A�ʏ�ړ����X�L�b�v
        if (isDashing)
        {
            // �_�b�V�����I�����邩�ǂ������m�F
            if (Time.time >= dashEndTime)
            {
                isDashing = false;
                rb.velocity = Vector2.zero; // �_�b�V���I����ɑ��x�����Z�b�g
            }
            else
            {
                return; // �_�b�V�����͒ʏ�ړ��𖳎�
            }
        }

        // ���͂��ꂽ�ړ��������擾�i�L�[���ƂɊm�F�j
        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector2.right;

        // �_�b�V������
        if (moveDirection != Vector2.zero)
        {
            if (moveDirection == lastMoveDirection && Time.time - lastTapTime < doubleTapTime && !isDashing)
            {
                Dash(moveDirection); // �_�b�V�������s
                isDashing = true;
                dashEndTime = Time.time + dashDuration; // �_�b�V���I�����Ԃ�ݒ�
            }

            // �Ō�̓��͕����Ǝ��Ԃ��X�V
            lastMoveDirection = moveDirection;
            lastTapTime = Time.time;
        }

        // �ʏ�ړ�
        if (!isDashing) // �_�b�V�����łȂ��ꍇ�ɂ̂ݒʏ�̈ړ���K�p
        {
            moveDirection = Vector2.zero; // �ړ������̃��Z�b�g

            if (Input.GetKey(KeyCode.W)) moveDirection = Vector2.up;
            if (Input.GetKey(KeyCode.S)) moveDirection = Vector2.down;
            if (Input.GetKey(KeyCode.A)) moveDirection = Vector2.left;
            if (Input.GetKey(KeyCode.D)) moveDirection = Vector2.right;

            rb.velocity = moveDirection * speed;
        }
        // �J�����͈͓̔��Ƀv���C���[�𐧌�
        ClampPlayerPosition();
        HandleInvincibility();
        HandleBlinking(); // �_�ŏ������Ăяo��
    }



    private void Dash(Vector2 direction)
    {
        // �_�b�V���̏u�ԓI�ȉ������C���p���X�ŉ�����
        rb.velocity = Vector2.zero; // �_�b�V���O�ɑ��x�����Z�b�g
        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);
        ActivateInvincibility(dashDuration); // ���G��Ԃ��Ǘ�
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
        invincible_time = duration; // �����œn���ꂽ���Ԃ𖳓G���Ԃɐݒ�
    }

    private void HandleInvincibility()
    {
        if (invincible)
        {
            // ���G���Ԃ����炷
            invincible_time -= Time.deltaTime;

            // ���G���Ԃ��I�������疳�G��Ԃ�����
            if (invincible_time <= 0)
            {
                invincible = false;
                spriteRenderer.color = originalColor; // �F�����ɖ߂�
                spriteRenderer.enabled = true; // �X�v���C�g��\��
            }
        }
    }

    private void HandleBlinking()
    {
        // invincible��true�̊Ԃ����_�ŏ������s��
        if (invincible)
        {
            // �^�C�}�[���X�V
            blinkTimer += Time.deltaTime;

            // �^�C�}�[���_�ŊԊu�𒴂�����X�v���C�g�̕\��/��\����؂�ւ���
            if (blinkTimer >= blinkInterval)
            {
                // �X�v���C�g�̕\��/��\���𔽓]
                spriteRenderer.enabled = !spriteRenderer.enabled;

                // �^�C�}�[�����Z�b�g
                blinkTimer = 0f;
            }

            // �X�v���C�g���\������Ă���Ƃ��ɔ������ɂ���
            if (spriteRenderer.enabled)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, transparentAlpha);
            }
        }
        else
        {
            // invincible��false�̂Ƃ��X�v���C�g����ɕ\�����A�F�����ɖ߂�
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
    }

    private void HandleCharging()
    {
        // �`���[�W����
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
            ActivateInvincibility(damegeinvincible); // ���G��Ԃ��Ǘ�
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
