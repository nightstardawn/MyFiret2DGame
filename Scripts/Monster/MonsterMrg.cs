using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理每个关卡中的怪物
/// </summary>
public class MonsterMrg : SingleBaseManger<MonsterMrg>
{
    /// <summary>
    /// 用存储关卡中的所有 怪物
    /// </summary>
    public Dictionary<string,MonsterObj> monsterObjs = new Dictionary<string, MonsterObj>();
    /// <summary>
    /// 增加怪物到怪物列表
    /// </summary>
    /// <param name="monsterObj"></param>
    public void AddIntoLevelMonsterList(MonsterObj monsterObj)
    {
        if (monsterObj != null)
            monsterObjs.Add(monsterObj.name,monsterObj);

    }
    /// <summary>
    /// 移除某个怪物
    /// </summary>
    /// <param name="monsterObj"></param>
    public void RemoveFromLevelMonsterList(MonsterObj monsterObj) 
    {
        if(monsterObj != null)
            monsterObjs.Remove(monsterObj.name);
        
    }
    /// <summary>
    /// 用于切换场景时的清除怪物列表
    /// </summary>
    public void ClearLevelMonsterList()
    {
        monsterObjs.Clear();
    }

}
