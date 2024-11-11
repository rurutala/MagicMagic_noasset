using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magic_test1 : MonoBehaviour
{


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
        if (other.tag == "PlayerAttack" && !BackgroundMove.Instance.ismoving && other.GetComponent<Bullet>().getType() == TYPE.Water && other.GetComponent<Bullet>().attack >= 3)
        {
            BackgroundMove.Instance.ismoveChange();
            Destroy(this.gameObject);
        }
    }
}
