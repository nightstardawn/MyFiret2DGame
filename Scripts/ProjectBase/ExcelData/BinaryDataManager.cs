using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
/// <summary>
/// 二进制数据管理类
/// </summary>
public class BinaryDataManager : SingleBaseManger<BinaryDataManager>
{
    /// <summary>
    /// 数据二进制存储位置
    /// </summary>
    public static string Data_Binary_Path = Application.streamingAssetsPath + "/Binary/";
    //用于存储所有Excel表数据的容器
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();
    //数据存储位置
    private static string Save_Path = Application.persistentDataPath + "/Data/";
    /// <summary>
    /// 用于初始化数据
    /// </summary>
    public void InitData()
    {
        LoadTable<T_playerInfoContainer, T_playerInfo>();
        LoadTable<T_BoarInfoContainer, T_BoarInfo>();
    }

    /// <summary>
    /// 加载Excel表的2进制数据到内存中
    /// </summary>
    /// <typeparam name="T">容器类名</typeparam>
    /// <typeparam name="K">数据结构类类名</typeparam>
    public void LoadTable<T,K>()
    {
        //读取Excel表 对应的二进制的文件 来进行解析

        using (FileStream fs = File.Open(Data_Binary_Path + typeof(K).Name + ".tang", FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            //记录当前读取了多少字节
            int index = 0;
            //先读取多少行
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;
            //读取主键名字
            int keyNameLeght = BitConverter.ToInt32(bytes, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLeght);
            index += keyNameLeght;


            //读取每一行信息
            //创建容器类对象
            Type contaninerType = typeof(T);
            object contaninerObj = Activator.CreateInstance(contaninerType);
            //得到数据结构类的Type
            Type classType = typeof(K);
            //通过反射 得到数据结构类 的所有字段的信息
            FieldInfo[] infos = classType.GetFields();

            for (int i = 0; i < count; i++)
            {
                //实例化数据结构类 对象
                object dataObject = Activator.CreateInstance(classType);
                //遍历所有字段信息
                foreach (FieldInfo info in infos)
                {
                    if (info.FieldType == typeof(int))
                    {
                        //相当就是 把2进制数据转为int 然后赋值给了对应的字段
                        info.SetValue(dataObject, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        //相当就是 把2进制数据转为float 然后赋值给了对应的字段
                        info.SetValue(dataObject, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if(info.FieldType == typeof(bool))  
                    {
                        //相当就是 把2进制数据转为float 然后赋值给了对应的字段
                        info.SetValue(dataObject, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if(info.FieldType == typeof(string))
                    {
                        //读取字符串的长度
                        int lenght = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        //读取字符串
                        info.SetValue(dataObject, BitConverter.ToString(bytes, lenght));
                        index += lenght;
                    }
                }

                //读完数据 应该将这个数据 添加到 容器的字典容器中
                //类比不通过反射的方法
                //dataDic                                                   .Add                                                           (keyName                                        ,dataObject)
                //contaninerType.GetField("dataDic").GetValue(contaninerObj).GetType().GetMethod("Add").Invoke(dicObject,new object[] {classType.GetField(keyName).GetValue(dataObject),dataObject});
                //1.得到 容器的字典容器的 字典对象
                object dicObject = contaninerType.GetField("dataDic").GetValue(contaninerObj);
                //2.得到字段对象 的 Add方法
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                //3.得到数据结构类对象中 指定主键的字段值
                object keyValue = classType.GetField(keyName).GetValue(dataObject);
                //4.存储数据
                mInfo.Invoke(dicObject, new object[] { keyValue, dataObject });
            }

            //将读取完的表记录下来
            tableDic.Add(typeof(T).Name,contaninerObj);
            fs.Close();
        }


    }
    /// <summary>
    /// 得到表的信息
    /// </summary>
    /// <typeparam name="T">容器类名</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if(tableDic.ContainsKey(tableName))
            return tableDic[tableName] as T;
        return null;
    }

    /// <summary>
    /// 存储类对象数据
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public void Save(object obj,string fileName)
    {
        using (FileStream fs = new FileStream(Save_Path+fileName+".tang",FileMode.OpenOrCreate,FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs,obj);
            fs.Close();
        }
    }
    /// <summary>
    /// 读取2进制数据转换对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flieNaem"></param>
    /// <returns></returns>
    public T Load<T>(string fileName) where T:class
    {
        //不存在就返回默认值
        if(File.Exists(Save_Path + fileName + ".tang"))
            return default(T);

        T obj;
        using (FileStream fs = File.Open(Save_Path + fileName + ".tang", FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fs) as T;
            fs.Close() ;
        }
        
        return obj;
    }

}
