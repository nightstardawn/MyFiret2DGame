using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AI状态基类
/// </summary>
public abstract class AIStateBace
{
    /// <summary>
    /// 记录AI的对象
    /// </summary>
    protected AILogic aiLogic;

    public AIStateBace(AILogic aiLogic)
    {
        this.aiLogic = aiLogic;
    }
    /// <summary>
    /// 处于AI状态 处理逻辑
    /// </summary>
    public  abstract void StayAIState();
    /// <summary>
    /// 每次进入AI状态 处理逻辑
    /// </summary>
    public  abstract void EnterAIState();
    /// <summary>
    /// 每次退出AI状态 处理逻辑
    /// </summary>
    public  abstract void ExitAIState();
}
