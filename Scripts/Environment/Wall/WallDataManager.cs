using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 墙数据管理器 主要用于一次性获取数据 避免之后再次多次单独获取
/// </summary>
public class WallDataManager : SingleBaseManger<WallDataManager>
{
    //存储墙的容器
    public List<Wall> Walls = new List<Wall>();
    /// <summary>
    /// 添加墙的方法
    /// </summary>
    public void AddWall(Wall wall)
    {
        Walls.Add(wall);
    }
    /// <summary>
    /// 删除墙的方法
    /// </summary>
    public void RemoveWall(Wall wall)
    {
        Walls.Remove(wall);
    }
    /// <summary>
    /// 清除墙的方法
    /// </summary>
    public void Clear()
    {
        Walls.Clear();
    }
}
