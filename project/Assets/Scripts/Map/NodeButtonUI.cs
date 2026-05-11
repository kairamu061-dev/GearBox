using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class NodeButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image flagImage;
    [SerializeField] Image tankImage;
    [SerializeField] TMP_Text labelText;
    [SerializeField] Button button;

    [Header("アニメーション")]
    [SerializeField] float pulseMin = 0.9f;
    [SerializeField] float pulseMax = 1.1f;
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float blinkSpeed = 3f;

    public MapNode Node { get; private set; }
    public event Action<MapNode> OnSelected;

    bool isAnimating;
    Coroutine animCoroutine;

    public void Setup(MapNode node)
    {
        Node = node;
        labelText.text = node.type.ToString();
        button.interactable = (node.state == NodeState.Reachable);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnSelected?.Invoke(node));

        ApplyStateVisual();
    }

    void ApplyStateVisual()
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        transform.localScale = Vector3.one;

        flagImage.gameObject.SetActive(Node.state == NodeState.Visited);
        tankImage.gameObject.SetActive(Node.state == NodeState.Current);

        var c = iconImage.color;
        switch (Node.state)
        {
            case NodeState.Unvisited:
                c.a = 0.4f; iconImage.color = c;
                break;
            case NodeState.Reachable:
                c.a = 1f; iconImage.color = c;
                animCoroutine = StartCoroutine(PulseAndBlink());
                break;
            case NodeState.Current:
                c.a = 1f; iconImage.color = c;
                break;
            case NodeState.Visited:
                c.a = 0.6f; iconImage.color = c;
                break;
        }
    }

    IEnumerator PulseAndBlink()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float scale = Mathf.Lerp(pulseMin, pulseMax, t);
            transform.localScale = Vector3.one * scale;

            float blink = Mathf.Sin(Time.time * blinkSpeed) * 0.5f + 0.5f;
            var c = iconImage.color;
            c.a = Mathf.Lerp(0.7f, 1f, blink);
            iconImage.color = c;

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData _)
    {
        if (Node.state != NodeState.Reachable) return;
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        transform.localScale = Vector3.one * pulseMax;
        var c = iconImage.color;
        c.a = 1f;
        iconImage.color = c;
    }

    public void OnPointerExit(PointerEventData _)
    {
        if (Node.state != NodeState.Reachable) return;
        animCoroutine = StartCoroutine(PulseAndBlink());
    }
}
