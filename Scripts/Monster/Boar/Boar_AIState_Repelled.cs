using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Boar击退状态逻辑类
/// </summary>
public class Boar_AIState_Repelled : AIStateBace
{
    public Boar_AIState_Repelled(AILogic AILogic) : base(AILogic)
    {

    }
    public override void EnterAIState()
    {
        //将状态切换成待机
        aiLogic.monsterObj.ChangeAnimation(E_Animation_Monster_Type.Idle);
        //将 XY的速度 切换成击退速度
        if (aiLogic.monsterObj.isplayerRight)//右边
            aiLogic.monsterObj.nowXSpeed = aiLogic.monsterObj.XRepelledSpeed;
        else//左边
            aiLogic.monsterObj.nowXSpeed = -aiLogic.monsterObj.XRepelledSpeed;
        aiLogic.monsterObj.nowYSpeed = aiLogic.monsterObj.YRepelledSpeed;
    }

    public override void ExitAIState()
    {

    }

    public override void StayAIState()
    {
        //执行怪物的击退方法
        (aiLogic.monsterObj as BoarObj).Repelled();
        //检测是否在空中
        aiLogic.monsterObj.isOnAir = !aiLogic.monsterObj.nowPlatform.CheckObjOnMe(aiLogic.monsterObj.transform.position);
        if (!aiLogic.monsterObj.isOnAir)
            aiLogic.ChangeAIState(E_AI_State.ToPlayerMove);

    }
}