using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// </summary>
public class BasePanel : MonoBehaviour
{
    private Dictionary<string,List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();


    protected virtual void Awake()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
    }
    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe()
    {

    }
    /// <summary>
    /// 隐藏自己
    /// </summary>
    public virtual void HideMe()
    {

    }
    #region 子类控件监听需要重写的方法
    /// <summary>
    /// 按钮 需要重写的 监听函数
    /// </summary>
    /// <param name="btnName"></param>
    protected virtual void OnBtnClick(string btnName)
    {

    }
    /// <summary>
    /// Toggle 需要重写的 监听函数
    /// </summary>
    /// <param name="toggleName"></param>
    /// <param name="value"></param>
    protected virtual void OnToggleChanged(string toggleName, bool value)
    {

    }
    /// <summary>
    /// Slider 需要重写的 监听函数
    /// </summary>
    /// <param name="sliderName"></param>
    /// <param name="value"></param>
    protected virtual void OnSliderChanged(string sliderName, float value)
    {

    }
    /// <summary>
    /// InputField 需要重写的 监听函数
    /// </summary>
    /// <param name="sliderName"></param>
    /// <param name="value"></param>
    protected virtual void OnInputFieldChanged(string inputFieldName, string value)
    {

    }
    #endregion

    /// <summary>
    /// 用于 寻找 子对象上的 UI控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        
        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(controls[i].gameObject.name, new List<UIBehaviour>() { controls[i] });
            //如果是按钮
            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() =>
                {
                    OnBtnClick(objName);
                });
            }
            //如果是Toggel
            if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnToggleChanged(objName, value);
                });
            }
            //如果是Slider
            if (controls[i] is Slider)
            {
                (controls[i] as Slider).onValueChanged.AddListener((value) =>
                {
                    OnSliderChanged(objName, value);
                });
            }
            //如果是InputField
            if (controls[i] is InputField)
            {
                (controls[i] as InputField).onValueChanged.AddListener((value) =>
                {
                    OnInputFieldChanged(objName, value);
                });
            }
        }
    }
    /// <summary>
    /// 得到对应名字的控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0;i < controlDic[controlName].Count; i++)
            {
                if (controlDic[controlName][i] is T)
                    return controlDic[controlName][i] as T;
            }
        }
        return null;
    }
    
}
