using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class AILitterFish : MonoBehaviour {

    private Animator m_Animator;

    [SerializeField]
    private Vector2 m_Rect;

    [SerializeField]
    private Vector2 m_MoveTimeRange = new Vector2(3, 5);
    
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void OnEnable()
    {
        StartCruise();
    }

    public void StartCruise()
    {
        float moveTime = UnityEngine.Random.Range(m_MoveTimeRange.x, m_MoveTimeRange.y);
        Vector3 targetPoint = new Vector3(UnityEngine.Random.Range(-m_Rect.x / 2.0f, m_Rect.x / 2.0f),
            UnityEngine.Random.Range(-m_Rect.y / 2.0f, m_Rect.y / 2.0f), transform.localPosition.z);
        Vector3 dict = targetPoint - transform.localPosition;
        dict.z = transform.localPosition.z;
        float angle = Vector3.Angle(Vector2.up, dict);
        if (dict.x > 0) angle = -angle;
        transform.DOLocalMove(dict, moveTime).OnComplete(StartCruise);
        transform.DORotate(new Vector3(0, 0, angle), 0.5f);
    }

    public void Waite()
    {
        transform.DOKill();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Story1MonsterFish")
        {
            gameObject.SetActive(false);
        }
    }

    public void MoveTo(Vector3 targetPoint, float moveTime, TweenCallback endCallback)
    {
        Vector3 dict = targetPoint - transform.position;
        dict.z = transform.position.z;
        float angle = Vector3.Angle(Vector2.up, dict);
        if (dict.x > 0) angle = -angle;
        transform.DOMove(targetPoint, moveTime).OnComplete(endCallback);
        transform.DORotate(new Vector3(0, 0, angle), 0.5f);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Story1MonsterFish")
        {
            gameObject.SetActive(false);
        }
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Story1MonsterFish")
        {
            gameObject.SetActive(false);
        }
    }
}
