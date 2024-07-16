using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class ExcelTool 
{
    /// <summary>
    /// Excel文件存放路径
    /// </summary>
    public static string Excel_Path = Application.dataPath + "/ArtRes/Excel/";
    /// <summary>
    /// 数据结构类存储位置
    /// </summary>
    public static string Data_Class_Path = Application.dataPath + "/Scripts/ExcelData/DataClass/";
    /// <summary>
    /// 数据容器类存储位置
    /// </summary>
    public static string Data_Container_Path = Application.dataPath + "/Scripts/ExcelData/Container/";

    /// <summary>
    /// 真正数据开始的索引
    /// </summary>
    public static int Data_BeginIndex = 4;
    [MenuItem("GameTool/GenerateExcel")]
    private static void GenerateExcelInfo()
    {
        //记录在指定路径中所有的Excel文件 用于生成对应的3个文件
        DirectoryInfo dInfo = Directory.CreateDirectory(Excel_Path);
        //得到所有文件信息 相当于得到所有的Excel表
        //声明Excel的 数据表容器
        DataTableCollection tableConllection;
        FileInfo[] files = dInfo.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            //通过后缀名 剔除除了Excel以外的文件
            if (files[i].Extension != ".xlsx" &&
                files[i].Extension != ".xls")
                continue;
            //打开一个Excel文件 并得到所有的文件数据
            using (FileStream fs = files[i].Open(FileMode.Open,FileAccess.Read))
            {
                IExcelDataReader excelReader = null;
                if (files[i].Extension == ".xls")
                {
                    //使用.xls格式 就用ExcelReaderFactory.CreateBinaryReader(fs)
                    excelReader = ExcelReaderFactory.CreateBinaryReader(fs);
                }
                else if (files[i].Extension != ".xlsx")
                {
                    //使用.xlsx格式 就用ExcelReaderFactory.CreateOpenXmlReader(fs)
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                }
                tableConllection = excelReader?.AsDataSet().Tables;
                fs.Close();
            }

            //遍历文件中的所有表的信息
            foreach (DataTable table in tableConllection)
            {
                //生成数据结构类
                GenarateExcelDataClass(table);
                //生成容器类
                GenarateExcelContainer(table);
                //生成2进制数据
                GenarateExcelBinary(table);
            }
        }
    }
    /// <summary>
    /// 生成Excel数据结构类的方法
    /// </summary>
    /// <param name="table"></param>
    private static void GenarateExcelDataClass(DataTable table)
    {
        //字段名行
        DataRow rowName = GetVariableNameRow(table);
        //字段类型行
        DataRow rowType = GetVariableTypeRow(table);
        //判断路径文件夹是否存在 如果没有 就创建文件夹
        if (!Directory.Exists(Data_Class_Path))
            Directory.CreateDirectory(Data_Class_Path);

        string str = "public class " + table.TableName + "\n{\n";
        //变量进行字符串拼接
        //得到有多少列
        for (int i = 0;i < table.Columns.Count; i++)
        {
            str += "    public " + rowType[i].ToString() + " " + rowName[i].ToString()+";\n";
        }

        str += "}";
        //把拼接好的字符串 存储到 指定的文件中去
        File.WriteAllText(Data_Class_Path + table.TableName+".cs", str);

        //刷新project界面
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 获取 变量名 所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetVariableNameRow(DataTable table)
    {
        return table.Rows[0];
    }
    /// <summary>
    /// 获取 变量名类型 所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetVariableTypeRow(DataTable table)
    {
        return table.Rows[1];
    }
    /// <summary>
    /// 生成数据容器类
    /// </summary>
    /// <param name="table"></param>
    private static  void GenarateExcelContainer(DataTable table)
    {
        //得到主键索引
        int KeyIndex = GetMainKeyIndex(table);
        //得到字段类型行
        DataRow rowType = GetVariableTypeRow(table);
        //没有路径创建路径
        if(!Directory.Exists(Data_Container_Path))
            Directory.CreateDirectory (Data_Container_Path);
        string str = "using System.Collections.Generic;\n";
        str += "public class " + table.TableName.ToString()+ "Container" + "\n{\n";
        str += "public Dictionary<" + rowType[KeyIndex].ToString() + "," + table.TableName.ToString() + ">";
        str += " dataDic = new Dictionary<" + rowType[KeyIndex].ToString() + "," + table.TableName.ToString() + ">();\n";
        str += "}";
        //写入文件
        File.WriteAllText(Data_Container_Path + table.TableName.ToString() + "Container.cs",str);
        //刷新界面
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 获取主键的索引
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static int GetMainKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[2];
        for (int i = 0;i < table.Constraints.Count;i++)
        {
            if (row[i].ToString()=="Key")
                return i;
        }
        return 0;
    }
    /// <summary>
    /// 生成Excel二进制数据
    /// </summary>
    /// <param name="table"></param>
    private static void GenarateExcelBinary(DataTable table)
    {
        //没有路径创建路径
        if(!Directory.Exists(BinaryDataManager.Data_Binary_Path))
            Directory.CreateDirectory(BinaryDataManager.Data_Binary_Path);
        //创建一个二进制文件写入
        using (FileStream fs = new FileStream(BinaryDataManager.Data_Binary_Path + table.TableName.ToString() + ".tang", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //存储Excel具体的二进制的信息
            //1.先要存储我们需要写多少行的数据
            //-Data_BeginIndex的原因 前Data_BeginIndex是配置规则 不是数据信息
            fs.Write(BitConverter.GetBytes(table.Rows.Count- Data_BeginIndex),0,4);

            //2.存储主键的变量名
            string keyName = GetVariableNameRow(table)[GetMainKeyIndex(table)].ToString();
            byte[] bytes =Encoding.UTF8.GetBytes(keyName);
            //2-1.存储主键字符串字节数组的长度
            fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
            //2-2存储主键的字符串
            fs.Write(bytes, 0, bytes.Length);
            
            //3.存储数据
            //3-1.遍历每一行
            DataRow row;//记录行
            DataRow rowType = GetVariableTypeRow(table);//记录列
            for (int i = Data_BeginIndex; i < table.Rows.Count;i++)
            {
                row = table.Rows[i];
                //3-2遍历每一列
                for (int j = 0;j<table.Columns.Count;j++)
                {
                    switch (rowType[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())),0,4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                            break;
                        case "string":
                            bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                            //先写入 字符串字节数组 的长度
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            //再写入 字符串
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }
            }
            fs.Close();
        }
        //刷新页面
        AssetDatabase.Refresh();
    }
}
