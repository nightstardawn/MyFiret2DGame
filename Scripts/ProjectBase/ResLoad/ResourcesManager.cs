using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// </summary>
public class ResourcesManager : SingleBaseManger<ResourcesManager>
{
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
        T res=Resources.Load<T>(path);
        //对象为GameObject类型 先实例化 再返回
        if (res is GameObject)
            return GameObject.Instantiate(res);
        //如果不是则直接返回
        else
            return res;
    }
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public void LoadAsync<T>(string path,UnityAction<T> callBack) where T : Object
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadAsync<T>(path,callBack));
    }
    /// <summary>
    /// 开启异步加载的协程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadAsync<T>(string path, UnityAction<T> callBack) where T : Object
    {
        ResourceRequest rr = Resources.LoadAsync<T>(path);
        yield return rr;

        if (rr.asset is GameObject)
            callBack(GameObject.Instantiate(rr.asset) as T);
        else
            callBack(rr.asset as T);

    }
}
