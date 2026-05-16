using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [Header("フィールドサイズ")]
    [SerializeField] float fieldWidth  = 64f;
    [SerializeField] float fieldHeight = 64f;

    [Header("壁プレハブ")]
    [SerializeField] GameObject solidWallPrefab;        // 壊せない壁
    [SerializeField] GameObject destructibleWallPrefab; // 壊せる壁

    [Header("敵プレハブ")]
    [SerializeField] EnemyData[] enemyDataList;

    [Header("砦")]
    [SerializeField] GameObject fortressPrefab;
    [SerializeField] int fortressCount = 2;

    [Header("ボス")]
    [SerializeField] GameObject bossPrefab;

    [Header("ゴール")]
    [SerializeField] GameObject goalPrefab;

    [Header("生成パラメータ")]
    [SerializeField] int solidWallCount       = 20;
    [SerializeField] int destructibleWallCount = 15;
    [SerializeField] int enemyCount           = 10;

    const float MarginFromStart = 8f;  // スタート付近に生成しない範囲
    const float MarginFromGoal  = 5f;  // ゴール付近に生成しない範囲

    Vector2 startPos;
    Vector2 goalPos;

    void Start()
    {
        startPos = Vector2.zero;                             // 戦車スポーン位置
        goalPos  = new Vector2(0, fieldHeight * 0.5f - 3f); // フィールド上方

        bool isBoss = RunManager.Instance.CurrentMapGraph?
            .GetNode(RunManager.Instance.CurrentNodeId)?.type == NodeType.Boss;

        if (isBoss)
            PlaceBoss();
        else
            PlaceGoal();

        PlaceWalls(solidWallPrefab, solidWallCount, hp: -1);
        PlaceWalls(destructibleWallPrefab, destructibleWallCount, hp: 60);
        PlaceEnemies();
        if (!isBoss) PlaceFortresses();
    }

    void PlaceGoal()
    {
        if (goalPrefab == null) return;
        Instantiate(goalPrefab, goalPos, Quaternion.identity);
    }

    void PlaceBoss()
    {
        if (bossPrefab == null) return;
        Instantiate(bossPrefab, goalPos, Quaternion.identity);
    }

    void PlaceWalls(GameObject prefab, int count, float hp)
    {
        if (prefab == null) return;
        int placed = 0, attempts = 0;
        while (placed < count && attempts < count * 10)
        {
            attempts++;
            var pos = RandomFieldPos();
            if (TooClose(pos, startPos, MarginFromStart)) continue;
            if (TooClose(pos, goalPos, MarginFromGoal))   continue;
            var go = Instantiate(prefab, pos, Quaternion.identity);
            if (hp > 0 && go.TryGetComponent<DestructibleWall>(out var dw))
                dw.Initialize(hp);
            placed++;
        }
    }

    void PlaceEnemies()
    {
        if (enemyDataList == null || enemyDataList.Length == 0) return;
        int placed = 0, attempts = 0;
        while (placed < enemyCount && attempts < enemyCount * 10)
        {
            attempts++;
            var pos = RandomFieldPos();
            if (TooClose(pos, startPos, MarginFromStart)) continue;
            var data = enemyDataList[Random.Range(0, enemyDataList.Length)];
            if (data?.prefab == null) continue;
            var go = Instantiate(data.prefab, pos, Quaternion.identity);
            go.GetComponent<EnemyController>()?.Initialize(data);
            placed++;
        }
    }

    void PlaceFortresses()
    {
        if (fortressPrefab == null) return;
        int placed = 0, attempts = 0;
        while (placed < fortressCount && attempts < fortressCount * 10)
        {
            attempts++;
            var pos = RandomFieldPos();
            if (TooClose(pos, startPos, MarginFromStart * 1.5f)) continue;
            if (TooClose(pos, goalPos,  MarginFromGoal * 1.5f))  continue;
            Instantiate(fortressPrefab, pos, Quaternion.identity);
            placed++;
        }
    }

    Vector2 RandomFieldPos() => new(
        Random.Range(-fieldWidth  * 0.5f + 1f, fieldWidth  * 0.5f - 1f),
        Random.Range(-fieldHeight * 0.5f + 1f, fieldHeight * 0.5f - 1f));

    bool TooClose(Vector2 pos, Vector2 target, float dist) =>
        Vector2.Distance(pos, target) < dist;
}
