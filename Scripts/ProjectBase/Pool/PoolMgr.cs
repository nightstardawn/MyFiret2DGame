using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 缓存池内的 容器类
/// </summary>
public class PoolData
{
    //容器父对象
    public GameObject father;
    //容器数据
    public List<GameObject> poolList;
    /// <summary>
    /// 构造函数 用于 设置对象存放
    /// </summary>
    /// <param name="obj">要存放的对象</param>
    /// <param name="poolObj">要存放在的 缓存池对象</param>
    public PoolData(GameObject obj,GameObject poolObj)
    {
        father = new GameObject(obj.name);
        father.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() {};
        PushObj(obj);
    }
    /// <summary>
    /// 放进缓存池
    /// </summary>
    /// <param name="obj">需要放入的对象</param>
    public void PushObj(GameObject obj)
    {
        poolList.Add(obj);
        //设置父对象
        obj.transform.parent = father.transform;
        //放入缓存池对象要先失活
        obj.SetActive(false);
    }
    /// <summary>
    /// 从容器中取出对象
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj = null;
        //取出第一个 并从列表中 移除
        obj = poolList[0];
        poolList.RemoveAt(0);
        //拿出 缓存池对象要 先激活
        obj.SetActive(true);
        //断开 与缓存池 的关系
        obj.transform.parent = null;
        return obj;
    }
}

/// <summary>
/// 缓存池模块
/// </summary>
public class PoolMgr : SingleBaseManger<PoolMgr>
{
    //用于存储的字典
    public Dictionary<string , PoolData> poolDic =new Dictionary<string, PoolData>();
    //缓存池对象
    private GameObject poolObj;
    /// <summary>
    /// 取出物体
    /// </summary>
    /// <param name="name">需要从缓存池拿取的对象名</param>
    /// <returns></returns>
    public void GetObj(string name,UnityAction<GameObject> callBack)
    {
        //缓存池内 有需要对象
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
            callBack(poolDic[name].GetObj()); 
        //缓存池内 没有需要对象
        else
        {
            //异步加载资源
            ResourcesManager.Instance.LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                callBack(o);
            });
        }
    }
    /// <summary>
    /// 放进缓存池
    /// </summary>
    /// <param name="name">放入缓存池中容器名</param>
    /// <param name="obj">需要放入的对象</param>
    public void PushObj(string name, GameObject obj)
    {
        //判断是否存在缓存池 如果不存在就创建
        if(poolObj==null)
            poolObj = new GameObject("Pool");
        //里面 有抽屉
        if (poolDic.ContainsKey(name))
            poolDic[name].PushObj(obj);
        //里面 没有抽屉
        else
            poolDic.Add(name, new PoolData(obj,poolObj));

    }
    /// <summary>
    /// 用于过场景时 要 清空缓存池子
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
