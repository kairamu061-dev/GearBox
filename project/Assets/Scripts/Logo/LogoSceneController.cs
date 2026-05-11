using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LogoSceneController : MonoBehaviour
{
    [SerializeField] LogoEntry[] logos;
    [SerializeField] Image logoImage;

    bool skipped;

    void Start()
    {
        if (logos == null || logos.Length == 0)
        {
            GoToTitle();
            return;
        }
        StartCoroutine(PlaySequence());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) skipped = true;
    }

    IEnumerator PlaySequence()
    {
        foreach (var entry in logos)
        {
            if (entry.sprite == null)
            {
                Debug.LogWarning("LogoEntry: sprite is null, skipping.");
                continue;
            }

            logoImage.sprite = entry.sprite;
            SetAlpha(0f);

            yield return Fade(0f, 1f, entry.fadeInDuration);
            if (skipped) { GoToTitle(); yield break; }

            yield return Hold(entry.holdDuration);
            if (skipped) { GoToTitle(); yield break; }

            yield return Fade(1f, 0f, entry.fadeOutDuration);
            if (skipped) { GoToTitle(); yield break; }
        }
        GoToTitle();
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (skipped) yield break;
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetAlpha(to);
    }

    IEnumerator Hold(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (skipped) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void SetAlpha(float alpha)
    {
        var c = logoImage.color;
        c.a = alpha;
        logoImage.color = c;
    }

    void GoToTitle() => SceneTransitionManager.Instance.TransitionImmediate("TitleScene");
}
