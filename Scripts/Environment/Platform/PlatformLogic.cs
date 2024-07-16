using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 平台逻辑处理类
/// </summary>
public class PlatformLogic:SingleBaseManger<PlatformLogic>
{
    //游戏对象当前所在平台
    private Platform nowPlatform=null;
    //平台数据
    private List<Platform> platformDate;
    
    /// <summary>
    /// 提供给外部添加逻辑检测的方法
    /// </summary>
    public void AddUpdateCheck(EntityObj obj)
    {
        EventCenter.Instance.AddEventListener<EntityObj>($"{obj.gameObject.name}的平台逻辑处理",UpdateCheck);
    }
    /// <summary>
    /// 提供给外部移除平台监听的方法
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveUpdateCheck(EntityObj obj)
    {
        EventCenter.Instance.RemoveEventListener<EntityObj>($"{obj.gameObject.name}的平台逻辑处理", UpdateCheck);
    }
    /// <summary>
    /// 用于每帧检测玩家变化的函数
    /// </summary>
    public void UpdateCheck(EntityObj obj)
    {
        if (obj == null)
            return;
        //让每一次的遍历的寻找的平台 是这一瞬间最高的平台 而不是整个跳跃过程中的平台
        nowPlatform = null;
        platformDate = PlatformDataManager.Instance.platforms;
        for (int i = 0; i < platformDate.Count; i++)
        {
            //判断玩家是否处于某个平台的条件下 并且处于最高平台或者当前平台为空
            if (platformDate[i].CheckObjOnFallMe(obj.transform.position) &&
                (nowPlatform == null || nowPlatform.Y < platformDate[i].Y ))
            {
                //将所处的平台更新
                nowPlatform = platformDate[i];
                //改变对象的平台数据
                //如果 原始平台 与新平台距离近 就 设置为落地平台
                if ( nowPlatform != null && obj.transform.position.y - nowPlatform.Y < 0.1f)
                    obj.SetNowFallPlatformData(nowPlatform);
                obj.ChangePlatformData(nowPlatform.Y,nowPlatform.left,nowPlatform.right,nowPlatform.canFall);
            }
        }

        //检测当前平台 是否满足下平台的条件
        if(nowPlatform != null&&
            !nowPlatform.CheckObjOnFallMe(obj.transform.position))
        {

            if ((obj as PlayerController))
                (obj as PlayerController).Fall();
        }
    }
}
