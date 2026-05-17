using UnityEngine;
using UnityEngine.UI;

// オーバーレイパネルの開閉を管理する汎用コンポーネント
public class OverlayPanel : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button blockerButton; // 背景クリックで閉じる

    void Start()
    {
        closeButton?.onClick.AddListener(() => gameObject.SetActive(false));
        blockerButton?.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Open()  => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);
}
