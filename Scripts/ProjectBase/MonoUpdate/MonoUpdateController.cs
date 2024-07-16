using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mono的Update的控制类
/// 主要用于没有继承Mono的类 可以Update更新
/// 统一对Update进行管理
/// </summary>
public class MonoUpdateController : MonoBehaviour
{
    private event UnityAction updateEvent;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        updateEvent?.Invoke();
    }
    /// <summary>
    /// 添加帧更新事件
    /// </summary>
    /// <param name="action">添加的委托</param>
    public void AddUpdateListener(UnityAction action)
    {
        updateEvent += action;
    }
    /// <summary>
    /// 删除帧更新事件
    /// </summary>
    /// <param name="action">删除的委托</param>
    public void RemoveUpdateListener(UnityAction action)
    {
        updateEvent -= action;
    }
}
