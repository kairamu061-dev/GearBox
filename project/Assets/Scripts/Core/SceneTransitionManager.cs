using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] float defaultFadeDuration = 0.4f;

    Image fadeImage;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildFadeOverlay();
        SetAlpha(0f);
    }

    // DontDestroyOnLoad で持ち歩くため、Canvas・Image を自前で生成する
    void BuildFadeOverlay()
    {
        var canvasGo = new GameObject("FadeCanvas");
        canvasGo.transform.SetParent(transform);  // 親が DontDestroyOnLoad なので子は自動で永続
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        var imgGo = new GameObject("FadeImage");
        imgGo.transform.SetParent(canvasGo.transform, false);
        fadeImage = imgGo.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;
        var rt = imgGo.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    public void TransitionTo(string sceneName) =>
        StartCoroutine(DoTransition(sceneName, defaultFadeDuration));

    public void TransitionTo(string sceneName, float duration) =>
        StartCoroutine(DoTransition(sceneName, duration));

    public void TransitionImmediate(string sceneName)
    {
        SetAlpha(0f);
        SceneManager.LoadScene(sceneName);
    }

    public void FadeOut(float duration) => StartCoroutine(DoFade(0f, 1f, duration));
    public void FadeIn(float duration) => StartCoroutine(DoFade(1f, 0f, duration));

    IEnumerator DoTransition(string sceneName, float duration)
    {
        yield return DoFade(0f, 1f, duration);
        SceneManager.LoadScene(sceneName);
        yield return DoFade(1f, 0f, duration);
    }

    IEnumerator DoFade(float from, float to, float duration)
    {
        float elapsed = 0f;
        SetAlpha(from);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;
        var c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
