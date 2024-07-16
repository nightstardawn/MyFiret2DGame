using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
public class EntityObj : MonoBehaviour
{
    [Header("基础信息")]
    /// <summary>
    /// 实体的身高
    /// </summary>
    public float roleHight;
    /// <summary>
    /// 实体的宽
    /// </summary>
    public float roleWidth;
    /// <summary>
    /// 角色移动速度
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 实体血量
    /// </summary>
    public float hp;
    /// <summary>
    /// 实体的攻击力
    /// </summary>
    public float atk;
    /// <summary>
    /// 实体的防御力
    /// </summary>
    public float def;
    /// <summary>
    /// 用于击退的水平加速度
    /// </summary>
    public float xAcceleration;
    /// <summary>
    /// 水平的击退速度
    /// </summary>
    public float XRepelledSpeed;
    /// <summary>
    /// 竖直的击退速度
    /// </summary>
    public float YRepelledSpeed;
    /// <summary>
    /// 无敌时间
    /// </summary>
    public float invinvibleTime;
    /// <summary>
    /// 重力加速度
    /// </summary>
    public float G = 9.8f;
    [Header("状态标识")]
    /// <summary>
    /// 用于检测是否撞墙
    /// </summary>
    public bool isCollidWall =false;
    /// <summary>
    /// 是否处于空中状态
    /// </summary>
    public bool isOnAir;
    /// <summary>
    /// 是否处于等待状态的标识
    /// </summary>
    public bool isWait;
    /// <summary>
    /// 受伤状态
    /// </summary>
    public bool isWound;
    /// <summary>
    /// 是否击退的标识
    /// </summary>
    public bool isRepelled;
    /// <summary>
    /// 无敌状态标识
    /// </summary>
    public bool isInvincible;
    /// <summary>
    /// 记录能否从平台上下落
    /// </summary>
    protected bool canFallFormNowPlatform;

    [Header("观测数据")]
    /// <summary>
    /// 实际X的移动速度
    /// </summary>
    public float nowXSpeed;
    /// <summary>
    /// 当前的实际Y的速度
    /// </summary>
    public float nowYSpeed;
    /// <summary>
    /// 当前所处的平台高度
    /// </summary>
    public float nowPlatformY;
    /// <summary>
    /// 当前平台的左边界
    /// </summary>
    public float nowPlatformLeft;
    /// <summary>
    /// 当前平台的右边界
    /// </summary>
    public float nowPlatformRight;
    /// <summary>
    /// 实体左侧的判定点
    /// </summary>
    public Vector3 roleLeftDeterminePoint;
    /// <summary>
    /// 实体右侧的判定点
    /// </summary>
    public Vector3 roleRightDeterminePoint;
    /// <summary>
    /// 实体头顶的判定点
    /// </summary>
    public Vector3 roleTopDeterminePoint;
    /// <summary>
    /// 角色当前所处的平台
    /// </summary>
    public Platform nowPlatform;


    [HideInInspector]
    /// <summary>
    /// 获取实体的精灵渲染器
    /// </summary>
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    /// <summary>
    /// 实体的动画控制器
    /// </summary>
    public Animator animator;


     protected virtual void Awake()
    {
        
    }
    /// <summary>
    /// 提供给子类初始化基础信息的函数
    /// </summary>
    public virtual void InitInfo()
    {
        //得到动画控制器的组件
        animator = GetComponent<Animator>();
        //获取精灵渲染器的组件
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        //初始化信息
        InitInfo();
        //监听 平台的逻辑更新
        PlatformLogic.Instance.AddUpdateCheck(this);
        //监听 墙相关的逻辑更新
        WallLogic.Instance.AddUpateCheck(this);
        // 监听 受伤相关逻辑
        EventCenter.Instance.AddEventListener<float>("玩家受伤",Wound);
    }
    protected virtual void OnDisable()
    {
        //移除墙监听
        WallLogic.Instance.RemoveUpdateCheck(this);
        //移除平台监听
        PlatformLogic.Instance.RemoveUpdateCheck(this);
    }
    protected virtual void Update()
    {
        //每帧更新一些基础信息
        UpdataInfo();
        //更新 下落逻辑
        FallLogic();
        //更新 平台相关逻辑
        EventCenter.Instance.EventTrigger<EntityObj>($"{this.gameObject.name}的平台逻辑处理", this);
        //更新 墙相关逻辑
        EventCenter.Instance.EventTrigger<EntityObj>($"{this.gameObject.name}的墙逻辑处理", this);
        

    }
    /// <summary>
    /// 用于每帧更新一些基础信息
    /// </summary>
    protected virtual void UpdataInfo()
    {
        //设置左右判定点
        roleLeftDeterminePoint = this.transform.position - Vector3.right * roleWidth / 2 + Vector3.up * roleHight / 2;
        roleRightDeterminePoint = this.transform.position + Vector3.right * roleWidth / 2 + Vector3.up * roleHight / 2;
        roleTopDeterminePoint = this.transform.position + Vector3.up * roleHight;
    }
    /// <summary>
    /// 下落逻辑处理
    /// </summary>
    protected virtual void FallLogic()
    {
        //更新角色的位置
        this.transform.Translate(Vector3.up * nowYSpeed * Time.deltaTime);
        nowYSpeed -= G * Time.deltaTime;
        //如果 角色的的高度 低于 角色所处平台的高度
        //则将角色置于平台上 
        if (this.transform.position.y < nowPlatformY)
        {
            Vector3 pos = this.transform.position;
            pos.y = nowPlatformY;
            this.transform.position = pos;
            //将空中状态改为false
            isOnAir = false;
            //将下落速度归零
            nowYSpeed = 0;
        }
    }

    /// <summary>
    /// 提供给外部 落地时 改变平台相关信息的方法
    /// </summary>
    /// <param name="y"></param>
    /// <param name="isfall"></param>
    public virtual void SetNowFallPlatformData(Platform nowEntityStayPlatform)
    {
        nowPlatform= nowEntityStayPlatform;
    }
    /// <summary>
    /// 提供给外部改变平台相关信息的方法的重载
    /// </summary>
    /// <param name="Y"></param>
    /// <param name="canfall"></param>
    public virtual void ChangePlatformData(float Y, float left,float right,bool canfall)
    {
        //改变当前所处平台的Y
        this.nowPlatformY = Y;
        //能否从平台上下落
        this.canFallFormNowPlatform = canfall;
        //当前所处平台的左边界
        this.nowPlatformLeft = left;
        //当前所处平台的右边界
        this.nowPlatformRight = right;

    }
    /// <summary>
    /// 下落的方法
    /// </summary>
    public virtual void Fall()
    {
        //将实体对象的 滞空状态设置为 true
        isOnAir = true;
        //改变对象的平台数据为初始值
        ChangePlatformData(-9999, 1, 1,false);
    }
    /// <summary>
    /// 实体的受伤方法
    /// </summary>
    /// <param name="dmg"></param>
    public virtual void Wound(float dmg)
    {
        
    }
    /// <summary>
    /// 死亡的方法
    /// </summary>
    public virtual void Dead()
    {

    }
}
