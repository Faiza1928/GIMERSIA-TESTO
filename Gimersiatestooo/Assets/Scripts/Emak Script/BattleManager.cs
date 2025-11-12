using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{

    [Header("Extra Prefabs")]
    public GameObject tanteWajanPrefab;

    [Header("Player Sprites")]
    public Sprite emakIdle;
    public Sprite emakAttack;

    [Header("Enemy Sprites")]
    public Sprite tetanggaIdle;
    public Sprite tetanggaAttack;

    //[Header("Enemy Pose Settings")]
    //public Vector3 enemyAttackScale = new Vector3(1.2f, 1.2f, 1f);
    //public Vector3 enemyAttackOffset = new Vector3(-0.3f, 0f, 0f);



    [Header("Background & UI")]
    [SerializeField] GameObject bgPlayerTurn;
    [SerializeField] GameObject bgEnemyTurn;
    [SerializeField] CanvasGroup parallaxUI; // Canvas_ParallaxUI

    [Header("Prefab References")]
    public Character playerPrefab;
    public Character enemyPrefab;

    [Header("Spawn Points")]
    public Transform playerSpawn;
    public Transform enemySpawn;

    [Header("HP Stuff")]
    public Slider playerHPBar;
    public TextMeshProUGUI playerHPText;
    public Slider enemyHPBar;
    public TextMeshProUGUI enemyHPText;

    private SpriteRenderer playerSprite;
    private SpriteRenderer enemySprite;

    private GameObject tanteWajanInstance;


    private float playerHPVisual;
    private float enemyHPVisual;

    public float hpLerpSpeed = 5f; // makin tinggi, makin cepat animasinya

    [Header("Chakra")]
    public ChakraManager chakraManager;

    private Character player;
    private Character enemy;
    private bool playerTurn = true;

    void Start()
    {
        // Spawn karakter
        player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        enemy = Instantiate(enemyPrefab, enemySpawn.position, Quaternion.identity);

        playerSprite = player.GetComponent<SpriteRenderer>(); // ambil sprite renderer Emak
        enemySprite = enemy.GetComponent<SpriteRenderer>();

        if (tanteWajanPrefab != null)
        {
            tanteWajanInstance = Instantiate(tanteWajanPrefab);
            tanteWajanInstance.SetActive(true);
        }


        playerHPVisual = player.currentHP;
        enemyHPVisual = enemy.currentHP;

        Debug.Log($"Battle Start! {player.characterName} vs {enemy.characterName}");
        Debug.Log($"{player.characterName}'s Turn!");

        UpdateUIInstant();

        // Awal game = player turn view
        SetBattleView(true);
    }

    void Update()
    {
        // Smooth HP Bar update
        if (player != null)
        {
            playerHPVisual = Mathf.Lerp(playerHPVisual, player.currentHP, Time.deltaTime * hpLerpSpeed);
            playerHPBar.value = playerHPVisual;
        }
        if (enemy != null)
        {
            enemyHPVisual = Mathf.Lerp(enemyHPVisual, enemy.currentHP, Time.deltaTime * hpLerpSpeed);
            enemyHPBar.value = enemyHPVisual;
        }

        // Text update (real-time)
        if (player != null && enemy != null)
        {
            playerHPText.text = $"{player.characterName}: {player.currentHP} / {player.maxHP}";
            enemyHPText.text = $"{enemy.characterName}: {enemy.currentHP} / {enemy.maxHP}";
        }
    }

    // === PLAYER TURN ===
    public void OnSkill1Pressed()
    {
        if (!playerTurn)
        {
            Debug.Log("Bukan giliran player!");
            return;
        }

        SetBattleView(true); // pastikan player view aktif\
        StartCoroutine(PerformPlayerAttack());
        chakraManager.GainChakra();

    }


    // === ENEMY TURN ===
    void EnemyTurn()
    {
        SetBattleView(false); // ganti ke enemy view

        if (enemy.IsAlive)
            StartCoroutine(PerformEnemyAttack());

        // Balik lagi ke player turn setelah delay
        //Invoke(nameof(ReturnToPlayerTurn), 1.5f);
    }

    // === KEMBALI KE PLAYER TURN ===
    void ReturnToPlayerTurn()
    {
        playerTurn = true;
        SetBattleView(true);
    }

    // === GANTI VIEW DAN UI ===
    void SetBattleView(bool playerTurn)
    {
        // Ganti background
        bgPlayerTurn.SetActive(playerTurn);
        bgEnemyTurn.SetActive(!playerTurn);

        // Hide or show player/enemy

        if (player != null) player.gameObject.SetActive(playerTurn);
        if (enemy != null) enemy.gameObject.SetActive(!playerTurn);

        // Tante Wajan ikut player turn
        if (tanteWajanInstance != null)
            tanteWajanInstance.SetActive(playerTurn);

        // Hide or show Canvas_ParallaxUI (Skill, Item, dsb)
        if (parallaxUI != null)
        {
            parallaxUI.alpha = playerTurn ? 1f : 0f;
            parallaxUI.interactable = playerTurn;
            parallaxUI.blocksRaycasts = playerTurn;
        }
    }

    // === LOGIKA SERANG ===
    void Attack(Character attacker, Character target)
    {
        int damage = Random.Range(10, 20);
        target.TakeDamage(damage);
        Debug.Log($"{attacker.characterName} attacks {target.characterName} for {damage} damage!");

        // Update bar batas max HP
        playerHPBar.maxValue = player.maxHP;
        enemyHPBar.maxValue = enemy.maxHP;

        if (!target.IsAlive)
        {
            Debug.Log($"{target.characterName} has been defeated!");
        }
    }

    // === Ganti Sprite pas Menyerang ===
    private IEnumerator PerformPlayerAttack()
    {
        //ganti pose attack
        if (playerSprite != null && emakAttack != null)
            playerSprite.sprite = emakAttack;

        yield return new WaitForSeconds(0.6f);

        Attack(player, enemy);

        //balik ke idle
        if (playerSprite != null && emakIdle != null)
            playerSprite.sprite = emakIdle;

        playerTurn = false;

        //delay sebelum enemy turn
        Invoke(nameof(EnemyTurn), 1.0f);

    }

    private IEnumerator PerformEnemyAttack()
    {
        if (enemySprite != null && tetanggaAttack != null)
        {
            enemySprite.sprite = tetanggaAttack;
        }

        yield return new WaitForSeconds(0.6f);

        Attack(enemy, player);

        if (enemySprite != null && tetanggaIdle != null)
        {
            enemySprite.sprite = tetanggaIdle;
        }

        Invoke(nameof(ReturnToPlayerTurn), 1.0f);
    }


    // === UPDATE UI LANGSUNG ===
    void UpdateUIInstant()
    {
        playerHPBar.maxValue = player.maxHP;
        enemyHPBar.maxValue = enemy.maxHP;

        playerHPBar.value = player.currentHP;
        enemyHPBar.value = enemy.currentHP;

        playerHPText.text = $"{player.characterName}: {player.currentHP} / {player.maxHP}";
        enemyHPText.text = $"{enemy.characterName}: {enemy.currentHP} / {enemy.maxHP}";

        playerHPVisual = player.currentHP;
        enemyHPVisual = enemy.currentHP;
    }

        // === SKILL 2: Heal + Attack ===
    public void UseSkill2()
    {
        if (!playerTurn)
        {
            Debug.Log("Bukan giliran player!");
            return;
        }

        if (!chakraManager.HasEnoughChakra(1))
        {
            Debug.Log("Chakra tidak cukup untuk Skill 2!");
            return;
        }

        StartCoroutine(PerformSkill2());
    }

    private IEnumerator PerformSkill2()
    {
        chakraManager.UseChakra(1); // kurangi chakra

        // ubah sprite player ke pose attack
        if (playerSprite != null && emakAttack != null)
            playerSprite.sprite = emakAttack;

        Debug.Log("Skill 2 aktif: Emak menyembuhkan diri dan menyerang!");

        // heal dulu
        player.Heal(20); // misal heal 20 HP
        yield return new WaitForSeconds(0.4f);

        // serang musuh
        Attack(player, enemy);

        // kembali ke idle pose
        yield return new WaitForSeconds(0.5f);
        if (playerSprite != null && emakIdle != null)
            playerSprite.sprite = emakIdle;

        // akhiri turn
        playerTurn = false;
        Invoke(nameof(EnemyTurn), 1.0f);
    }

}
