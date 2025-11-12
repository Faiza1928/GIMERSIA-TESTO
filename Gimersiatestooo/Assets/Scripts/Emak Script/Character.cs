using UnityEngine;

public class Character : MonoBehaviour
{
    public float damageMultiplier = 1f;
    public bool canUseSkill = true;

    [Header("Character Stats")]
    public string characterName = "Unnamed";
    public int maxHP = 100;
    public int currentHP;

    public bool IsAlive => currentHP > 0;

    void Awake()
    {
        //set HP penuh di awal
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        Debug.Log($"{characterName} took { amount} damage! HP: { currentHP}/{maxHP}");
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        Debug.Log($"{characterName} healed {amount}! HmiPlatform: {currentHP}/{maxHP}");
    }

}
