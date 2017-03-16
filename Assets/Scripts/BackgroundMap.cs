using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMap : MonoBehaviour
{

    [SerializeField]
    private Sprite[] m_BgSprites;

    [SerializeField]
    private float m_ZDepth;

    [SerializeField]
    private GameObject[] m_Effect;

    // Use this for initialization
    void Start()
    {
        InitBackground();
        InitEffect();
    }

    void InitBackground()
    {
        if (m_BgSprites == null || m_BgSprites.Length <= 0) return;
        GameObject[] spriteObjectes = new GameObject[m_BgSprites.Length];
        for (int i = 0; i < m_BgSprites.Length; i++)
        {
            GameObject obj = new GameObject("[SptriteGameObjectPrefab]");
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = m_BgSprites[i];
            spriteObjectes[i] = obj;
        }
        Bounds mapBounds = new Bounds(Vector3.zero, new Vector3(500, 500, 0));
        // 如果有边界存在的话,就用边界的大小
        if (Edge.Instance != null)
        {
            mapBounds = Edge.Instance.Range;
            // 地图大小要比边界大
            mapBounds.size += new Vector3(50, 50, 0);
        }
        Bounds spriteBounds = spriteObjectes[0].GetComponent<SpriteRenderer>().bounds;
        for(float x = mapBounds.min.x; x <= mapBounds.max.x; x += spriteBounds.size.x)
        {
            for(float y = mapBounds.min.y; y <= mapBounds.max.y; y += spriteBounds.size.y)
            {
                int spriteIndex = Random.Range(0, m_BgSprites.Length - 1);
                GameObject obj = GameObject.Instantiate(spriteObjectes[spriteIndex], transform);
                obj.transform.position = new Vector3(x, y, transform.position.z);
                obj.SetActive(true);
            }
        }
    }


    void InitEffect()
    {
        if (m_Effect == null || m_Effect.Length <= 0) return;
        GameObject[] spriteObjectes = new GameObject[m_Effect.Length];
        for (int i = 0; i < m_Effect.Length; i++)
        {
            GameObject obj = new GameObject("[SptriteGameObjectPrefab]");
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            spriteObjectes[i] = m_Effect[i];
        }
        Bounds mapBounds = new Bounds(Vector3.zero, new Vector3(500, 500, 0));
        // 如果有边界存在的话,就用边界的大小
        if (Edge.Instance != null)
        {
            mapBounds = Edge.Instance.Range;
            // 地图大小要比边界大
            mapBounds.size += new Vector3(50, 50, 0);
        }
        Vector2 Size = new Vector2(100f, 100f);
        for (float x = mapBounds.min.x; x <= mapBounds.max.x; x += Size.x)
        {
            for (float y = mapBounds.min.y; y <= mapBounds.max.y; y += Size.y)
            {
                int spriteIndex = Random.Range(0, m_Effect.Length - 1);
                GameObject obj = GameObject.Instantiate(spriteObjectes[spriteIndex], transform);
                obj.transform.position = new Vector3(x, y, transform.position.z - 15);
                obj.SetActive(true);
            }
        }
    }
}
