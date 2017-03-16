using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFishManager : MonoBehaviour {
    static MonsterFishManager _instance;

    public GameObject MFish;
    public int FishCount = 4;

    public bool isDangerous = false;

    GameObject[] FishList = new GameObject[20];
    public static MonsterFishManager GetInstance()
    {
        return _instance;
    }

    void Start () {
        _instance = this;
        for (int i = 0; i < FishCount; i++)
        {
            float dis = 0;
            Vector3 pos = new Vector3(0,0,0);
            while (dis <= 150)
            {
                pos = new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), 0);
                dis = (Shoal.Instance.transform.position - pos).magnitude;
            }
            GameObject fish = (GameObject)Instantiate(MFish,
                pos, new Quaternion(0,0,0,0));
            FishList[i] = fish;
        }
        
    }

    void Update()
    {
        isDangerous = false;
        for (int i = 0; i < FishCount; i++)
        {
            GameObject go = FishList[i];
            if (go != null && go.GetComponent<MonsterFish>().isChase)
                isDangerous = true;
        }
    }

    public void StopGame()
    {
        for (int i = 0; i < FishCount; i++)
        {
            GameObject go = FishList[i];
            go.GetComponent<MonsterFish>().StopMonster();
        }
    }
}
