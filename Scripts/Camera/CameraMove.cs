using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //跟随目标
    public Transform target;
    //移动速度
    public float moveSpeed=8;
    //目标位置
    private Vector3 targetPos;
    //摄像机偏离目标Y的位置
    private float offsetY=1.5f;
    void Update()
    {

        targetPos = target.position;
        //对目标进行y的偏移
        targetPos.y += offsetY;
        //摄像机不需要更新Z
        targetPos.z =this.transform.localPosition.z;

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, moveSpeed*Time.deltaTime);
    }
}
