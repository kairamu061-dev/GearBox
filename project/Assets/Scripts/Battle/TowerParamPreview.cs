using UnityEngine;
using UnityEngine.UI;
using TMPro;

// タワーにホバーしたときパラメータを表示するUI
public class TowerParamPreview : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text statsText;

    public static TowerParamPreview Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(TowerInstance tower)
    {
        if (tower?.data == null) return;
        var d = tower.data;
        if (nameText) nameText.text = $"{d.displayName}  Lv{tower.level}";
        if (statsText)
        {
            var sb = new System.Text.StringBuilder();
            if (d.damage > 0)   sb.AppendLine($"DMG: {d.damage}");
            if (d.cooldown > 0) sb.AppendLine($"CT:  {d.cooldown:F1}s");
            if (d.range > 0)    sb.AppendLine($"射程: {d.range}");
            sb.AppendLine($"種別: {d.attackType}");
            statsText.text = sb.ToString().TrimEnd();
        }
        panel?.SetActive(true);
    }

    public void Hide() => panel?.SetActive(false);
}
