using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Boar巡逻AI逻辑处理
/// </summary>
public class Boar_AIState_Patrol : AIStateBace
{

    public Boar_AIState_Patrol(AILogic aILogic) : base(aILogic)
    {

    }

    public override void EnterAIState()
    {
        //切换为巡逻动画
        aiLogic.monsterObj.ChangeAnimation(E_Animation_Monster_Type.Patrol);
        //将x水平移动速度 变为移动速度
        aiLogic.monsterObj.nowXSpeed = aiLogic.monsterObj.moveSpeed;
    }

    public override void ExitAIState()
    {

    }

    public override void StayAIState()
    {
        //执行怪物中的巡逻函数
        (aiLogic.monsterObj as BoarObj).Patrol();
        //判断是否和玩家处于同一平台 就切换成朝玩家移动的状态
        if (aiLogic.monsterObj.nowPlatform == aiLogic.monsterObj.player.nowPlatform)
            aiLogic.ChangeAIState(E_AI_State.ToPlayerMove);
    }
}
