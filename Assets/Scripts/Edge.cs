using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {

    public static Edge Instance { get; private set; }
    [SerializeField]
    private float EdgeWidth = 1f;
    public Bounds Range;
    [SerializeField]
    private GameObject m_TopEdge;
    [SerializeField]
    private GameObject m_BottomEdge;
    [SerializeField]
    private GameObject m_RightEdge;
    [SerializeField]
    private GameObject m_LeftEdge;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitEdge();
    }

#if UNITY_EDITOR
    void Update()
    {
        InitEdge();
    }
#endif

    void InitEdge()
    {
        float halfEdgeWidth = EdgeWidth / 2.0f;
        m_TopEdge.transform.position = new Vector3(0, Range.size.y / 2 + halfEdgeWidth, 0);
        m_TopEdge.transform.localScale = new Vector3(Range.size.x, 1, 1);
        m_BottomEdge.transform.position = new Vector3(0, -Range.size.y / 2 - halfEdgeWidth, 0);
        m_BottomEdge.transform.localScale = new Vector3(Range.size.x, 1, 1);
        m_RightEdge.transform.position = new Vector3(Range.size.x / 2 + halfEdgeWidth, 0, 0);
        m_RightEdge.transform.localScale = new Vector3(1, Range.size.y, 1);
        m_LeftEdge.transform.position = new Vector3(-Range.size.x / 2 - halfEdgeWidth, 0, 0);
        m_LeftEdge.transform.localScale = new Vector3(1, Range.size.y, 1);
    }
    void OnDestroy()
    {
        Instance = null;
    }
}
