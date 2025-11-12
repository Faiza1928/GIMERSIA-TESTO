using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{

    [Header("Status Effects")]
    public DebuffManager debuffManager;


    [Header("Items")]
    public ItemManager itemManager;
    private int turnCount = 1;


    [Header("Extra Prefabs")]
    public GameObject tanteWajanPrefab;
    public GameObject objekKiriPrefab;

    [Header("Player Sprites")]
    public Sprite emakIdle;
    public Sprite emakAttack;
    public Sprite emakSkill2Pose;

    [Header("Ultimate UI")]
    public GameObject ultimateOverlay; // UI Image fullscreen
    public Image ultimateOverlayImage; // komponen Image-nya

    [Header("Ultimate Sprites (4 pose)")]
    public Sprite emakUltiPose1;
    public Sprite emakUltiPose2;
    public Sprite emakUltiPose3;
    public Sprite emakUltiPose4;

    [Header("Enemy Sprites EnemyTurn")]
    public Sprite tetanggaIdle;
    public Sprite tetanggaAttack;

    private Vector3 enemyOriginalPosition;
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
    private GameObject objekKiriInstance;



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

        // SIMPAN posisi asli enemy sekali di sini
        enemy.transform.position += new Vector3(8f, -1f, 0f);
        enemyOriginalPosition = enemy.transform.position;

        if (tanteWajanPrefab != null)
        {
            tanteWajanInstance = Instantiate(tanteWajanPrefab);
            tanteWajanInstance.SetActive(true);
        }

        if (objekKiriPrefab != null)
        {
            objekKiriInstance = Instantiate(objekKiriPrefab);
            objekKiriInstance.SetActive(false); // HIDE dulu di awal (player turn)
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

        if (debuffManager != null)
            debuffManager.OnEnemyTurnStart(enemy);

        if (enemy.IsAlive)
            StartCoroutine(PerformEnemyAttack());

        turnCount++;
        itemManager.CheckSpawnItems(turnCount);
        // Balik lagi ke player turn setelah delay
        //Invoke(nameof(ReturnToPlayerTurn), 1.5f);
    }

    // === KEMBALI KE PLAYER TURN ===
    void ReturnToPlayerTurn()
    {
        if (debuffManager != null)
            debuffManager.OnEnemyTurnEnd(enemy);

        playerTurn = true;
        SetBattleView(true);

        turnCount++;
        itemManager.CheckSpawnItems(turnCount);
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

        // Objek Kiri: muncul saat ENEMY turn
        if (objekKiriInstance != null)
            objekKiriInstance.SetActive(!playerTurn); // ← kebalikan dari playerTurn

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
        // Hitung damage dasar
        int baseDamage = 9;

        // Terapkan multiplier (misalnya 0.5 untuk sandal, 0 untuk sapu)
        float multiplier = Mathf.Clamp(attacker.damageMultiplier, 0f, 10f);
        int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);

        // Terapkan damage ke target
        target.TakeDamage(finalDamage);

        // Log informasi jelas
        Debug.Log($"{attacker.characterName} menyerang {target.characterName} dengan base {baseDamage} → final {finalDamage} (x{multiplier})");

        // Update bar batas max HP
        playerHPBar.maxValue = player.maxHP;
        enemyHPBar.maxValue = enemy.maxHP;

        // Cek kematian
        CheckBattleEnd();
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
        // Pakai posisi yang sudah disimpan, bukan ambil dari transform sekarang
        //Vector3 attackPos = enemyOriginalPosition + new Vector3(6f, -1f, 0f);

        if (enemySprite != null && tetanggaAttack != null)
        {
            enemySprite.sprite = tetanggaAttack;
            //enemy.transform.position = attackPos; // geser ke kanan

        }

        yield return new WaitForSeconds(0.6f);

        Attack(enemy, player);

        if (enemySprite != null && tetanggaIdle != null)
        {
            enemySprite.sprite = tetanggaIdle;
            enemy.transform.position = enemyOriginalPosition; // balik ke posisi asli
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


        Debug.Log("Skill 2 aktif: Emak menyembuhkan diri dan menyerang!");
        // ubah sprite player ke pose attack
        if (playerSprite != null && emakSkill2Pose != null)
            playerSprite.sprite = emakSkill2Pose;

        // heal dulu
        player.Heal(9); // misal heal 20 HP
        yield return new WaitForSeconds(0.3f);
        Attack(player, enemy);
        yield return new WaitForSeconds(0.3f);

        if (playerSprite != null && emakIdle != null)
            playerSprite.sprite = emakIdle;

        // akhiri turn
        playerTurn = false;
        Invoke(nameof(EnemyTurn), 1.0f);
    }

    // === SKILL 3: Ultimate Attack ===
    public void UseUltimate()
    {
        if (!playerTurn)
        {
            Debug.Log("Bukan giliran player!");
            return;
        }

        if (!chakraManager.HasEnoughChakra(3))
        {
            Debug.Log("Chakra tidak cukup untuk Ultimate!");
            return;
        }

        StartCoroutine(PerformUltimate());
    }

    private IEnumerator PerformUltimate()
    {
        chakraManager.UseChakra(3); // pakai 3 bar chakra

        // Ubah sprite ke pose serang
        if (playerSprite != null && emakAttack != null)
            playerSprite.sprite = emakAttack;

        Debug.Log("🔥 ULTI AKTIF! Emak mengeluarkan jurus pamungkas!");

        // === POSE 1: Fullscreen UI ===
        if (ultimateOverlay != null)
        {
            ultimateOverlay.SetActive(true);
            if (ultimateOverlayImage != null && emakUltiPose1 != null)
                ultimateOverlayImage.sprite = emakUltiPose1;
        }
        yield return new WaitForSeconds(1f);

        // ✅ HIDE overlay sebelum lanjut ke Pose 2-4
        if (ultimateOverlay != null)
            ultimateOverlay.SetActive(false);

        // Pose 2
        if (playerSprite != null && emakUltiPose2 != null)
            playerSprite.sprite = emakUltiPose2;
        yield return new WaitForSeconds(0.4f);

        // Pose 3
        if (playerSprite != null && emakUltiPose3 != null)
            playerSprite.sprite = emakUltiPose3;
        yield return new WaitForSeconds(0.4f);

        // Pose 4 (serangan!)
        if (playerSprite != null && emakUltiPose4 != null)
            playerSprite.sprite = emakUltiPose4;
        yield return new WaitForSeconds(0.4f);

        // Serangan besar (damage tetap 15)
        int ultiDamage = 15;
        float multiplier = Mathf.Clamp(player.damageMultiplier, 0f, 10f);
        int finalDamage = Mathf.RoundToInt(ultiDamage * multiplier);

        enemy.TakeDamage(finalDamage);

        Debug.Log($"{player.characterName} menggunakan ULTIMATE! Base {ultiDamage} → Final {finalDamage}");

        yield return new WaitForSeconds(0.5f);

        // Balik ke pose idle
        if (playerSprite != null && emakIdle != null)
            playerSprite.sprite = emakIdle;

        playerTurn = false;
        Invoke(nameof(EnemyTurn), 1.0f);
    }

    // === WIN/LOSE LOGIC ===
    void CheckBattleEnd()
    {
        if (!player.IsAlive)
        {
            Debug.Log("💀 PLAYER KALAH!");
            PlayerLose();
        }
        else if (!enemy.IsAlive)
        {
            Debug.Log("🎉 PLAYER MENANG!");
            PlayerWin();
        }
    }

    void PlayerWin()
    {
        // Tampilkan UI Win atau pindah scene
        Debug.Log("Loading Win Scene...");
        // SceneManager.LoadScene("WinScene"); // uncomment nanti
    }

    void PlayerLose()
    {
        // Tampilkan UI Lose atau pindah scene
        Debug.Log("Loading Lose Scene...");
        // SceneManager.LoadScene("LoseScene"); // uncomment nanti
    }

}
