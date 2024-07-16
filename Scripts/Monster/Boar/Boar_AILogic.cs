using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 野猪 的 AI逻辑类
/// </summary>
public class Boar_AILogic : AILogic
{
    public Boar_AILogic(MonsterObj monster) : base(monster)
    {

    }

    public override void AddStateInStateDic()
    {
        stateDic.Add(E_AI_State.Patrol, new Boar_AIState_Patrol(this));
        stateDic.Add(E_AI_State.ToPlayerMove, new Boar_AIState_ToPlayerMove(this));
        stateDic.Add(E_AI_State.Repelled, new Boar_AIState_Repelled(this));
    }
}
