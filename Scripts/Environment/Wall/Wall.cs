using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 墙对象类
/// </summary>
public class Wall : MonoBehaviour
{
    /// <summary>
    /// 墙的X
    /// </summary>
    public float X=>this.transform.position.x;
    /// <summary>
    /// 墙的高
    /// </summary>
    public float hight = 5;

    /// <summary>
    /// 墙的顶部
    /// </summary>
    public float top => this.transform.position.y+hight/2;
    /// <summary>
    /// 墙的底部
    /// </summary>
    public float bot=> this.transform.position.y-hight/2;
    /// <summary>
    /// 能否穿越
    /// </summary>
    public bool canThought;
    /// <summary>
    /// 墙实际检测的偏移位置
    /// </summary>
    public float wallOffset;
    private void Start()
    {
        WallDataManager.Instance.AddWall(this);
        wallOffset = 0.05f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position-Vector3.up*hight/2,this.transform.position+Vector3.up*hight/2);
        Gizmos.color= Color.green;
        Gizmos.DrawLine(this.transform.position - Vector3.up * hight / 2 + Vector3.right * wallOffset, this.transform.position + Vector3.up * hight / 2 + Vector3.right * wallOffset);
        Gizmos.DrawLine(this.transform.position - Vector3.up * hight / 2 - Vector3.right * wallOffset, this.transform.position + Vector3.up * hight / 2 - Vector3.right * wallOffset);

    }
    /// <summary>
    /// 检测是否与墙相撞
    /// </summary>
    /// <param name="pos"></param>
    public bool CheckCollidMe(Vector3 pos)
    {
        //检测是是否在墙的范围内
        if (pos.y < top && pos.y > bot && Mathf.Abs(pos.x - this.transform.position.x) < wallOffset)
            return true;
        return false;
    }
}
