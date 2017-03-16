using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class Story1 : MonoBehaviour {

    [SerializeField]
    private MonsterFishForStory m_MosterFish;
    [SerializeField]
    private Shoal m_Shoal;
    [SerializeField]
    private AILitterFish m_AiFish;
    [SerializeField]
    private Level1StoryMgr m_StoryMgr;

    [SerializeField]
    private Camera2DFollow m_Camera2DFollow;

    private void Awake()
    {
        InitStroy();
    }

    /// <summary>
    /// 初始化剧情
    /// </summary>
	public void InitStroy()
    {
        Vector3 target = Vector3.zero;
        Bounds bounds = Edge.Instance.Range;
        bounds.size -= new Vector3(30, 30, 0);
        float offset = 114;
        while (true)
        {
            target = m_Shoal.transform.position + new Vector3(0, offset, 0);
            if (bounds.Contains(target)) break;
            target = m_Shoal.transform.position + new Vector3(offset, 0, 0);
            if (bounds.Contains(target)) break;
            target = m_Shoal.transform.position + new Vector3(-offset, 0, 0);
            if (bounds.Contains(target)) break;
            target = m_Shoal.transform.position + new Vector3(0, -offset, 0);
            if (bounds.Contains(target)) break;
            break;
        }
        transform.position = target;
        m_Shoal.Move(0, 0);
    }

    /// <summary>
    /// 移动摄像机到目标地点
    /// </summary>
    public void MoveCameraToTarget()
    {
        m_Camera2DFollow.enabled = false;
        m_Camera2DFollow.gameObject.transform.DOMove(new Vector3(transform.position.x, transform.position.y, m_Camera2DFollow.transform.position.z), 2);
    }

    /// <summary>
    /// 怪物出现
    /// </summary>
    public void MonsterAppear()
    {
        m_MosterFish.SetTarget(m_AiFish.transform);
        m_MosterFish.OnAttackFinishEvent += (x) =>
        {
            m_MosterFish.gameObject.SetActive(false);
            m_StoryMgr.AILitterFishKillEnd();
        };
    }
    

    /// <summary>
    /// 摄像机返回
    /// </summary>
    public void MoveCamerReturn()
    {
        m_Camera2DFollow.gameObject.transform.DOMove(new Vector3(m_Shoal.transform.position.x,
            m_Shoal.transform.position.y, m_Camera2DFollow.gameObject.transform.position.z), 1)
            .OnComplete(() => m_Camera2DFollow.enabled = true);
    }
}
