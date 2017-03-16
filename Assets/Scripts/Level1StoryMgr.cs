using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1StoryMgr : MonoBehaviour {

    [SerializeField]
    private FSequence m_FSquence1;

    [SerializeField]
    private FSequence m_FSquence2;

    [SerializeField]
    private FSequence m_FSquence3;

    [SerializeField]
    private FSequence m_FSquence4;

    [SerializeField]
    private float m_Squence2Interval = 10f;

    [SerializeField]
    private float m_Squence3Interval = 20f;

    [SerializeField]
    private Story1 m_Story1;

    bool hasPlayerEndSquence1 = false;
    bool hasPlayerEndSquence2 = false;
    bool hasPlayerEndSquence3 = false;
    bool hasPlayerEndSquence4 = false;

    public void Squence1PlayEnd()
    {
        hasPlayerEndSquence1 = true;
    }

    public void Squence2PlayEnd()
    {
        hasPlayerEndSquence2 = true;
    }

    public void Squence3PlayEnd()
    {
        hasPlayerEndSquence3 = true;
        m_Story1.gameObject.SetActive(false);
    }

    public void Squence4PlayEnd()
    {
        hasPlayerEndSquence4 = true;
    }

    public void AILitterFishKillEnd()
    {
        if (hasPlayerEndSquence2 && !hasPlayerEndSquence3)
        {
            m_FSquence3.Play();
        }
    }

    void Update()
    {
        if (Shoal.Instance.IsDeath) return;
        if (hasPlayerEndSquence1 && !hasPlayerEndSquence2 && !m_FSquence2.IsPlaying)
        {
            m_Squence2Interval -= Time.deltaTime;
            if (m_Squence2Interval < 0) m_FSquence2.Play();
        }
        if(hasPlayerEndSquence3 && !hasPlayerEndSquence4 && !m_FSquence4.IsPlaying)
        {
            m_Squence3Interval -= Time.deltaTime;
            if (m_Squence3Interval < 0) m_FSquence4.Play();
        }
    }
}
