using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Mono的Update的管理类
/// 1.统一对Update进行管理
/// 2.添加协程
/// </summary>
public class MonoManager : SingleBaseManger<MonoManager>
{
    public MonoUpdateController controller;

    public MonoManager()
    {
        GameObject obj = new GameObject("MonoUpdateController");
        controller = obj.AddComponent<MonoUpdateController>();
    }

    /// <summary>
    /// 添加帧更新事件
    /// </summary>
    /// <param name="action">添加的委托</param>
    public void AddUpdateListener(UnityAction action)
    {
        controller.AddUpdateListener(action);
    }
    /// <summary>
    /// 删除帧更新事件
    /// </summary>
    /// <param name="action">删除的委托</param>
    public void RemoveUpdateListener(UnityAction action)
    {
        controller.RemoveUpdateListener(action);
    }

    #region 给外部开启协程的函数
    /// <summary>
    /// 开启协程的函数
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }
    /// <summary>
    /// 开启协程的函数
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName, value);
    }
    /// <summary>
    /// 开启协程的函数
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }
    #endregion

    #region 关闭协程的函数
    /// <summary>
    /// 关闭协程的函数
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public void StopCoroutine(Coroutine routine)
    {
        controller.StopCoroutine(routine);
    }
    /// <summary>
    /// 关闭协程的函数
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    }
    #endregion

}
