using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 平台数据管理器 主要用于一次性获取数据 避免之后再次多次单独获取
/// </summary>
public class PlatformDataManager : SingleBaseManger<PlatformDataManager>
{
    //存储平台的容器
    public List<Platform> platforms = new List<Platform>();
    /// <summary>
    /// 添加平台的方法
    /// </summary>
    /// <param name="plat"></param>
    public void AddPlatform(Platform plat)
    {
        platforms.Add(plat);
    }
    /// <summary>
    /// 移除平台的方法
    /// </summary>
    /// <param name="plat"></param>
    public void RemovePlatform(Platform plat)
    {
        platforms.Remove(plat);
    }
    /// <summary>
    /// 清除所有平台的方法
    /// </summary>
    public void Clear()
    {
        platforms.Clear();
    }
}
