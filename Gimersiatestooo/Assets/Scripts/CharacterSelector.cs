using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CharacterPanel
{
    public GameObject panel;               // Panel UI untuk karakter ini
    public string characterName;           // Nama karakter (misal "Bu Yati")
    public bool isUnlocked;                // Status apakah karakter terbuka
}

public class CharacterSelector : MonoBehaviour
{
    [Header("Character Panels")]
    public CharacterPanel[] characters;    // Isi dua: Bu Yati & Bu Sri

    [Header("UI Elements Global")]
    public TextMeshProUGUI equippedItemsText;
    public Button leftButton;
    public Button rightButton;
    public Button confirmButton;

    private int currentIndex = 0;

    void Start()
    {
        // Cegah error kalau array kosong
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("Array 'characters' belum diisi di Inspector!");
            return;
        }

        // Update unlock status dari GameManager (kalau sudah ada)
        if (GameManager.Instance != null && characters.Length > 1)
        {
            if (GameManager.Instance.isSecondCharacterUnlocked)
                characters[1].isUnlocked = true;
        }

        // Matikan semua panel dulu
        foreach (var c in characters)
            c.panel.SetActive(false);

        // Tampilkan karakter pertama
        characters[currentIndex].panel.SetActive(true);

        // Update teks item
        UpdateEquippedItemsText();

        // Atur tombol navigasi
        leftButton.onClick.AddListener(PreviousCharacter);
        rightButton.onClick.AddListener(NextCharacter);
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    void PreviousCharacter()
    {
        characters[currentIndex].panel.SetActive(false);
        currentIndex--;
        if (currentIndex < 0) currentIndex = characters.Length - 1;
        characters[currentIndex].panel.SetActive(true);
    }

    void NextCharacter()
    {
        characters[currentIndex].panel.SetActive(false);
        currentIndex++;
        if (currentIndex >= characters.Length) currentIndex = 0;
        characters[currentIndex].panel.SetActive(true);
    }

    void ConfirmSelection()
    {
        var selected = characters[currentIndex];
        if (!selected.isUnlocked)
        {
            Debug.Log("Karakter masih terkunci, belum bisa dipilih!");
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.selectedCharacter = selected.characterName;

        SceneManager.LoadScene("GameScene");
    }

    void UpdateEquippedItemsText()
    {
        if (equippedItemsText == null) return;

        if (GameManager.Instance == null)
        {
            equippedItemsText.text = "Data tidak ditemukan.";
            return;
        }

        var items = GameManager.Instance.purchasedItems;
        if (items != null && items.Count > 0)
        {
            equippedItemsText.text = "Item ter-equip:\n" + string.Join("\n", items);
        }
        else
        {
            equippedItemsText.text = "Belum ada item yang di-equip.";
        }
    }
}
