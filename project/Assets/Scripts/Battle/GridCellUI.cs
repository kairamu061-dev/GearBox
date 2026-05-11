using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCellUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Image towerIcon;

    static readonly Color EmptyColor   = new(0.2f, 0.16f, 0.1f, 0.8f);
    static readonly Color ValidColor   = new(0.3f, 0.7f, 0.3f, 0.6f);
    static readonly Color InvalidColor = new(0.7f, 0.2f, 0.2f, 0.6f);

    public System.Action OnClicked;
    public System.Action OnHovered;

    public void SetTower(TowerInstance tower)
    {
        if (towerIcon == null) return;
        towerIcon.gameObject.SetActive(tower != null);
        if (tower?.data?.icon != null)
            towerIcon.sprite = tower.data.icon;
    }

    public void SetHighlight(bool? valid)
    {
        if (backgroundImage == null) return;
        backgroundImage.color = valid switch
        {
            true  => ValidColor,
            false => InvalidColor,
            null  => EmptyColor,
        };
    }

    public void OnPointerClick(PointerEventData _) => OnClicked?.Invoke();
    public void OnPointerEnter(PointerEventData _) => OnHovered?.Invoke();
}
