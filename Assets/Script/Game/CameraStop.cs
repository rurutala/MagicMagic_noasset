using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStop : MonoBehaviour
{
    public bool called = false;

    public EnemyManager enemymanager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 衝突したオブジェクトがメインカメラか確認
        if (other.CompareTag("MainCamera") && !called && BackgroundMove.Instance.ismoving)
        {
            BackgroundMove.Instance.ismoveChange();
            called = true;
        }
        if(enemymanager != null)
        {
            enemymanager.enemy_start();
            enemymanager = null;
        }
    }
}