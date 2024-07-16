using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo
{

}
public class EventInfo<T> :IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}
public class EventInfo : IEventInfo 
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件中心模块
/// </summary>
public class EventCenter : SingleBaseManger<EventCenter>
{
    //存放 事件的 容器
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加监听 有1个参数的事件
    /// </summary>
    /// <param name="name">添加监听事件名</param>
    /// <param name="action">添加委托</param>
    public void AddEventListener<T>(string name,UnityAction<T> action)
    {
        //事件中心 有无对应事件
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            //添加委托
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    /// <summary>
    /// 添加监听 无参数的事件
    /// </summary>
    /// <param name="name">添加监听事件名</param>
    /// <param name="action">添加委托</param>
    public void AddEventListener(string name, UnityAction action)
    {
        //事件中心 有无对应事件
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            //添加委托
            eventDic.Add(name, new EventInfo(action));
        }
    }
    /// <summary>
    /// 移除 有参数的事件
    /// </summary>
    /// <param name="name">移除事件名</param>
    /// <param name="action">对应添加的委托函数</param>
    public void RemoveEventListener<T>(string name,UnityAction<T> action)
    {
        if(eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo<T>).actions -= action;

    }
    /// <summary>
    /// 移除 无参数的事件事件
    /// </summary>
    /// <param name="name">移除事件名</param>
    /// <param name="action">对应添加的委托函数</param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo).actions -= action;
    }
    /// <summary>
    /// 触发 有参数的事件
    /// </summary>
    /// <param name="name">要触发的事件名</param>
    public void EventTrigger<T>(string name,T info)
    {
        //事件中心 有无对应事件
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo<T>).actions?.Invoke(info);
    }
    /// <summary>
    /// 触发 无参数的事件
    /// </summary>
    /// <param name="name">要触发的事件名</param>
    public void EventTrigger(string name)
    {
        //事件中心 有无对应事件
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo).actions?.Invoke();
    }
    /// <summary>
    /// 清空事件中心
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
