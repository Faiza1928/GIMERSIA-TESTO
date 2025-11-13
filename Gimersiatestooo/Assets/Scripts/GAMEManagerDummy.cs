using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Item dummy (boleh kosong aja sekarang)
    public List<string> purchasedItems = new List<string>();

    // Data karakter
    public string selectedCharacter;
    public bool isSecondCharacterUnlocked = false; // false dulu biar Bu Sri masih terkunci

    void Awake()
    {
        Instance = this;
    }

    // Dummy unlock biar gak error
    public void UnlockSecondCharacter()
    {
        isSecondCharacterUnlocked = true;
        Debug.Log("Karakter kedua berhasil di-unlock (dummy).");
    }
}
