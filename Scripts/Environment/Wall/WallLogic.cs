using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 墙的逻辑处理类
/// </summary>
public class WallLogic : SingleBaseManger<WallLogic>
{
    /// <summary>
    /// 用于存储墙数据
    /// </summary>
    private List<Wall> wallData;
    /// <summary>
    /// 提供给外部添加墙监听的方法
    /// </summary>
    /// <param name="obj"></param>
    public void AddUpateCheck(EntityObj obj)
    {
        //读取所有墙的信息
        wallData = WallDataManager.Instance.Walls;
        EventCenter.Instance.AddEventListener<EntityObj>($"{obj.gameObject.name}的墙逻辑处理", UpdateCheck);
    }
    /// <summary>
    /// 提供给外部移除墙监听的方法
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveUpdateCheck(EntityObj obj)
    {
        EventCenter.Instance.RemoveEventListener<EntityObj>($"{obj.gameObject.name}的墙逻辑处理",UpdateCheck);
    }
    /// <summary>
    /// 用于每帧检测玩家变化的函数
    /// </summary>
    /// <param name="obj"></param>
    private void UpdateCheck(EntityObj obj)
    {
        if (obj == null)
            return;
        for (int i = 0; i < wallData.Count; i++)
        {
            //检测实体是否与墙相撞 并且墙不可穿越
            if ((wallData[i].CheckCollidMe(obj.transform.position+obj.roleWidth/2*Vector3.right) || wallData[i].CheckCollidMe(obj.transform.position - obj.roleWidth / 2 * Vector3.right)) && !wallData[i].canThought)
            {
                
                Vector3 pos = obj.transform.position;
                //判断实体体在墙的左边还是右边
                if (Vector3.Cross(wallData[i].transform.up, obj.transform.position - wallData[i].transform.position).z < 0)//右边
                    pos.x = wallData[i].transform.position.x+obj.roleWidth/2+wallData[i].wallOffset;
                else if(Vector3.Cross(wallData[i].transform.up, obj.transform.position - wallData[i].transform.position).z > 0)//左边
                    pos.x = wallData[i].transform.position.x-obj.roleWidth / 2-wallData[i].wallOffset;
                //将实体的碰撞墙标记更改为true
                obj.isCollidWall = true;
                //将物体的位置设置为与墙边的检测距离
                obj.transform.position = pos;
                //使物体的水平速度为0
                obj.nowXSpeed = 0;
            }
        }

    }
}
