using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager versi gabungan (utama + dummy)
/// Bisa dipakai semua tim:
/// - Kalau scene belum punya MoneyText → sistem uang otomatis nonaktif (dummy mode)
/// - Kalau scene punya MoneyText → sistem uang aktif seperti biasa
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Data")]
    public int playerMoney = 10000;
    public TextMeshProUGUI moneyText; // optional, bisa kosong di scene dummy
    public List<string> purchasedItems = new();
    public string selectedCharacter;
    public bool isSecondCharacterUnlocked = false;

    private bool isDummyMode = false; // otomatis true kalau UI uang tidak ditemukan

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Coba cari moneyText di scene saat ini
        DetectMoneyText();
        if (!isDummyMode) UpdateMoneyDisplay();
    }

    // --- Deteksi TMP text di setiap scene baru ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DetectMoneyText();

        if (!isDummyMode)
        {
            UpdateMoneyDisplay();
            Debug.Log($"[GameManager] UI uang ditemukan di scene {scene.name}, saldo: Rp {playerMoney:N0}");
        }
        else
        {
            Debug.Log($"[GameManager] Dummy mode aktif di scene {scene.name} (tidak ada UI uang)");
        }
    }

    // --- Cek apakah scene punya TextMeshPro untuk uang ---
    private void DetectMoneyText()
    {
        GameObject textObj = GameObject.Find("MoneyText");
        if (textObj == null)
            textObj = GameObject.Find("MoneyLabel"); // alternatif nama

        if (textObj != null)
        {
            moneyText = textObj.GetComponent<TextMeshProUGUI>();
            isDummyMode = false;
        }
        else
        {
            moneyText = null;
            isDummyMode = true;
        }
    }

    // ---------- MONEY ----------
    public void UpdateMoneyDisplay()
    {
        if (isDummyMode || moneyText == null) return;
        moneyText.text = $"Rp {playerMoney:N0},-";
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        if (!isDummyMode) UpdateMoneyDisplay();
        Debug.Log($"+Rp {amount:N0} ditambahkan. Total sekarang: Rp {playerMoney:N0}");
    }

    public bool SpendMoney(int amount)
    {
        if (playerMoney < amount)
        {
            Debug.Log("Uang tidak cukup!");
            return false;
        }

        playerMoney -= amount;
        if (!isDummyMode) UpdateMoneyDisplay();
        return true;
    }

    // ---------- ITEM ----------
    public void AddPurchasedItem(string itemName)
    {
        if (!purchasedItems.Contains(itemName))
        {
            purchasedItems.Add(itemName);
            Debug.Log($"Item {itemName} ditambahkan ke daftar pembelian.");
        }
    }

    // ---------- SCENE ----------
    public void GoToChooseCharacter()
    {
        SceneManager.LoadScene("ChooseCharacter");
    }

    public void UnlockSecondCharacter()
    {
        isSecondCharacterUnlocked = true;
        Debug.Log("Karakter kedua berhasil di-unlock!");
    }
}