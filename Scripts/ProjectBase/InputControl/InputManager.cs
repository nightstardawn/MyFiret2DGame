using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入控制模块
/// </summary>
public class InputManager : SingleBaseManger<InputManager>
{
    //用于检测S键的状态
    private bool sKeyState;

    //InputSystem的配置文件类
    private PlayerInputControl inputControl;
    /// <summary>
    /// 构造函数 调用Mono的Update
    /// </summary>
    public InputManager()
    {
        //创建新的PlayerInputControl对象
        inputControl = new PlayerInputControl();
        //监听点击的按键
        PointInput();
        //监听长按的按键
        MonoManager.Instance.AddUpdateListener(myUpdate);
    }

    private void myUpdate()
    {
        LastInput();
    }
    /// <summary>
    /// 提供个外部启用input的方法
    /// </summary>
    public void OnEnableControl()
    {
        inputControl.Enable();
    }
    /// <summary>
    /// 提供个外部 禁用input的方法
    /// </summary>
    public void OnDisableControl()
    {
        inputControl.Disable();
    }
    /// <summary>
    /// 注册点击事件的方法
    /// </summary>
    private void PointInput()
    {
        //单独按键状态检测
        RecoveryKeys();

        #region 跳跃触发
        inputControl.Gameplay.Jump.started += (context) =>
        {
            if (!sKeyState)
                EventCenter.Instance.EventTrigger<bool>("跳跃触发", true);
        };
        #endregion

        #region 下平台触发
        inputControl.Gameplay.FallDownFromPlatForm.started += (context) =>
        {
            EventCenter.Instance.EventTrigger<bool>("下平台触发", true);
        };
        #endregion


        #region 攻击按键触发
        inputControl.Gameplay.NormalAtk.started += (context) =>
        {
            EventCenter.Instance.EventTrigger("攻击键触发");
        };
        #endregion


    }
    /// <summary>
    /// 注册长按事件的方法
    /// </summary>
    private void LastInput()
    {
        #region 长按移动按键触发
        EventCenter.Instance.EventTrigger<Vector2>("WASD触发", inputControl.Gameplay.Move.ReadValue<Vector2>());
        #endregion

    }
    /// <summary>
    /// 监听 按键状态
    /// </summary>
    private void RecoveryKeys()
    {
        #region S键
        inputControl.Gameplay.SKeyState.started += cxt => sKeyState = true;
        inputControl.Gameplay.SKeyState.canceled += cxt => sKeyState = false;
        #endregion

        #region 移动按键检测
        inputControl.Gameplay.Move.started += (context) =>
        {
            EventCenter.Instance.EventTrigger<bool>("WASD松开", true);
        };
        inputControl.Gameplay.Move.canceled += (context) =>
        {
            EventCenter.Instance.EventTrigger<bool>("WASD松开", false);
        };
        #endregion
    }


}
