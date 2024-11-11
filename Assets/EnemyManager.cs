using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    public GameObject prefab; // ��������v���n�u
    public Transform spawnPoint; // �X�|�[������ꏊ
    public float spawnDelay = 1f; // �X�|�[���܂ł̒x������
}

public class EnemyManager : MonoBehaviour
{
    public List<SpawnSettings> spawnSettingsList; // �����̐����ݒ�̃��X�g

    public bool iscalled = false;
    public bool finish = false;

    public void enemy_start()
    {
        iscalled = true;
        finish = false; // ���������܂�false�̂܂�
        StartCoroutine(SpawnAllEnemies()); // �S�G�𐶐�����R���[�`�����Ă�
    }

    private IEnumerator SpawnAllEnemies()
    {
        foreach (var settings in spawnSettingsList)
        {
            yield return StartCoroutine(SpawnPrefabWithDelay(settings));
        }
        finish = true; // ���ׂĂ̐���������������true�ɂ���
    }

    private IEnumerator SpawnPrefabWithDelay(SpawnSettings settings)
    {
        yield return new WaitForSeconds(settings.spawnDelay); // �x�����Ԃ�ҋ@
        Instantiate(settings.prefab, settings.spawnPoint.position, Quaternion.identity, transform); // �e�I�u�W�F�N�g�ɐݒ�
    }

    private void Update()
    {
        if (!iscalled || !finish) return; // �������������Ă��Ȃ��Ԃ͉����s��Ȃ�

        // �S�Ă̐������������Ă���A���q�I�u�W�F�N�g���Ȃ��ꍇ�ɂ̂݊֐����Ăяo��
        if (transform.childCount == 0)
        {
            AllPrefabsDestroyed();
        }
    }

    private void AllPrefabsDestroyed()
    {
        Debug.Log("All prefabs have been destroyed.");
        BackgroundMove.Instance.ismoveChange();
        Destroy(this.gameObject);
    }
}
