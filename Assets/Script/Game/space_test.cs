using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class space_test : MonoBehaviour
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
        if((other.tag == "PlayerAttack" || other.tag == "Player") && !BackgroundMove.Instance.ismoving)
        {
            BackgroundMove.Instance.ismoveChange();
            Destroy(this.gameObject);
        }
    }
}
