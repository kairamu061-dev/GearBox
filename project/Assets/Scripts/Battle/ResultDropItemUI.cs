using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultDropItemUI : MonoBehaviour
{
    [SerializeField] TMP_Text itemLabel;
    [SerializeField] Toggle toggle;

    public void Setup(ResultDropItem item, string label, bool autoSelect = false)
    {
        if (itemLabel) itemLabel.text = label;
        if (toggle)
        {
            toggle.isOn = item.selected;
            toggle.interactable = !autoSelect;
            toggle.onValueChanged.AddListener(v => item.selected = v);
        }
    }
}
