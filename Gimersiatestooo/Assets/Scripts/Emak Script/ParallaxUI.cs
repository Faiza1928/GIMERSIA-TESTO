using UnityEngine;

public class ParallaxUI : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Settings")]
    [Tooltip("Seberapa besar efek gerakan kamera pada UI (0.01 = sangat halus).")]
    [Range(0f, 1f)] public float parallaxStrength = 0.03f;

    [Tooltip("Kehalusan transisi.")]
    public float smoothness = 5f;

    private Vector3 initialCamPos;
    private Vector3 initialUIPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        initialCamPos = cameraTransform.position;
        initialUIPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Hitung offset kamera dari posisi awal
        Vector3 camDelta = cameraTransform.position - initialCamPos;

        // Gerakkan UI berlawanan arah dengan pergerakan kamera, biar tetap subtle
        Vector3 targetPos = initialUIPos - camDelta * parallaxStrength;

        // Lerp biar halus
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothness);
    }
}
