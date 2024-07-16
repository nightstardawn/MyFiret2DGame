using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
/// <summary>
/// AI状态 枚举
/// </summary>
public enum E_AI_State
{
    /// <summary>
    /// 空状态
    /// </summary>
    Null,
    /// <summary>
    /// 巡逻状态
    /// </summary>
    Patrol,
    /// <summary>
    /// 朝玩家移动
    /// </summary>
    ToPlayerMove,
    /// <summary>
    /// 攻击状态
    /// </summary>
    Atk,
    /// <summary>
    /// 回到出生点状态
    /// </summary>
    BackSpawn,
    /// <summary>
    /// 击退状态
    /// </summary>
    Repelled,
}

/// <summary>
/// AI逻辑处理类
/// </summary>
public abstract class AILogic
{
    /// <summary>
    /// 怪物对象
    /// </summary>
    public MonsterObj monsterObj;
    /// <summary>
    /// 当前AI状态枚举
    /// </summary>
    public E_AI_State e_nowAIState =E_AI_State.Null;
    /// <summary>
    /// 用于记录AI逻辑中所有的状态
    /// </summary>
    protected Dictionary<E_AI_State, AIStateBace> stateDic = new Dictionary<E_AI_State, AIStateBace>();

    /// <summary>
    /// 当前AI状态
    /// </summary>
    private AIStateBace nowAIState;
    /// <summary>
    /// 初始化 设置AI逻辑处理的对象
    /// </summary>
    /// <param name="monster"></param>
    public AILogic(MonsterObj monster)
    {
        this.monsterObj = monster;
        //添加初始化AI逻辑对象
        AddStateInStateDic();
        //开始 就 更新当前状态的逻辑为巡逻
        ChangeAIState(E_AI_State.Patrol);
    }
    /// <summary>
    /// 给子类重写增加状态的方法
    /// </summary>
    public abstract void AddStateInStateDic();
    /// <summary>
    /// 更新AI的状态
    /// </summary>
    public void UpdateAIState()
    {
        nowAIState.StayAIState();
    }
    /// <summary>
    /// 切换AI的逻辑状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeAIState(E_AI_State state)
    {
        if (e_nowAIState!=E_AI_State.Null)
            //在切换之前 执行上一个状态 的结束方法
            stateDic[e_nowAIState].ExitAIState();
        //切换 当前状态 设置状态
        this.e_nowAIState = state;
        //设置 当前状态 后执行这个状态的进入方法
        stateDic[e_nowAIState].EnterAIState();
        //记录当前状态
        nowAIState = stateDic[e_nowAIState];
    }

}
