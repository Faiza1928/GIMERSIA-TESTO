using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseManager : MonoBehaviour
{
    [Header("UI")]
    public Button backButton;

    [Header("Game Result")]
    public bool isWin = true;       // True = menang
    public int rewardAmount = 1500; // Uang tambahan kalau menang

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager tidak ditemukan di scene ini!");
            return;
        }

        // Tambahkan uang kalau menang
        if (isWin)
        {
            GameManager.Instance.AddMoney(rewardAmount);
            Debug.Log($"Menang! +Rp {rewardAmount:N0} ditambahkan. Total: Rp {GameManager.Instance.playerMoney:N0}");
        }
        else
        {
            Debug.Log("Kalah, tidak ada penambahan uang.");
        }

        // Setup tombol balik
        if (backButton != null)
            backButton.onClick.AddListener(BackToWarung);
        else
            Debug.LogWarning("Tombol Back belum di-assign di Inspector!");
    }

    // Public biar bisa dipanggil dari Button
    public void BackToWarung()
    {
        SceneManager.LoadScene("WarungScene");
    }
}
