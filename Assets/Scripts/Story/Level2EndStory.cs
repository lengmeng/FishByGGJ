using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2EndStory : MonoBehaviour {

    [SerializeField]
    private AILitterFish m_RedFish;
    
    [SerializeField]
    private AILitterFish m_YellowFish;

    [SerializeField]
    private MonsterFishForStory m_MonsterFish;

    [SerializeField]
    private Transform m_ExitPoint;

    [SerializeField]
    private int m_NextIndexSceneIndex;

    [SerializeField]
    private float m_WaitTime = 10;
    int m_KillNum = 0;

    [SerializeField]
    private FSequence m_FSequence;

    public void FishMove()
    {
        m_RedFish.Waite();
        m_RedFish.MoveTo(m_ExitPoint.position, 12, null);
        m_YellowFish.Waite();
        m_YellowFish.MoveTo(m_ExitPoint.position, 10, null);
    }

    public void MonsterAppear()
    {
        m_MonsterFish.gameObject.SetActive(true);
        m_MonsterFish.SetTarget(m_ExitPoint);
        m_MonsterFish.OnAttackFinishEvent += ((x) => { AddKillFish(); m_MonsterFish.gameObject.SetActive(false); });
    }

    public void AddKillFish()
    {
        m_KillNum++;
    }

    private void Update()
    {
        if (m_KillNum > 0)
        {
            m_WaitTime -= Time.time;
            if (m_WaitTime < 0)
            {
                m_FSequence.Stop();
                SceneManager.LoadScene(m_NextIndexSceneIndex);
                m_KillNum = 0;
            }
        }
    }
}
