using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultSceneController : MonoBehaviour
{
    [SerializeField] TMP_Text baseScrapText;
    [SerializeField] TMP_Text pendingScrapText;
    [SerializeField] Button btnReceive;

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);

        int baseScrap = 50; // TODO: ノード・難易度に応じて変動
        if (baseScrapText)  baseScrapText.text  = $"基本報酬: {baseScrap} Sc";
        if (pendingScrapText) pendingScrapText.text = $"回収スクラップ: {RunManager.Instance.PendingScrap} Sc";

        // 基本報酬は即時反映
        RunManager.Instance.AddScrap(baseScrap);

        btnReceive?.onClick.AddListener(OnReceive);
    }

    void OnReceive()
    {
        RunManager.Instance.CommitPendingScrap();
        // TODO: タワー・強化パーツの選択UI
        SceneTransitionManager.Instance.TransitionTo("MapScene");
    }
}
