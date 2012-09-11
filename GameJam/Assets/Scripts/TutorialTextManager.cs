using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialTextManager : MonoBehaviour {

    bool isFinished = false;
    float timeCache = 0;
    public int index = 0;
    public List<GameObject> msg = new List<GameObject>();

	// Use this for initialization
	void OnEnable () {
        print("enabled");
        for (int i = 0; i < msg.Count; i++)
        {
            msg[i].active = false;
        }
        msg[0].active = true;
        timeCache = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
        if (isFinished)
            this.enabled = false;

        for (int i = 0; i < msg.Count; i++)
        {
            if (index == i)
            {
                if (i != 3 && GameManager.currentPlayMode != CurrentPlayMode.Grey)
                {
                    if (msg[i].active && ((Time.timeSinceLevelLoad - timeCache) % 60) > 10)
                    {
                        msg[i].active = false;
                        timeCache = Time.timeSinceLevelLoad;
                        if (index == msg.Count-1)
                        {
                            isFinished = true;
                        }
                        index++;
                    }
                    else if (!msg[i].active && ((Time.timeSinceLevelLoad - timeCache) % 60) > 1)
                    {
                        msg[i].active = true;
                        timeCache = Time.timeSinceLevelLoad;
                    }
                }
                else
                {
                    isFinished = true;
                }
                return;
            }
        }	
    }
}
