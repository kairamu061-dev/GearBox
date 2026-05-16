using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] RectTransform minimapRoot;
    [SerializeField] Image playerDot;
    [SerializeField] Image goalDot;
    [SerializeField] Image goalArrow;
    [SerializeField] float fieldWidth  = 64f;
    [SerializeField] float fieldHeight = 64f;

    readonly List<(Transform src, Image dot)> enemyDots = new();

    Transform playerTransform;
    Transform goalTransform;

    // ミニマップの外周に表示する方向矢印の半径
    [SerializeField] float arrowRadius = 70f;

    public void RegisterEnemy(Transform enemy)
    {
        if (goalDot == null) return;
        var dot = Instantiate(goalDot, minimapRoot);
        dot.color = new Color(0.9f, 0.2f, 0.2f, 0.8f);
        dot.rectTransform.sizeDelta = new Vector2(6, 6);
        enemyDots.Add((enemy, dot));
    }

    void LateUpdate()
    {
        // プレイヤー・ゴールを未取得なら毎フレーム探す（スポーン後に見つかる）
        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) playerTransform = p.transform;
        }
        if (goalTransform == null)
        {
            var g = FindFirstObjectByType<GoalTrigger>();
            if (g) goalTransform = g.transform;
        }

        if (playerTransform && playerDot)
            playerDot.rectTransform.anchoredPosition = WorldToMinimap(playerTransform.position);

        if (goalTransform && goalDot)
            goalDot.rectTransform.anchoredPosition = WorldToMinimap(goalTransform.position);

        // 敵ドット更新・無効敵を除去
        for (int i = enemyDots.Count - 1; i >= 0; i--)
        {
            var (src, dot) = enemyDots[i];
            if (src == null) { Destroy(dot.gameObject); enemyDots.RemoveAt(i); continue; }
            dot.rectTransform.anchoredPosition = WorldToMinimap(src.position);
        }

        // ゴール方向矢印：ミニマップ外周に配置して向きを示す
        if (playerTransform && goalTransform && goalArrow)
        {
            var dir = (goalTransform.position - playerTransform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            // 矢印をミニマップ中心から arrowRadius の位置に配置
            goalArrow.rectTransform.anchoredPosition =
                new Vector2(Mathf.Sin(-angle * Mathf.Deg2Rad),
                            Mathf.Cos(-angle * Mathf.Deg2Rad)) * arrowRadius;
            goalArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            goalArrow.gameObject.SetActive(true);
        }
        else if (goalArrow)
        {
            goalArrow.gameObject.SetActive(false);
        }
    }

    Vector2 WorldToMinimap(Vector3 world)
    {
        var mapSize = minimapRoot.rect.size;
        float nx = (world.x + fieldWidth  * 0.5f) / fieldWidth;
        float ny = (world.y + fieldHeight * 0.5f) / fieldHeight;
        return new Vector2(nx * mapSize.x - mapSize.x * 0.5f,
                           ny * mapSize.y - mapSize.y * 0.5f);
    }
}
