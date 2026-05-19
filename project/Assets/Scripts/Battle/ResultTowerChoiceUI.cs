using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultTowerChoiceUI : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text statsText;
    [SerializeField] Image selectionHighlight;
    [SerializeField] Button button;

    public void Setup(TowerData data, System.Action onClick)
    {
        if (nameText) nameText.text = data.displayName;
        if (statsText)
        {
            var sb = new System.Text.StringBuilder();
            if (data.damage > 0)   sb.AppendLine($"DMG {data.damage}");
            if (data.cooldown > 0) sb.AppendLine($"CT  {data.cooldown:F1}s");
            if (data.range > 0)    sb.AppendLine($"射程 {data.range}");
            statsText.text = sb.ToString().TrimEnd();
        }
        SetSelected(false);
        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() => onClick?.Invoke());
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight) selectionHighlight.gameObject.SetActive(selected);
    }
}
