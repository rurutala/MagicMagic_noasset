using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageSelect : MonoBehaviour
{
    public int StageID = 0;
    public int Stage_count = 3;
    public List<string> stage_select;
    public List<int> remian_count;
    public TextMeshProUGUI title;
    public TextMeshProUGUI remain;

    public SceneMove move;

    // Start is called before the first frame update
    void Start()
    {
        title.text = stage_select[StageID];
        remain.text = remian_count[StageID].ToString();
        move.SceneName = "Game" + StageID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void right()
    {
        if (StageID + 2 > Stage_count) return;
        StageID += 1;
        title.text = stage_select[StageID];
        remain.text = remian_count[StageID].ToString();
        move.SceneName = "Game" + StageID;
    }
    public void left()
    {
        Debug.Log("called");
        if (StageID != 0)
        {
            StageID -= 1;
            title.text = stage_select[StageID];
            remain.text = remian_count[StageID].ToString();
            move.SceneName = "Game" + StageID;
        }
    }
}
