using UnityEngine;

public class AimProvider : MonoBehaviour
{
    public static AimProvider Instance { get; private set; }

    Camera mainCam;

    void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    public Vector2 AimPosition =>
        mainCam != null ? mainCam.ScreenToWorldPoint(Input.mousePosition) : Vector2.zero;
}
