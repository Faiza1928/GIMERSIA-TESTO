using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<string> purchasedItems = new List<string>() { "Sendok Besi", "Daster Sakti" };
    public string selectedCharacter;
    public bool isSecondCharacterUnlocked = true; // ganti ke false kalau mau test karakter terkunci

    void Awake()
    {
        Instance = this;
    }

    // Dummy unlock biar ga error
    public void UnlockSecondCharacter()
    {
        isSecondCharacterUnlocked = true;
    }
}
