using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainSceneMgr : MonoBehaviour {
    public int currLevelNum;
    public int nextLevelNum;

    public static MainSceneMgr _instance;
    // Use this for initialization
    void Start () {
        _instance = this;
        Shoal.Instance.OnDethEvent += OnGameEnd;
        Musiccontroller._instance.PlayMusic(0);
    }

    void Update()
    {
        if (MonsterFishManager.GetInstance() != null)
        {
            if (!MonsterFishManager.GetInstance().isDangerous)
            {
                if (Musiccontroller._instance.currMusicNum != 0)
                    Musiccontroller._instance.PlayMusic(0);
            }
            else
            {
                if (Musiccontroller._instance.currMusicNum != 1)
                    Musiccontroller._instance.PlayMusic(1);
            }
        }
    }

    public static MainSceneMgr GetInstance()
    {
        return _instance;
    }

    public void OnClick()
    {
        Transform ts = transform.Find("GGWin");
        ts.gameObject.SetActive(false);
        SceneManager.LoadScene(currLevelNum);
    }

    void OnGameEnd()
    {
        Transform ts = transform.Find("GGWin");
        ts.gameObject.SetActive(true);
        MonsterFishManager.GetInstance().StopGame();
    }

}
