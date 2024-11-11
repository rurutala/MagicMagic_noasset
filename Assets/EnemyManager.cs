using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    public GameObject prefab; // 生成するプレハブ
    public Transform spawnPoint; // スポーンする場所
    public float spawnDelay = 1f; // スポーンまでの遅延時間
}

public class EnemyManager : MonoBehaviour
{
    public List<SpawnSettings> spawnSettingsList; // 複数の生成設定のリスト

    public bool iscalled = false;
    public bool finish = false;

    public void enemy_start()
    {
        iscalled = true;
        finish = false; // 生成完了までfalseのまま
        StartCoroutine(SpawnAllEnemies()); // 全敵を生成するコルーチンを呼ぶ
    }

    private IEnumerator SpawnAllEnemies()
    {
        foreach (var settings in spawnSettingsList)
        {
            yield return StartCoroutine(SpawnPrefabWithDelay(settings));
        }
        finish = true; // すべての生成が完了したらtrueにする
    }

    private IEnumerator SpawnPrefabWithDelay(SpawnSettings settings)
    {
        yield return new WaitForSeconds(settings.spawnDelay); // 遅延時間を待機
        Instantiate(settings.prefab, settings.spawnPoint.position, Quaternion.identity, transform); // 親オブジェクトに設定
    }

    private void Update()
    {
        if (!iscalled || !finish) return; // 生成が完了していない間は何も行わない

        // 全ての生成が完了しており、かつ子オブジェクトがない場合にのみ関数を呼び出す
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
