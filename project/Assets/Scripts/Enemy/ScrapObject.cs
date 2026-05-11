using System.Collections;
using UnityEngine;

public class ScrapObject : MonoBehaviour
{
    const float LifeTime = 60f;
    bool collected;

    void OnEnable()
    {
        collected = false;
        StartCoroutine(AutoExpire());
    }

    IEnumerator AutoExpire()
    {
        yield return new WaitForSeconds(LifeTime);
        if (!collected) gameObject.SetActive(false);
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;
        RunManager.Instance.AddPendingScrap(1);
        gameObject.SetActive(false);
    }
}
