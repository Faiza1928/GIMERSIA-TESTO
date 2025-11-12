using UnityEngine;
using UnityEngine.InputSystem;

public class WorldUIParallax : MonoBehaviour
{
    public float parallaxStrength = 0.5f;
    public float smoothness = 8f;
    public Camera mainCamera;

    Vector3 startPos;
    Vector3 targetPos;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        startPos = transform.position;
    }

    void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 offset = (mouseScreen - center) / center;

        Vector3 worldOffset = new Vector3(offset.x * parallaxStrength, offset.y * parallaxStrength, 0);
        targetPos = startPos + worldOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothness);
    }
}
