using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    /// <summary>
    /// 平台的Y
    /// </summary>
    public float Y=>this.transform.position.y;//利用属性赋值方便之后制作 动态平台
    /// <summary>
    /// 平台的宽
    /// </summary>
    public float width = 5;
    /// <summary>
    /// 平台的左边界
    /// </summary>
    public float left=>this.transform.position.x-width/2;
    /// <summary>
    /// 平台的右边界
    /// </summary>
    public float right=>this.transform.position.x+width/2;
    /// <summary>
    /// 平台是否可以下落
    /// </summary>
    public bool canFall=true;
    public void Start()
    {
        PlatformDataManager.Instance.AddPlatform(this);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position-Vector3.right*width/2, this.transform.position + Vector3.right * width / 2);
    }
    /// <summary>
    /// 提供外部检测玩家是否在我这个平台上
    /// </summary>
    /// <returns></returns>
    public bool CheckObjOnFallMe(Vector3 pos)
    {
        //对象的Y 在我之上 并且在我的边界内
        if(pos.y>= Y && pos.x <=right&&pos.x>=left)
            return true;
        return false;
    }
    /// <summary>
    /// 检测对象是否在平台上
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CheckObjOnMe(Vector3 pos)
    {
        if(pos.y-Y<0.1f && pos.x <= right && pos.x >= left)
            return true;
        return false;
    }
}
