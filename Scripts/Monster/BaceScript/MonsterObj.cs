using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 怪物对象类
/// </summary>

public class MonsterObj : EntityObj
{
    [Header("怪物额外基础信息")]
    /// <summary>
    /// 怪物奔跑速度
    /// </summary>
    public float runSpeed;
    [Header("观测数据")]
    /// <summary>
    /// 玩家控制角色
    /// </summary>
    public PlayerController player;
    /// <summary>
    /// 用于 处理AI逻辑处理的 对象
    /// </summary>
    protected AILogic aiLogic;
    /// <summary>
    /// 记录所需要的等待时间
    /// </summary>
    protected float waitTime;
    /// <summary>
    /// 玩家在怪物的左边还是右边 true为右
    /// </summary>
    public bool isplayerRight 
    {
        get
        {
            if (Vector3.Cross(Vector3.up,player.transform.position-transform.position).z<0)//左边
                return false;
            return true;
        }
    }
    /// <summary>
    /// 检测与玩家是否相撞
    /// </summary>
    public bool isCollidPlayer 
    {
        get
        {
            //两个矩形相交的条件：两个矩形的重心距离在x轴y轴上都小于两个矩形长或宽的一半之和。
            //重心距离在x轴上的投影长度<两个矩形的在x轴的长度之和/2
            //重心距离在y轴上的投影长度 < 两个矩形在y轴上的宽度之和 / 2
            if (Mathf.Abs(roleTopDeterminePoint.x-player.roleTopDeterminePoint.x)<(roleWidth+player.roleWidth)/2&&
               Mathf.Abs(roleLeftDeterminePoint.y - player.roleLeftDeterminePoint.y) < (roleHight + player.roleHight) / 2)
                return true;
            return false;
        }
    }
    protected override void Start()
    {
        base.Start();
        //初始化AI逻辑对象
        aiLogic = new Boar_AILogic(this);
        //初始化玩家对象
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    public override void InitInfo()
    {
        base.InitInfo();
    }
    protected override void Update()
    {
        base.Update();
        //更新ai逻辑
        aiLogic.UpdateAIState();
    }

    /// <summary>
    /// 用于设置动画切换的函数
    /// </summary>
    /// <param name="type"></param>
    public virtual void ChangeAnimation(E_Animation_Monster_Type type)
    {
    }
    
}
    
