using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
/// <summary>
/// 朝玩家移动AI逻辑
/// </summary>
public class Boar_AIState_ToPlayerMove : AIStateBace
{
    public Boar_AIState_ToPlayerMove(AILogic AILogic) : base(AILogic)
    {

    }
    public override void EnterAIState()
    {
        //切换成朝玩家动画
        aiLogic.monsterObj.ChangeAnimation(E_Animation_Monster_Type.MoveToPlayer);
        //将x水平移动速度 变为奔跑速度
        aiLogic.monsterObj.nowXSpeed = aiLogic.monsterObj.runSpeed;
    }
    public override void ExitAIState()
    {

    }
    public override void StayAIState()
    {
        //如果撞墙了就 设置奔跑速度（暂时主要用于击退后卡墙的解决办法）
        if (aiLogic.monsterObj.isCollidWall)
        {
            aiLogic.monsterObj.nowXSpeed = aiLogic.monsterObj.runSpeed;
            aiLogic.monsterObj.isCollidWall = false;
        }
        //执行朝玩家移动的函数
        (aiLogic.monsterObj as BoarObj).ToMovePlayer();
        //怪物和玩家不在同一平台上 就切换成巡逻状态
        if (aiLogic.monsterObj.nowPlatform != aiLogic.monsterObj.player.nowPlatform)
            aiLogic.ChangeAIState(E_AI_State.Patrol);
        //判断是否与玩家相撞
        if (aiLogic.monsterObj.isCollidPlayer)
        {
            //玩家扣血
            EventCenter.Instance.EventTrigger<float>("玩家受伤", aiLogic.monsterObj.atk);
            //将状态改为击退状态
            aiLogic.ChangeAIState(E_AI_State.Repelled);
        }
    }
}
