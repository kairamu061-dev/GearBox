using UnityEngine;
using UnityEngine.UI;

public class GameOverSceneController : MonoBehaviour
{
    [SerializeField] Button btnTitle;

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);

        // ラン履歴を保存
        SaveManager.Instance?.AppendHistory(new RunHistoryData
        {
            runId    = System.DateTime.Now.ToString("yyyyMMdd_HHmmss"),
            result   = "GameOver",
            timestamp = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
        });

        btnTitle?.onClick.AddListener(() =>
            SceneTransitionManager.Instance.TransitionTo("TitleScene"));
    }
}
