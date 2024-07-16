using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleBaseManger<T> where T : new()
{
    private static T instance =new T();
    public static T Instance=>instance;

}
