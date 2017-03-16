using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MonsterFishForStory : MonoBehaviour{
    // 大鱼的当前状态
    [SerializeField]
    MFishStatus currStatus;

    //大鱼移动速度
    [SerializeField]
    private float MFSpeed = 10;
    // 大鱼冲刺速度
    [SerializeField]
    float MFSprint = 200;
    // 大鱼冲刺距离
    [SerializeField]
    float MFSprintDis = 50;
    // 大鱼检测范围
    [SerializeField]
    float ChaseRange = 40;
    // 大鱼冲刺范围
    [SerializeField]
    float SkillRange = 20;
    // 鱼群巡视范围
    [SerializeField]
    Vector4 PartolArea = new Vector4(-100, 100, -100, 100);

    Tween patrol_tween;
    Tween chase_tween;

    public event Action<Transform> OnAttackFinishEvent = delegate { };

    Transform targetTrn;
    

    void Start () {
        currStatus = MFishStatus.CHASE;
    }
    // 简单粗暴的检测
    void Update()
    {
        if (targetTrn != null)
            StatusUpdate();
    }

    public void SetTarget(Transform tf)
    {
        targetTrn = tf;
    }

    /// <summary>
    /// 设置大鱼的巡视范围
    /// </summary>
    /// <param name="v4">巡视方位xy表示x轴范围，zw表示y轴范围</param>
    public void SetPartolArea(Vector4 v4)
    {
        PartolArea = v4;
    }
   
    /// <summary>
    /// 状态检测
    /// </summary>
    void StatusUpdate()
    {
        //print("当前状态" + currStatus);
        switch (currStatus)
        {
            case MFishStatus.SLEEP:
                AllKill();
                currStatus = MFishStatus.PATROL;
                Partol();
                break;
            case MFishStatus.PATROL:
                DetectionUpdate();
                break;
            case MFishStatus.CHASE:
                AllKill();
                ChaseFish();
                break;
            case MFishStatus.SKILL:
                currStatus = MFishStatus.SKILLING;
                AllKill();
                UseSkill();
                break;
            case MFishStatus.READYSKILL:
                currStatus = MFishStatus.SKILLING;
                AllKill();
                ReadySkill();
                break;
            case MFishStatus.SKILLING: // 不可被打断的状态
                break;
        }
    }

    /// <summary>
    /// 获取单位向量
    /// </summary>
    /// <returns></returns>
    Vector2 GetDirInfo()
    {
        float val = (transform.eulerAngles.z + 90) % 360.0f;
        return new Vector2(Mathf.Cos(val * Mathf.Deg2Rad), Mathf.Sin(val * Mathf.Deg2Rad));
    }

    /// <summary>
    /// 准备技能
    /// </summary>
    void ReadySkill()
    {
        // 显示范围提示
        Transform ts = transform.Find("WarningRange");
        ts.gameObject.SetActive(true);

        Tween tween = DOTween.To(() => transform.position, r => transform.position = r, transform.position, 3);
        tween.OnComplete<Tween>(OnSkillComplete);
    }

    /// <summary>
    /// 技能准备完毕
    /// </summary>
    void OnSkillComplete()
    {
        Transform ts = transform.Find("WarningRange");
        ts.gameObject.SetActive(false);
        currStatus = MFishStatus.SKILL;
    }

    /// <summary>
    /// 发动技能
    /// </summary>
    void UseSkill()
    {
        Vector2 v2 = GetDirInfo();
        Vector3 SkillTarget = new Vector3(transform.position.x + v2.x * MFSprintDis, 
            transform.position.y + MFSprintDis * v2.y, transform.position.z);
        Tween tween = DOTween.To(() => transform.position, r => transform.position = r, SkillTarget,
            (transform.position - SkillTarget).magnitude / MFSprint);
        tween.SetEase(Ease.Linear);
        tween.OnComplete<Tween>(OnPartolComplete);
    }
    
    /// <summary>
    /// 侦查圈检测
    /// </summary>
    void DetectionUpdate()
    {
        float distance = (targetTrn.position - transform.position).magnitude;
        if (distance <= SkillRange)
        {
            currStatus = MFishStatus.READYSKILL;
        }
        else if (distance <= ChaseRange)
        {
            currStatus = MFishStatus.CHASE;
        }
        else if (currStatus == MFishStatus.CHASE)
        {
            currStatus = MFishStatus.SLEEP;
        }

    }

    /// <summary>
    /// 追击小鱼
    /// </summary>
    void ChaseFish()
    {
        Vector3 fishPos = targetTrn.position;

        Vector3 dir = fishPos - transform.position;
        Vector3 velocity = new Vector3(dir.x, dir.y, 0);
        float angle = Vector3.Angle(Vector3.up, velocity);
        if (dir.x > 0) angle = -angle;
        transform.DORotate(new Vector3(0, 0, angle), 0.1f);

        chase_tween = DOTween.To(() => transform.position, r => transform.position = r, fishPos,
            (transform.position - fishPos).magnitude / MFSpeed);
        chase_tween.SetEase(Ease.Linear);
        DetectionUpdate();
    }


    /// <summary>
    /// 大鱼随机巡逻
    /// </summary>
    void Partol()
    {
        Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(PartolArea.x, PartolArea.y), UnityEngine.Random.Range(PartolArea.z, PartolArea.w), 0);

        Vector3 dir = randomPoint - transform.position;
        Vector2 velocity = new Vector2(dir.x, dir.y);
        float angle = Vector3.Angle(Vector2.up, velocity);
        if (dir.x > 0) angle = -angle;
        transform.DORotate(new Vector3(0, 0, angle), 0.5f);

        patrol_tween = DOTween.To(() => transform.position, r => transform.position = r, randomPoint,
            (transform.position - randomPoint).magnitude / MFSpeed);

        patrol_tween.SetEase(Ease.Linear);
        patrol_tween.OnComplete<Tween>(OnPartolComplete);
    }
    /// <summary>
    /// 巡逻结束时
    /// </summary>
    private void OnPartolComplete()
    {
        currStatus = MFishStatus.SLEEP;
        OnAttackFinishEvent.Invoke(targetTrn);
    }


    void AllKill()
    {
        chase_tween.Kill();
        patrol_tween.Kill();
    }
    
}
