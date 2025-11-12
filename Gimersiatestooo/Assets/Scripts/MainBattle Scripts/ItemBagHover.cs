using UnityEngine;
using UnityEngine.EventSystems;

public class ItemBagHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject bagPanel;
    private bool pointerInside = false;

    public void OpenBag()
    {
        bagPanel.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        Debug.Log("Pointer masuk tas");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        Debug.Log("Pointer keluar tas");
        Invoke(nameof(TryClose), 0.2f);
    }

    private void TryClose()
    {
        if (!pointerInside)
            Debug.Log("Tas ditutup otomatis");
            bagPanel.SetActive(false);
    }
}
