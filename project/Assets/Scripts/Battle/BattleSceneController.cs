using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSceneController : MonoBehaviour
{
    public static BattleSceneController Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] Slider hpBar;
    [SerializeField] TMP_Text scrapText;
    [SerializeField] GameObject clearText;
    [SerializeField] GameObject gameOverText;

    [Header("カメラ")]
    [SerializeField] CameraFollow cameraFollow;

    bool battleEnded;

    void Awake() => Instance = this;

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);
        RunManager.Instance.OnHpChanged    += UpdateHpBar;
        RunManager.Instance.OnScrapChanged += UpdateScrap;
        UpdateHpBar(RunManager.Instance.CurrentHp, RunManager.Instance.MaxHp);
        UpdateScrap(RunManager.Instance.Scrap);
        clearText?.SetActive(false);
        gameOverText?.SetActive(false);
    }

    void OnDestroy()
    {
        RunManager.Instance.OnHpChanged    -= UpdateHpBar;
        RunManager.Instance.OnScrapChanged -= UpdateScrap;
    }

    void UpdateHpBar(int current, int max)
    {
        if (hpBar) { hpBar.maxValue = max; hpBar.value = current; }
    }

    void UpdateScrap(int amount)
    {
        if (scrapText) scrapText.text = $"{amount} ⚙";
    }

    public void OnGoalReached()
    {
        if (battleEnded) return;
        battleEnded = true;
        StartCoroutine(ClearSequence());
    }

    public void OnGameOver()
    {
        if (battleEnded) return;
        battleEnded = true;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator ClearSequence()
    {
        clearText?.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneTransitionManager.Instance.TransitionTo("ResultScene");
    }

    IEnumerator GameOverSequence()
    {
        gameOverText?.SetActive(true);
        yield return new WaitForSeconds(2f);
        RunManager.Instance.ResetRun();
        SceneTransitionManager.Instance.TransitionTo("GameOverScene");
    }
}
