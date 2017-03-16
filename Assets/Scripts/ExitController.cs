using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ExitController : MonoBehaviour {
    public float ExitRange;
    int nextLevel;
	// Use this for initialization
	void Start () {
        nextLevel = MainSceneMgr.GetInstance().nextLevelNum;
    }
	
	// Update is called once per frame
	void Update () {
        float distance = (Shoal.Instance.transform.position - transform.position).magnitude;
        if (distance <= ExitRange)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
