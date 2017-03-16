using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitMananger : MonoBehaviour {
    public GameObject[] ExitGoList;

	// 随机显示出口
	void Start () {
        int exitNum = Random.Range(0, ExitGoList.Length);
        ExitGoList[exitNum].SetActive(true);
    }
}
