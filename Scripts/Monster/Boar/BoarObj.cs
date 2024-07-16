using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 野猪的动作类型枚举
/// </summary>
public enum E_Animation_Monster_Type
{
    Idle,
    MoveToPlayer,
    Patrol,
    Wound,
    Dead,
}
/// <summary>
/// 野猪类型枚举
/// </summary>
public enum E_Monster_Boar_Data
{
    Brown,
    Write,
    Black
}


/// <summary>
/// 野猪对象类
/// </summary>

public class BoarObj : MonsterObj
{
    [Header("野猪的额外基础信息")]
    /// <summary>
    /// 野猪数据读取序号
    /// </summary>
    public E_Monster_Boar_Data e_boarDataIndex = E_Monster_Boar_Data.Brown;
    protected override void Start()
    {
        base.Start();
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    public override void InitInfo()
    {
        base.InitInfo();
        T_BoarInfo info = BinaryDataManager.Instance.GetTable<T_BoarInfoContainer>().dataDic[(int)e_boarDataIndex];
        roleWidth = info.f_roleWidth;
        roleHight = info.f_roleHight;
        runSpeed = info.f_runSpeed;
        moveSpeed = info.f_moveSpeed;
        hp = info.f_hp;
        atk = info.f_atk;
        XRepelledSpeed = info.f_XRepelledSpeed;
        YRepelledSpeed = info.f_YRepelledSpeed;
        xAcceleration = info.f_xAcceleration;
        waitTime = info.f_PatrolWait;
    }
    protected override void Update()
    {
        base.Update();
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

    /// <summary>
    /// 用于设置动画切换的函数
    /// </summary>
    /// <param name="type"></param>
    public override void ChangeAnimation(E_Animation_Monster_Type type)
    {
        switch (type)
        {

            case E_Animation_Monster_Type.Idle:
                animator.SetBool("isWalk", false);
                animator.SetBool("isRun", false);
                break;
            case E_Animation_Monster_Type.MoveToPlayer:
                animator.SetBool("isRun", true);
                break;
            case E_Animation_Monster_Type.Patrol:
                animator.SetBool("isWalk", true);
                break;
            case E_Animation_Monster_Type.Dead:
                animator.SetTrigger("WoundTrigger");
                break;
            case E_Animation_Monster_Type.Wound:
                animator.SetTrigger("DeadTrigger");
                break;
        }
    }
    /// <summary>
    /// 巡逻函数
    /// </summary>
    public  void Patrol()
    {
        //等待结束后才继续执行巡逻逻辑
        if (!isWait)
        {
            //切换为巡逻动画
            ChangeAnimation(E_Animation_Monster_Type.Patrol);
            //怪物对象移动
            transform.Translate(nowXSpeed * Time.deltaTime * (spriteRenderer.flipX ? transform.right : -transform.right));
            //如果 判定点离开了 平台 就掉头
            if ((this.roleLeftDeterminePoint.x < nowPlatformLeft || this.roleRightDeterminePoint.x > nowPlatformRight || isCollidWall))
            {
                isWait = true;
                //将实体进行翻转
                spriteRenderer.flipX = !spriteRenderer.flipX;
                //开启协程等待几秒
                MonoManager.Instance.StartCoroutine(WaitForSecond(waitTime));
            }
        }

    }
    /// <summary>
    /// 用于等待的协程函数
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator WaitForSecond(float time)
    {
        //将动画转换为等待
        ChangeAnimation(E_Animation_Monster_Type.Idle);
        //等待几秒
        yield return new WaitForSeconds(time);
        //将等待标识改为false 等待结束
        isWait = false;
        //将怪物的撞墙标记改为false
        isCollidWall = false;
        //翻转速度
        nowXSpeed = moveSpeed;
    }

    /// <summary>
    /// 朝玩家移动的函数
    /// </summary>
    public  void ToMovePlayer()
    {
        //怪物对象移动
        transform.Translate(nowXSpeed * Time.deltaTime * (spriteRenderer.flipX ? transform.right : -transform.right));
        //判断玩家在怪物的左边还是右边
        if (Vector3.Cross(transform.up, player.transform.position - transform.position).z < 0)//右边
        {
            //改变朝向 
            spriteRenderer.flipX = true;
        }
        else if (Vector3.Cross(transform.up, player.transform.position - transform.position).z > 0)//左边
        {
            //改变朝向
            spriteRenderer.flipX = false;
        }

    }
    /// <summary>
    /// 击退的函数
    /// </summary>
    public  void Repelled()
    {
        this.transform.Translate(transform.right * Time.deltaTime * nowXSpeed);
        if (!isplayerRight)
            nowXSpeed -= xAcceleration * Time.deltaTime;
        else
            nowXSpeed += xAcceleration * Time.deltaTime;
    }


}


