using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Button buyButton;
    [SerializeField] TMP_Text btnLabel;

    public void Setup(string itemName, int price, bool sold, System.Action onBuy)
    {
        if (nameText)  nameText.text  = itemName;
        if (priceText) priceText.text = $"{price} Sc";
        buyButton?.onClick.RemoveAllListeners();

        if (sold)
        {
            buyButton.interactable = false;
            if (btnLabel) btnLabel.text = "売切";
        }
        else
        {
            buyButton.interactable = RunManager.Instance.Scrap >= price;
            if (btnLabel) btnLabel.text = "購入";
            buyButton?.onClick.AddListener(() => onBuy?.Invoke());
        }
    }
}
