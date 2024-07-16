using System.Collections;
using UnityEngine;
/// <summary>
/// 动画状态枚举
/// </summary>
public enum E_Animation_Player_Type
{
    /// <summary>
    /// 攻击
    /// </summary>
    Atk,
    /// <summary>
    /// 下蹲
    /// </summary>
    Crouch,
    /// <summary>
    /// 死亡
    /// </summary>
    Death,
    /// <summary>
    /// 防御
    /// </summary>
    Defend,
    /// <summary>
    /// 下落
    /// </summary>
    Fall,
    /// <summary>
    /// 落地
    /// </summary>
    FallGround,
    /// <summary>
    /// 待机
    /// </summary>
    Idle,
    /// <summary>
    /// 跳跃
    /// </summary>
    Jump,
    /// <summary>
    /// 二段跳
    /// </summary>
    JumpDouble,
    /// <summary>
    /// 跑
    /// </summary>
    Run,
    /// <summary>
    /// 施法
    /// </summary>
    SpellCast,
    /// <summary>
    /// 受伤
    /// </summary>
    Wound
}
/// <summary>
/// 角色控制类
/// </summary>
public class PlayerController : EntityObj
{
    [Header("玩家独有数据")]
    /// <summary>
    /// 角色的跳跃限制
    /// </summary>
    [SerializeField] private int jumpLimit;

    [Header("玩家观测数据")]
    /// <summary>
    /// 攻击次数 用于连招计数
    /// </summary>
    [SerializeField] private int atkIndex;
    /// <summary>
    /// 初始 跳跃 竖直速度
    /// </summary>
    public float initYSpeed;
    /// <summary>
    /// 当前输入方向
    /// </summary>
    [SerializeField] private Vector3 nowDirect;
    /// <summary>
    /// 跳跃次数
    /// </summary>
    [SerializeField] public int jumpIndex;
    /// <summary>
    /// 跳跃的按键是否触发
    /// </summary>
    [SerializeField] private bool isjumpTrigger;
    /// <summary>
    /// 下平台的按键是否触发
    /// </summary>
    [SerializeField] private bool isFallDownFromPlatForm;
    /// <summary>
    ///  用于检测移动输入是否停止 刚刚开始时设置为停止
    /// </summary>
    [SerializeField] private bool isMoveStop=true;
    /// <summary>
    /// 角色能否移动
    /// </summary>
    [SerializeField]
    private bool CanMoving 
    {
        get
        {
            AnimatorStateInfo stateInfo1 = animator.GetCurrentAnimatorStateInfo(1);
            if (stateInfo1.IsName("Atk1")||
                stateInfo1.IsName("Atk2")||
                isWound)
                return false;
            return true;
        }
    }
    /// <summary>
    /// 角色是否能跳跃
    /// </summary>
    [SerializeField]
    private bool CanJump 
    {
        get
        {
            AnimatorStateInfo stateInfo1 = animator.GetCurrentAnimatorStateInfo(1);
            if (stateInfo1.IsName("Atk1") ||
                stateInfo1.IsName("Atk2")||
                stateInfo1.IsName("FallGround"))
                return false;
            return true;
        }
    }   
    protected override void Start()
    {
        base.Start();
        //监听输入相关
        ListenInput();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        //移除监听输入相关
        RemoveListenInput();
    }

    //用于绘制实体的判定
    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //底部的判定线 右下 左下
        Gizmos.DrawLine(this.transform.position + Vector3.right * roleWidth / 2, this.transform.position - Vector3.right * roleWidth / 2);
        //左边的判定线 左下 左上
        Gizmos.DrawLine(this.transform.position - Vector3.right * roleWidth / 2, this.transform.position - Vector3.right * roleWidth / 2 + Vector3.up * roleHight);
        //右边的判定线 右下 右上
        Gizmos.DrawLine(this.transform.position + Vector3.right * roleWidth / 2, this.transform.position + Vector3.right * roleWidth / 2 + Vector3.up * roleHight);
        //上面的判定线 右上 左上
        Gizmos.DrawLine(this.transform.position + Vector3.right * roleWidth / 2 + Vector3.up * roleHight, this.transform.position - Vector3.right * roleWidth / 2 + Vector3.up * roleHight);
        //绘制右判定点
        Gizmos.DrawWireSphere(this.transform.position + Vector3.right * roleWidth / 2 + Vector3.up * roleHight / 2, 0.1f);
        //绘制左判定点
        Gizmos.DrawWireSphere(this.transform.position - Vector3.right * roleWidth / 2 + Vector3.up * roleHight / 2, 0.1f);
    }
    //用于综合Input相关监听函数
    public virtual void ListenInput()
    {
        //启用Input输入
        InputManager.Instance.OnEnableControl();
        //监听移动输入
        EventCenter.Instance.AddEventListener<Vector2>("WASD触发", Move);
        EventCenter.Instance.AddEventListener<bool>("WASD松开",StopMove);
        //监听跳跃输入
        EventCenter.Instance.AddEventListener<bool>("跳跃触发", Jump);
        //监听下平台输入
        EventCenter.Instance.AddEventListener<bool>("下平台触发", FallDownFromPlatform);
        //监听攻击键的输入
        EventCenter.Instance.AddEventListener("攻击键触发",NormalAtk);
    }
    //用于删除Input相关监听函数
    public virtual void RemoveListenInput()
    {
        //关闭Input输入
        InputManager.Instance.OnDisableControl();
        //移除监听移动输入
        EventCenter.Instance.RemoveEventListener<Vector2>("WASD触发", Move);
        EventCenter.Instance.RemoveEventListener<bool>("WASD松开", StopMove);
        //移除监听跳跃输入
        EventCenter.Instance.RemoveEventListener<bool>("跳跃触发", Jump);
        //移除下平台监听
        EventCenter.Instance.RemoveEventListener<bool>("下平台触发", FallDownFromPlatform);
        //移除攻击键监听
        EventCenter.Instance.RemoveEventListener("攻击键触发", NormalAtk);
    }
    public override void InitInfo()
    {
        base.InitInfo();
        //测试代码
        T_playerInfo info = BinaryDataManager.Instance.GetTable<T_playerInfoContainer>().dataDic[0];
        hp = info.f_hp;
        atk = info.f_atk;
        invinvibleTime = info.f_invinvibleTime;
        jumpLimit = info.f_jumpLimit;
    }
    /// <summary>
    /// 用于攻击的函数
    /// </summary>
    /// <param name="index"></param>
    public void NormalAtk()
    {
        //先停止延迟
        CancelInvoke("DelayAtkIndex");
        //获取状态机的状态
        AnimatorStateInfo stateInfo1 = animator.GetCurrentAnimatorStateInfo(1);
        //通过状态机 的名字 对攻击的序列赋值
        if (stateInfo1.IsName("Null"))
            atkIndex = 1;
        else if(stateInfo1.IsName("Atk1"))
            atkIndex = 2;
        else
            atkIndex=0;
        //将动画状态设置为攻击
        ChangeAnimation(E_Animation_Player_Type.Atk);
        //设置延迟函数
        Invoke("DelayAtkIndex", 0.3f);

    }
    /// <summary>
    /// 用于 清除连招计数
    /// </summary>
    private void DelayAtkIndex()
    {
        atkIndex=0;
        //将动画状态设置为攻击
        ChangeAnimation(E_Animation_Player_Type.Atk);
    }
    /// <summary>
    /// 处理移动的函数
    /// </summary>
    /// <param name="direct"></param>
    private void Move(Vector2 direct)
    {
        //如果不能移动就直接返回
        if (!CanMoving)
            return;
        //得到输入的方向
        nowDirect.x = direct.x;
        //判断输入的方向 改变人物的朝向
        if(nowDirect.x!=0)
            spriteRenderer.flipX= nowDirect.x > 0 ? false : true;
        //判断 移动输入是否停止 
        if (isMoveStop)
            nowXSpeed = 0;
        else
            nowXSpeed = moveSpeed;
        //将动画状态改为跑步
        ChangeAnimation(E_Animation_Player_Type.Run);
        this.transform.Translate(nowXSpeed * Time.deltaTime * nowDirect);
    }
    /// <summary>
    /// 用于检测移动输入是否停止的函数
    /// </summary>
    /// <param name="isStop"></param>
    public void StopMove(bool isStop)
    {
        isMoveStop = !isStop;
    }
    /// <summary>
    /// 处理跳跃的函数
    /// </summary>
    private void Jump(bool canJump)
    {
        isjumpTrigger = canJump;
        //跳跃按键按下 跳跃次数少于跳跃限制 并且满足跳跃条件
        if (isjumpTrigger && jumpIndex< jumpLimit && CanJump)
        {
            //赋予初始速度
            nowYSpeed = initYSpeed;
            //复原跳跃触发器
            isjumpTrigger=false;
            //增加已经跳跃次数
            jumpIndex++;
            //将空中状态改为true
            isOnAir = true;
            //将动画状态改为跳跃
            ChangeAnimation(E_Animation_Player_Type.Jump);
        }
    }
    /// <summary>
    /// 处理下平台相关逻辑
    /// </summary>
    /// <param name="canFallDown"></param>
    private void FallDownFromPlatform(bool canFallDown)
    {
        isFallDownFromPlatForm = canFallDown;
        //判断当前平台 是否可以下落
        if(isFallDownFromPlatForm && canFallFormNowPlatform)
        {
            Fall();
        }
    }
    /// <summary>
    /// 用于更新下落的方法
    /// </summary>
    protected override void FallLogic()
    {
        //更新角色的位置
        //当第一帧先减去重力加速度 再位移
        //防止 下落时卡平台
        nowYSpeed -= G * Time.deltaTime;
        this.transform.Translate(Vector3.up * nowYSpeed * Time.deltaTime);
        //将动画状态改为下落
        ChangeAnimation(E_Animation_Player_Type.Fall);
        //如果 角色的的高度 低于 角色所处平台的高度
        //则将角色置于平台上 并重置跳跃次数
        if (this.transform.position.y < nowPlatformY)
        {
            Vector3 pos = this.transform.position;
            pos.y = nowPlatformY;
            this.transform.position = pos;
            //将下落速度归零
            nowYSpeed = 0;
            //重置跳跃次数
            jumpIndex = 0;
            //将空中状态改为false
            isOnAir = false;
            //将动画状态改为待机
            ChangeAnimation(E_Animation_Player_Type.Idle);
        }
    }
    /// <summary>
    /// 下落的方法
    /// </summary>
    public override void Fall()
    {
        base.Fall();
        //如果角色跳出离开平台 就只有一次跳跃机会
        jumpIndex += 1;
    }

    /// <summary>
    /// 用于转变动画状态
    /// </summary>
    /// <param name="animation"></param>
    public void ChangeAnimation(E_Animation_Player_Type animation)
    {
        switch (animation)
        {
            //攻击动画
            case E_Animation_Player_Type.Atk:
                animator.SetInteger("atkIndex", atkIndex);
                break;
            //下蹲动画
            case E_Animation_Player_Type.Crouch:
                break;
            //死亡动画
            case E_Animation_Player_Type.Death:
                animator.SetTrigger("DeadTrigger");
                break;
            //防御动画
            case E_Animation_Player_Type.Defend:
                break;
            //下落动画
            case E_Animation_Player_Type.Fall:
                //设置动画控制器中的Y速度
                animator.SetFloat("nowYSpeed", nowYSpeed);
                if (nowYSpeed < 0)
                    //设置动画状态机的空中条件
                    animator.SetBool("isOnAir", true);
                break;
            //下落落地动画
            case E_Animation_Player_Type.FallGround:
                break;
            //待机动画
            case E_Animation_Player_Type.Idle:
                //设置动画控制器中的是否在空中的变量
                animator.SetBool("isOnAir", isOnAir);
                //设置动画控制器的跳跃次数
                animator.SetInteger("jumpIndex", jumpIndex);
                break;
            //跳跃动画
            case E_Animation_Player_Type.Jump:
                //设置动画控制器中的是否在空中的变量
                animator.SetBool("isOnAir", isOnAir);
                //设置动画状态机的跳跃次数
                animator.SetInteger("jumpIndex", jumpIndex);
                break;
            //二段跳动画
            case E_Animation_Player_Type.JumpDouble:
                break;
            //跑步动画
            case E_Animation_Player_Type.Run:
                //设置动画状态
                animator.SetFloat("nowXSpeed", nowXSpeed);
                break;
            //施法动画
            case E_Animation_Player_Type.SpellCast:
                break;
            //受伤动画
            case E_Animation_Player_Type.Wound:
                animator.SetTrigger("WoundTrigger");
                break;
        }

    }
    /// <summary>
    /// 实体的受伤方法
    /// </summary>
    /// <param name="dmg"></param>
    public override void Wound(float dmg)
    {
        base.Wound(dmg);
        isWound = true;
        if (isInvincible)
            return;
        //将状态改为无敌状态
        isInvincible = true;
        //开启处理无敌的协程函数
        StartCoroutine(InvincibleTime());
        //切换动画
        ChangeAnimation(E_Animation_Player_Type.Wound);
        //扣除hp
        hp -= dmg;
        //hp小于0 就死亡
        if (hp <= 0)
        {
            hp = 0;
            //执行死亡的方法
            Dead();
        }
            
    }
    /// <summary>
    /// 处理无敌时间的协程函数
    /// </summary>
    /// <returns></returns>
    public IEnumerator InvincibleTime()
    {
        yield return new WaitForSeconds(invinvibleTime);
        isInvincible =false;
    }
    /// <summary>
    /// 实体死亡的方法
    /// </summary>
    public override void Dead()
    {
        base.Dead();
        //切换死亡动画
        ChangeAnimation(E_Animation_Player_Type.Death);
        //后续逻辑
    }
}
