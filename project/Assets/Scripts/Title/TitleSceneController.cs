using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    [Header("ボタン（データなし）")]
    [SerializeField] Button btnStart;

    [Header("ボタン（データあり）")]
    [SerializeField] Button btnContinue;
    [SerializeField] Button btnNewGame;

    [Header("共通")]
    [SerializeField] Button btnPastRuns;
    [SerializeField] Button btnSettings;
    [SerializeField] Button btnQuit;

    [Header("ダイアログ")]
    [SerializeField] GameObject confirmNewGameDialog;
    [SerializeField] Button btnConfirmYes;
    [SerializeField] Button btnConfirmNo;

    [Header("パネル")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject pastRunsPanel;

    [Header("初期タワー")]
    [SerializeField] TowerData startTowerData;

    void Start()
    {
        SceneTransitionManager.Instance.FadeIn(0.4f);
        RefreshButtons();
        BindButtons();
    }

    void RefreshButtons()
    {
        bool hasSave = SaveManager.Instance.HasSave();
        btnStart?.gameObject.SetActive(!hasSave);
        btnContinue?.gameObject.SetActive(hasSave);
        btnNewGame?.gameObject.SetActive(hasSave);
        confirmNewGameDialog?.SetActive(false);
        settingsPanel?.SetActive(false);
        pastRunsPanel?.SetActive(false);
    }

    void BindButtons()
    {
        btnStart?.onClick.AddListener(OnStart);
        btnContinue?.onClick.AddListener(OnContinue);
        btnNewGame?.onClick.AddListener(OnNewGame);
        btnConfirmYes?.onClick.AddListener(OnConfirmNewGame);
        btnConfirmNo?.onClick.AddListener(() => confirmNewGameDialog?.SetActive(false));
        btnSettings?.onClick.AddListener(() => settingsPanel?.SetActive(true));
        btnPastRuns?.onClick.AddListener(() => pastRunsPanel?.SetActive(true));
        btnQuit?.onClick.AddListener(OnQuit);
    }

    void OnStart()
    {
        RunManager.Instance.InitNewRun(startTowerData);
        SceneTransitionManager.Instance.TransitionTo("MapScene");
    }

    void OnContinue()
    {
        var save = SaveManager.Instance.LoadRun();
        if (save == null) { OnStart(); return; }
        if (!SaveManager.Instance.RestoreRun(save))
        {
            Debug.LogError("セーブデータの復元に失敗しました。新規ランを開始します。");
            OnStart();
            return;
        }
        SceneTransitionManager.Instance.TransitionTo(save.resumeSceneName ?? "MapScene");
    }

    void OnNewGame() => confirmNewGameDialog?.SetActive(true);

    void OnConfirmNewGame()
    {
        SaveManager.Instance.DeleteSave();
        confirmNewGameDialog?.SetActive(false);
        RunManager.Instance.InitNewRun(startTowerData);
        SceneTransitionManager.Instance.TransitionTo("MapScene");
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
