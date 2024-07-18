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
/// ���������ݹ�����
/// </summary>
public class BinaryDataManager : SingleBaseManger<BinaryDataManager>
{
    /// <summary>
    /// ���ݶ����ƴ洢λ��
    /// </summary>
    public static string Data_Binary_Path = Application.streamingAssetsPath + "/Binary/";
    //���ڴ洢����Excel�����ݵ�����
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();
    //���ݴ洢λ��
    private static string Save_Path = Application.persistentDataPath + "/Data/";
    /// <summary>
    /// ���ڳ�ʼ������
    /// </summary>
    public void InitData()
    {
        LoadTable<T_playerInfoContainer, T_playerInfo>();
        LoadTable<T_BoarInfoContainer, T_BoarInfo>();
        LoadTable<T_PlayerATKInfoContainer,T_PlayerATKInfo>();
    }

    /// <summary>
    /// ����Excel���2�������ݵ��ڴ���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <typeparam name="K">���ݽṹ������</typeparam>
    public void LoadTable<T,K>()
    {
        //��ȡExcel�� ��Ӧ�Ķ����Ƶ��ļ� �����н���

        using (FileStream fs = File.Open(Data_Binary_Path + typeof(K).Name + ".tang", FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            //��¼��ǰ��ȡ�˶����ֽ�
            int index = 0;
            //�ȶ�ȡ������
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;
            //��ȡ��������
            int keyNameLeght = BitConverter.ToInt32(bytes, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLeght);
            index += keyNameLeght;


            //��ȡÿһ����Ϣ
            //�������������
            Type contaninerType = typeof(T);
            object contaninerObj = Activator.CreateInstance(contaninerType);
            //�õ����ݽṹ���Type
            Type classType = typeof(K);
            //ͨ������ �õ����ݽṹ�� �������ֶε���Ϣ
            FieldInfo[] infos = classType.GetFields();

            for (int i = 0; i < count; i++)
            {
                //ʵ�������ݽṹ�� ����
                object dataObject = Activator.CreateInstance(classType);
                //���������ֶ���Ϣ
                foreach (FieldInfo info in infos)
                {
                    if (info.FieldType == typeof(int))
                    {
                        //�൱���� ��2��������תΪint Ȼ��ֵ���˶�Ӧ���ֶ�
                        info.SetValue(dataObject, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        //�൱���� ��2��������תΪfloat Ȼ��ֵ���˶�Ӧ���ֶ�
                        info.SetValue(dataObject, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if(info.FieldType == typeof(bool))  
                    {
                        //�൱���� ��2��������תΪfloat Ȼ��ֵ���˶�Ӧ���ֶ�
                        info.SetValue(dataObject, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if(info.FieldType == typeof(string))
                    {
                        //��ȡ�ַ����ĳ���
                        int lenght = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        //��ȡ�ַ���
                        info.SetValue(dataObject,Encoding.UTF8.GetString(bytes,index,lenght)); 
                        index += lenght;
                    }
                }

                //�������� Ӧ�ý�������� ��ӵ� �������ֵ�������
                //��Ȳ�ͨ������ķ���
                //dataDic                                                   .Add                                                           (keyName                                        ,dataObject)
                //contaninerType.GetField("dataDic").GetValue(contaninerObj).GetType().GetMethod("Add").Invoke(dicObject,new object[] {classType.GetField(keyName).GetValue(dataObject),dataObject});
                //1.�õ� �������ֵ������� �ֵ����
                object dicObject = contaninerType.GetField("dataDic").GetValue(contaninerObj);
                //2.�õ��ֶζ��� �� Add����
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                //3.�õ����ݽṹ������� ָ���������ֶ�ֵ
                object keyValue = classType.GetField(keyName).GetValue(dataObject);
                //4.�洢����
                mInfo.Invoke(dicObject, new object[] { keyValue, dataObject });
            }

            //����ȡ��ı��¼����
            tableDic.Add(typeof(T).Name,contaninerObj);
            fs.Close();
        }


    }
    /// <summary>
    /// �õ������Ϣ
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if(tableDic.ContainsKey(tableName))
            return tableDic[tableName] as T;
        return null;
    }

    /// <summary>
    /// �洢���������
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
    /// ��ȡ2��������ת������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flieNaem"></param>
    /// <returns></returns>
    public T Load<T>(string fileName) where T:class
    {
        //�����ھͷ���Ĭ��ֵ
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
