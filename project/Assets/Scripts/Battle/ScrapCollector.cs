using UnityEngine;

public class ScrapCollector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ScrapObject>(out var scrap))
            scrap.Collect();
    }
}
