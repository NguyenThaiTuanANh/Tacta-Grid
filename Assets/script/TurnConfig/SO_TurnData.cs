using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_TurnData", menuName = "Scriptable Objects/SO_TurnData")]
public class SO_TurnData : ScriptableObject
{
    public List<TurnGroup> turnGroups = new();

    public List<EnemyData> SpawnEnemyListByRate(List<EnemyData> list, int count)
    {
        List<EnemyData> result = new();

        // Tổng %
        float total = 0;
        foreach (var e in list) total += e.spawnPercent;

        for (int i = 0; i < count; i++)
        {
            float rand = UnityEngine.Random.Range(0, total);
            float accum = 0;

            foreach (var e in list)
            {
                accum += e.spawnPercent;
                if (rand <= accum)
                {
                    result.Add(e);
                    break;
                }
            }
        }

        return result;
    }


    private TurnGroup GetGroupByTurn(int currentTurn)
    {
        foreach (var group in turnGroups)
        {
            if (currentTurn >= group.startTurn && currentTurn <= group.endTurn)
                return group;
        }
        return null;
    }

    public List<EnemyData> GetListEnemyPrefabByTurn(int currentTurn, int amount)
    {
        var group = this.GetGroupByTurn(currentTurn);
        if (group == null) return null;

        List<EnemyData> enemies = SpawnEnemyListByRate(group.enemies, amount);
        return enemies;
    }

}

[Serializable]
public class TurnGroup
{
    public int startTurn;
    public int endTurn;
    public List<EnemyData> enemies = new();
}

[Serializable]
public class EnemyData
{
    public GameObject prefab;
    public int spawnPercent;
    public float baseHealth;
    public float baseDamage;
    public float baseSpeed;
}
