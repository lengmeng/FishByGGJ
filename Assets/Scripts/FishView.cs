using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小鱼控制器
/// </summary>
public class FishView : MonoBehaviour {
    /// <summary>
    /// 小鱼的运动帧
    /// </summary>
    [SerializeField]
    private Sprite[] m_RunningSprites;

    private SpriteRenderer m_SpriteRenderer;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSpritesIndex(int index)
    {
        if (m_SpriteRenderer == null || index < 0 || index >= m_RunningSprites.Length)
            return;
        m_SpriteRenderer.sprite = m_RunningSprites[index];
    }
}