using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CharacterPanel
{
    public GameObject panel;        // panel utama karakter (container)
    public string characterName;
    public bool isUnlocked;

    [Header("UI State Group")]
    public GameObject lockedGroup;  // locked
    public GameObject unlockedGroup; // unlocked 
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
        
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("Array 'characters' belum diisi di Inspector!");
            return;
        }

        // Update unlock status 
        if (GameManager.Instance != null && characters.Length > 1)
        {
            if (GameManager.Instance.isSecondCharacterUnlocked)
                characters[1].isUnlocked = true;
        }

        // Matiin panel
        foreach (var c in characters)
        {
            c.panel.SetActive(false);
            UpdateCharacterLockState(c);
        }

        // Tampilin karakter pertama
        characters[currentIndex].panel.SetActive(true);

        UpdateEquippedItemsText();

        leftButton.onClick.AddListener(PreviousCharacter);
        rightButton.onClick.AddListener(NextCharacter);
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    void UpdateCharacterLockState(CharacterPanel c)
    {
        if (c.lockedGroup != null) c.lockedGroup.SetActive(!c.isUnlocked);
        if (c.unlockedGroup != null) c.unlockedGroup.SetActive(c.isUnlocked);

        // tombol gabisa dipencet kalo locked
        if (c == characters[currentIndex])
        {
            confirmButton.interactable = c.isUnlocked;
        }
    }



    void PreviousCharacter()
    {
        characters[currentIndex].panel.SetActive(false);
        currentIndex--;
        if (currentIndex < 0) currentIndex = characters.Length - 1;
        characters[currentIndex].panel.SetActive(true);

        UpdateCharacterLockState(characters[currentIndex]);
    }

    void NextCharacter()
    {
        characters[currentIndex].panel.SetActive(false);
        currentIndex++;
        if (currentIndex >= characters.Length) currentIndex = 0;
        characters[currentIndex].panel.SetActive(true);

        UpdateCharacterLockState(characters[currentIndex]);
    }

    void ConfirmSelection()
    {
        var selected = characters[currentIndex];

        if (!selected.isUnlocked)
        {
            Debug.Log("Karakter ini masih terkunci dan tidak bisa dipilih!");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.selectedCharacter = selected.characterName;
            Debug.Log("Karakter yang dipilih: " + selected.characterName);
        }

        // Pindah ke scene gelut
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
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
