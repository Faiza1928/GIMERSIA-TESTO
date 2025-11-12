using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollowOffset : MonoBehaviour
{
    [Header("Camera Movement")]
    public float cameraMovementStrength = 2f;
    public float cameraSmoothness = 5f;
    public Vector3 cameraOffset = new Vector3(-1f, 0, 0); // posisi awal agak ke kiri
    public Vector2 cameraLimit = new Vector2(2f, 1f); // batas gerak kanan/kiri & atas/bawah

    private Vector3 cameraStartPos;

    [Header("UI Movement (optional)")]
    public RectTransform uiRoot;
    public float uiMovesStrength = 20f;
    public float uiSmoothness = 7f;
    private Vector3 uiStartPos;

    void Start()
    {
        cameraStartPos = transform.position + cameraOffset;

        if (uiRoot != null)
            uiStartPos = uiRoot.anchoredPosition;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float normalizedX = (mousePos.x / Screen.width - 0.5f) * 2f;
        float normalizedY = (mousePos.y / Screen.height - 0.5f) * 2f;

        Vector3 offset = new Vector3(normalizedX * cameraMovementStrength, normalizedY * cameraMovementStrength, 0);

        // Batasi agar kamera tidak bisa ke kiri, tapi bisa ke kanan dan atas-bawah sedikit
        offset.x = Mathf.Clamp(offset.x, 0, cameraLimit.x);
        offset.y = Mathf.Clamp(offset.y, -cameraLimit.y, cameraLimit.y);



        Vector3 targetCamPos = cameraStartPos + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime * cameraSmoothness);

        if (uiRoot != null && uiRoot.gameObject.activeInHierarchy)
        {
            Vector3 targetUIPos = uiStartPos + new Vector3(normalizedX * uiMovesStrength, normalizedY * uiMovesStrength, 0);
            uiRoot.anchoredPosition = Vector3.Lerp(uiRoot.anchoredPosition, targetUIPos, Time.deltaTime * uiSmoothness);
        }

    }
}
