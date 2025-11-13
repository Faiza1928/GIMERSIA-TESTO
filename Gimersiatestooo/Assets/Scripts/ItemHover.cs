using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI speechText;     // Drag Text_Speech
    public string hoverMessage;            // Teks hover (contoh: "Mengembalikan 20 HP")
    public int price = 5000;               // Harga item
    public int maxPurchase = 2;            // Maksimal pembelian per item

    private Image itemImage;
    private int purchaseCount = 0;
    private Color originalColor;
    private Color highlightColor = new Color(0.8f, 1f, 0.8f); // Hijau muda saat hover
    private Color purchasedColor = new Color(0.7f, 0.7f, 0.7f); // Abu saat purchased

    void Start()
    {
        itemImage = GetComponent<Image>();
        originalColor = itemImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (purchaseCount < maxPurchase)
        {
            itemImage.color = highlightColor;
            speechText.text = $"{hoverMessage}\nHarga: Rp {price:N0}";
        }
        else
        {
            speechText.text = "Item ini sudah mencapai batas pembelian!";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (purchaseCount < maxPurchase)
            itemImage.color = originalColor;

        speechText.text = "Mau beli apa bu? Silahkan";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (purchaseCount >= maxPurchase)
        {
            speechText.text = "Kamu sudah beli item ini maksimal 2x!";
            return;
        }

        // Cek apa si uang cukup
        if (GameManager.Instance != null && GameManager.Instance.SpendMoney(price))
        {
            purchaseCount++;

            // simpen item yang dibeli ke GameManager
            GameManager.Instance.AddPurchasedItem(gameObject.name);

            itemImage.color = Color.Lerp(originalColor, purchasedColor, (float)purchaseCount / maxPurchase);

            if (purchaseCount >= maxPurchase)
            {
                speechText.text = $"Kamu sudah beli {maxPurchase}x. Terima kasih!";
                Transform label = transform.Find("PurchasedLabel");
                if (label != null)
                    label.gameObject.SetActive(true);
            }
            else
            {
                speechText.text = $"Item dibeli ({purchaseCount}/{maxPurchase})!";
            }
        }
        else
        {
            speechText.text = "Uang tidak cukup untuk membeli item ini!";
        }
    }
}
