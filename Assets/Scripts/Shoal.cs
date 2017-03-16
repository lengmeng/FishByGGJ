using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

/// <summary>
/// 鱼群控制
/// </summary>
public class Shoal : MonoBehaviour
{
    public enum FishEnum
    {
        // 速度快的鱼
        Speed,
        // 隐匿性的鱼
        Hide,
        // 大视野的鱼
        GreatlyVision,
        FishCount
    }

    /// <summary>
    /// 当前选择的鱼类型
    /// </summary>
    private FishEnum m_CurrentFishType;

    /// <summary>
    /// 速度快的鱼
    /// </summary>
    [SerializeField]
    private FishInfo m_SpeedFish;

    /// <summary>
    /// 隐匿性强的鱼
    /// </summary>
    [SerializeField]
    private FishInfo m_HideFish;

    /// <summary>
    /// 大视野的鱼
    /// </summary>
    [SerializeField]
    private FishInfo m_GreatlyVisionFish;

    /// <summary>
    /// 小鱼的感知范围
    /// </summary>
    [SerializeField]
    private GameObject InteractionRange;

    /// <summary>
    /// 小鱼的动作控制器
    /// </summary>
    private List<Animator> m_FishSpriteAnimations;
    
    [SerializeField]
    public GameObject[] m_FishList;

    private Rigidbody2D m_Rigidbody2D;

    public static Shoal Instance { get; private set; }

    /// <summary>
    /// 鱼群死亡事件
    /// </summary>
    public event Action OnDethEvent = delegate { };

    /// <summary>
    /// 是否已经死亡
    /// </summary>
    public bool IsDeath { get; private set; }

    /// <summary>
    /// 鱼群是否在奔跑状态.
    /// </summary>
    private bool m_IsRunning;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        SwitchFish(FishEnum.Hide);
        Instance = this;
        m_IsRunning = false;
        m_FishSpriteAnimations = new List<Animator>();
        foreach(GameObject go in m_FishList)
        {
            m_FishSpriteAnimations.Add(go.GetComponent<Animator>());
        }
        IsDeath = false;
        canChange = true;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public FishInfo GetCurrentFish()
    {
        switch (m_CurrentFishType)
        {
            case FishEnum.Speed: return m_SpeedFish;
            case FishEnum.Hide: return m_HideFish;
            case FishEnum.GreatlyVision: return m_GreatlyVisionFish;
            default: return null;
        }
    }

    public void Move(float xMove, float yMove)
    {
        // 要设置鱼为静止状态
        FishInfo fishInfo = GetCurrentFish();
        m_Rigidbody2D.velocity = new Vector2(xMove * fishInfo.Speed, yMove * fishInfo.Speed);
        if (Mathf.Abs(xMove) < 0.001f && Mathf.Abs(yMove) < 0.001f)
        {
            if (m_IsRunning)
            {
                FishIdeo();
                m_IsRunning = false;
            }
            return;
        }
        if (!m_IsRunning)
        {
            m_IsRunning = true;
            FishRunning();
        }
        float angle = Vector3.Angle(Vector2.up, m_Rigidbody2D.velocity);
        if (xMove > 0) angle = -angle;
        transform.DORotate(new Vector3(0, 0, angle), 0.5f);

    }

    public void FishRunning()
    {
        if (m_FishSpriteAnimations == null) return;
        foreach (var it in m_FishSpriteAnimations)
        {
            it.SetTrigger("Running");
        }
    }

    public void FishIdeo()
    {
        if (m_FishSpriteAnimations == null) return;
        foreach (var it in m_FishSpriteAnimations)
        {
            it.SetTrigger("Ideo");
        }
    }

    public void SwitchFish(FishEnum fish)
    {
        // 简单粗暴
        if (m_FishList.Length <= 2 && fish == FishEnum.Speed) return;

        if (canChange)
        {
            m_CurrentFishType = fish;
            FishInfo fishInfo = GetCurrentFish();
            Camera.main.DOOrthoSize(fishInfo.VisionSize, 0.5f);
            InteractionRange.transform.localScale = new Vector3(fishInfo.InteractionRanage * 2, fishInfo.InteractionRanage * 2, 1);
            PosChange(fish);
        }
    }

    public void HideAllFish()
    {
        foreach (GameObject go in m_FishList)
        {
            go.SetActive(false);
        }
    }

    /// <summary>
    /// 被吃的接口
    /// </summary>
    public void Devour()
    {
        IsDeath = true;
        OnDethEvent.Invoke();
    }
    
    string[] names = { "BlueFish", "RedFish", "YellowFish" };
    private bool canChange;
    /// <summary>
    /// 改变鱼的位置
    /// </summary>
    /// <param name="fish"></param>
    void PosChange(FishEnum fish)
    {
        string name = names[(int)fish];
        for (int i = 0; i < m_FishList.Length; i++)
        {
            GameObject go = m_FishList[i];
            if (go.name == name && i != 0)
            {
                GameObject headFish = m_FishList[0];
                m_FishList[0] = go;
                m_FishList[i] = headFish;
                canChange = false;
                PosChangeAnimation(headFish, go);
            }
        }
    }
    /// <summary>
    /// 变换动画
    /// </summary>
    void PosChangeAnimation(GameObject go1, GameObject go2)
    {
        Vector3 pos1 = go1.transform.localPosition;
        Vector3 pos2 = go2.transform.localPosition;

        go1.transform.DOLocalMove(pos2, 0.8f);
        go2.transform.DOLocalMove(pos1, 1.0f).OnComplete(OnMoveFish);
    }
    void OnMoveFish()
    {
        canChange = true;
    }
}