using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn quái mỗi turn, có hệ thống scaling:
/// - Số lượng quái tăng theo turn
/// - Máu + Damage + Speed tăng theo turn
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs & References")]
    [SerializeField] private SO_TurnData so_TurnData;
    [SerializeField] private PathManager pathManager;
    [SerializeField] private Transform enemyParent;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 0.3f;

    [Header("Enemy Count Scaling")]
    [SerializeField] private int baseEnemyCount = 3;        // số quái turn 1
    [SerializeField] private int enemyIncreasePerTurn = 2;  // mỗi turn tăng thêm bao nhiêu quái

    [Header("Enemy Health Scaling")]
    [SerializeField] private float healthIncreasePerTurn = 15f;

    [Header("Enemy Damage Scaling")]
    [SerializeField] private float damageIncreasePerTurn = 1f;

    [Header("Enemy Speed Scaling")]
    [SerializeField] private float speedIncreasePerTurn = 1.2f;

    private int currentTurn = 1;
    private bool isSpawning = false;


    // ====================================================================== //
    //                          PUBLIC API
    // ====================================================================== //

    /// <summary>
    /// Gọi khi bắt đầu turn mới
    /// </summary>
    public void SpawnEnemiesForTurn(int turnNumber)
    {
        currentTurn = turnNumber;
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    /// <summary>
    /// Kiểm tra xem đang spawn enemy hay không
    /// </summary>
    public bool IsSpawning()
    {
        return isSpawning;
    }


    // ====================================================================== //
    //                          STATS CALCULATOR 
    // ====================================================================== //

    private int GetEnemyCount(int turn)
    {
        return Mathf.Max(1, baseEnemyCount + (turn - 1) * enemyIncreasePerTurn);
    }

    private float GetEnemyHealth(int turn, float baseHealth)
    {
        return baseHealth + (turn - 1) * healthIncreasePerTurn;
    }

    private float GetEnemyDamage(int turn, float baseDamage)
    {
        return baseDamage + (turn - 1) * damageIncreasePerTurn;
    }

    private float GetEnemySpeed(int turn, float baseSpeed)
    {
        return baseSpeed + (turn - 1) * speedIncreasePerTurn;
    }


    // ====================================================================== //
    //                          SPAWN LOGIC
    // ====================================================================== //

    private IEnumerator SpawnEnemiesCoroutine()
    {
        isSpawning = true;

        if (pathManager == null)
        {
            isSpawning = false;
            yield break;
        }

        List<Vector3> waypoints = pathManager.GetWaypoints();
        if (waypoints == null || waypoints.Count == 0)
        {
            isSpawning = false;
            yield break;
        }

        int enemyCount = GetEnemyCount(currentTurn);

        List<EnemyData> enemyPrefabs = so_TurnData.GetListEnemyPrefabByTurn(currentTurn, enemyCount);

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            GameObject enemyObj = Instantiate(enemyPrefabs[i].prefab, enemyParent);

            float HP = GetEnemyHealth(currentTurn, enemyPrefabs[i].baseHealth);
            float DMG = GetEnemyDamage(currentTurn, enemyPrefabs[i].baseDamage);
            float SPD = GetEnemySpeed(currentTurn, enemyPrefabs[i].baseSpeed);

            // Đặt vị trí spawn (waypoint đầu) - 3D
            if (waypoints.Count > 0)
            {
                enemyObj.transform.position = waypoints[0];
            }

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Initialize(waypoints);
                enemy.SetStats(HP, DMG, SPD);
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }
}
