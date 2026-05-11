using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TowerCardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image selectionHighlight;

    public TowerInstance Tower { get; private set; }
    public System.Action<TowerInstance> OnSelected;
    public System.Action<TowerInstance> OnHoverEnter;
    public System.Action<TowerInstance> OnHoverExit;

    public void Setup(TowerInstance tower)
    {
        Tower = tower;
        if (nameText) nameText.text = tower.data.displayName;
        if (iconImage && tower.data.icon) iconImage.sprite = tower.data.icon;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight) selectionHighlight.gameObject.SetActive(selected);
    }

    public void OnPointerClick(PointerEventData _) => OnSelected?.Invoke(Tower);
    public void OnPointerEnter(PointerEventData _) => OnHoverEnter?.Invoke(Tower);
    public void OnPointerExit(PointerEventData _) => OnHoverExit?.Invoke(Tower);
}
