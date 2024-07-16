using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class SceneController : SingleBaseManger<SceneController>
{
    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name,UnityAction action)
    {
        SceneManager.LoadScene(name);
        action?.Invoke();
    }

    /// <summary>
    /// 提供给外部 的 异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void LoadSceneAsyn(string name, UnityAction action)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, action));
    }
    /// <summary>
    /// 协程异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsyn(string name,UnityAction action)
    {
        AsyncOperation ao =  SceneManager.LoadSceneAsync(name);
        //得到加载进度
        while (!ao.isDone)
        {
            //在这里更新进度条
            //通过 事件中心 去触发进度条更新事件
            EventCenter.Instance.EventTrigger("进度条更新",ao.progress);
            yield return ao.progress;
        }
        action?.Invoke();
        
    }
    
}
