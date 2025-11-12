using UnityEngine;
using UnityEngine.UI;  // Use TMPro if using TextMeshPro

public class CreditsManager : MonoBehaviour
{
    public float scrollSpeed = 100f; // Adjust the speed of the scroll
    public float stopPositionY = 500f; // Set the Y position at which to stop scrolling

    private RectTransform rectTransform;

    void Start()
    {
        // Get the RectTransform component of the UI element
        rectTransform = GetComponent<RectTransform>();

        // Check if rectTransform is null
        if (rectTransform == null)
        {
            Debug.LogError("No RectTransform found on this GameObject. Please attach it to a UI element.");
            enabled = false; // Disable the script to prevent further errors
        }

    }

    void Update()
    {
        // Check if the current Y position is less than the stop position
        if (rectTransform.anchoredPosition.y < stopPositionY)
        {
            // Move the text upwards over time
            rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }
    }
}