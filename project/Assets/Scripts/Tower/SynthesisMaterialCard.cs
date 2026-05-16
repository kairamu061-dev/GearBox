using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SynthesisMaterialCard : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] Image highlight;
    [SerializeField] Button button;

    public TowerInstance Tower { get; private set; }

    public void Setup(TowerInstance tower, bool selected, System.Action onClick)
    {
        Tower = tower;
        if (label && tower != null)
            label.text = tower.isOnGrid
                ? $"[配置中] {tower.data.displayName} Lv{tower.level}"
                : $"{tower.data.displayName} Lv{tower.level}";
        SetSelected(selected);
        button?.onClick.RemoveAllListeners();
        if (onClick != null) button?.onClick.AddListener(() => onClick());
    }

    public void SetLabel(string text)
    {
        if (label) label.text = text;
        button?.gameObject.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (highlight) highlight.gameObject.SetActive(selected);
    }
}
