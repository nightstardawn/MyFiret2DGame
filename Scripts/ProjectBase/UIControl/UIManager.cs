using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UI_Layer 
{ 
    bot,
    mid,
    top,
    system
}


/// <summary>
/// UI管理器
/// </summary>
public class UIManager : SingleBaseManger<UIManager>
{
    // 界面上存在的面板 存储容器
    public Dictionary<string, BasePanel> panelDic= new Dictionary<string, BasePanel>();
    //UI面板的层级
    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;
    //记录UICavans父对象 
    public RectTransform canvans;

    /// <summary>
    /// 获取UI层级父对象
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetLayerFather(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.bot:
                return this.bot;
            case E_UI_Layer.mid:
                return this.mid;
            case E_UI_Layer.top:
                return this.top;
            case E_UI_Layer.system:
                return this.system;
        }
        return null;
    }
    public UIManager()
    {
        //去找到Cavens
        GameObject obj = ResourcesManager.Instance.Load<GameObject>("UI/Default/Canvas");
        //过场景不移除
        GameObject.DontDestroyOnLoad(obj);
        canvans = obj.transform as RectTransform;
        //找到各层

        bot = canvans.Find("Bot");
        mid = canvans.Find("Mid");
        top = canvans.Find("Top");
        system = canvans.Find("System");

    }
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板的脚本类型</typeparam>
    /// <param name="panelName">面板的预设体路径UI/</param>
    /// <param name="layer">显示在哪一层</param>
    /// <param name="callBack">面板显示后 要执行的逻辑函数</param>
    public virtual void ShowPanel<T>(string panelName,E_UI_Layer layer, UnityAction<T> callBack) where T : BasePanel
    {
        //如果 已经加载过的面板 则直接调用显示方法 以及回调函数 并直接返回
        if (panelDic.ContainsKey(panelName))
        {
            //显示面板
            panelDic[panelName].ShowMe();
            if (callBack != null)
                callBack(panelDic[panelName] as T);
            return;
        }
        ResourcesManager.Instance.LoadAsync<GameObject>("UI/" + panelName, (obj) =>
        {
            //设置为Cavans 对应层级的 子对象
            Transform father=bot;
            switch (layer)
            {
                case E_UI_Layer.bot:
                    father = bot;
                    break;
                case E_UI_Layer.mid:
                    father = mid;
                    break;
                case E_UI_Layer.top:
                    father = top;
                    break;
                case E_UI_Layer.system:
                    father = system;
                    break;
            }
            //设置父对象
            obj.transform.SetParent(father);
            //初始化面板位置
            obj.transform.localPosition=Vector3.zero;
            obj.transform.localScale=Vector3.one;

            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            //得到 预设体身上的 面板脚本
            T panel=obj.GetComponent<T>();
            //处理面板创建完成后的逻辑
            if(callBack!= null)
                callBack(panel);
            //存储面板
            panelDic.Add(panelName, panel);
        });
    }
    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName">面板的预设体路径UI/</param>
    public virtual void HidePanel(string panelName) 
    {
        if (panelDic.ContainsKey(panelName))
        {
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }
    /// <summary>
    /// 得到一个已经显示的面板
    /// </summary>
    /// <param name="panelName">面板的预设体路径UI/</param>
    public T GetPanel<T>(string panelName) where T:BasePanel
    {
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }
    /// <summary>
    /// 用于自定义监听控件的方法
    /// </summary>
    /// <param name="control">监听控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control,EventTriggerType type,UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger=control.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry =new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);
        trigger.triggers.Add(entry);
    }
}
