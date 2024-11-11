using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timemanager : MonoBehaviour
{

    public static Timemanager Instance { get; private set; }

    public bool stop = false;

    public float time;
    public float curret_time;
    public GameObject stoppanel;
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time == -1) return;

        if(time > curret_time && stop)
        {
            curret_time += Time.deltaTime;
            if(time <= curret_time)
            {
                curret_time = 0;
                start();
            }
        }
    }

    public void Stop()
    {
        stoppanel.SetActive(true);
        stop = true;
        this.time = -1;
    }

    public void Stop(float time)
    {
        stoppanel.SetActive(true);
        stop = true;
        this.time = time;
    }

    public void start()
    {
        stoppanel.SetActive(false);
        stop = false;
        this.time = -1;
    }
}
