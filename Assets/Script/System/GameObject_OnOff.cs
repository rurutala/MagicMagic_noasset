using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObject_OnOff : MonoBehaviour
{
    public GameObject Target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeONOFF()
    {
        Target.SetActive(!Target.activeSelf);
    }
}
