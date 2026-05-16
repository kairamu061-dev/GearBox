using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] RectTransform minimapRoot;
    [SerializeField] Image playerDot;
    [SerializeField] Image goalDot;
    [SerializeField] Image goalArrow;

    // ミニマップに映す半径（ワールド単位）
    [SerializeField] float viewRadius = 15f;
    // ゴール方向矢印をミニマップ外周に置くオフセット
    [SerializeField] float arrowRadius = 70f;

    readonly List<(Transform src, Image dot)> enemyDots = new();

    Transform playerTransform;
    Transform goalTransform;

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

        if (playerTransform == null) return;

        // 自機は常にミニマップ中央
        if (playerDot)
            playerDot.rectTransform.anchoredPosition = Vector2.zero;

        // ゴールドット（視野内なら表示、外なら非表示）
        if (goalTransform && goalDot)
        {
            var rel = goalTransform.position - playerTransform.position;
            bool inView = rel.magnitude <= viewRadius;
            goalDot.gameObject.SetActive(inView);
            if (inView)
                goalDot.rectTransform.anchoredPosition = WorldToLocal(goalTransform.position);
        }

        // 敵ドット
        for (int i = enemyDots.Count - 1; i >= 0; i--)
        {
            var (src, dot) = enemyDots[i];
            if (src == null) { Destroy(dot.gameObject); enemyDots.RemoveAt(i); continue; }
            var rel = src.position - playerTransform.position;
            bool inView = rel.magnitude <= viewRadius;
            dot.gameObject.SetActive(inView);
            if (inView)
                dot.rectTransform.anchoredPosition = WorldToLocal(src.position);
        }

        // ゴール方向矢印（常時表示・ミニマップ外周に配置）
        if (goalTransform && goalArrow)
        {
            var dir = (goalTransform.position - playerTransform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            goalArrow.rectTransform.anchoredPosition =
                new Vector2(Mathf.Sin(-angle * Mathf.Deg2Rad),
                            Mathf.Cos(-angle * Mathf.Deg2Rad)) * arrowRadius;
            goalArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            goalArrow.gameObject.SetActive(true);
        }
    }

    // ワールド座標 → ミニマップ上のローカル座標（プレイヤー中心）
    Vector2 WorldToLocal(Vector3 world)
    {
        var mapSize = minimapRoot.rect.size;
        float relX = world.x - playerTransform.position.x;
        float relY = world.y - playerTransform.position.y;
        return new Vector2(
            relX / viewRadius * mapSize.x * 0.5f,
            relY / viewRadius * mapSize.y * 0.5f);
    }
}
