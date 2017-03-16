using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1EndStory : MonoBehaviour {

    [SerializeField]
    private AILitterFish m_RedFish;

    [SerializeField]
    private AILitterFish m_BlueFish;

    [SerializeField]
    private AILitterFish m_YellowFish;

    [SerializeField]
    private MonsterFishForStory m_MonsterFish;

    [SerializeField]
    private Transform m_ExitPoint;

    [SerializeField]
    private int m_NextSceneIndex;

    private bool m_IsWaitSwitchScene = false;

    private float m_WaitTime = 5f;

    public void RedFishExit()
    {
        m_RedFish.Waite();
        m_RedFish.MoveTo(m_ExitPoint.position, 2, () => {
            SpriteRenderer spriteRenderer = m_RedFish.transform.GetComponent<SpriteRenderer>();
            spriteRenderer.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(()=>m_RedFish.gameObject.SetActive(false));
            YellowFishExit();
        });
    }

    public void YellowFishExit()
    {
        m_YellowFish.Waite();
        m_YellowFish.MoveTo(m_ExitPoint.position, 2, () =>
        {
            SpriteRenderer spriteRenderer = m_YellowFish.transform.GetComponent<SpriteRenderer>();
            spriteRenderer.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(()=>m_YellowFish.gameObject.SetActive(false));
        });
    }

    public void MonsterAppear()
    {
        m_MonsterFish.SetTarget(m_BlueFish.transform);
        m_MonsterFish.gameObject.SetActive(true);
        m_MonsterFish.OnAttackFinishEvent += (x) => {
            m_IsWaitSwitchScene = true;
        };
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(m_NextSceneIndex);
    }

    public void Update()
    {
        if (!m_IsWaitSwitchScene) return ;
        m_WaitTime -= Time.time;
        if (m_WaitTime < 0) LoadNextScene();
    }
}
