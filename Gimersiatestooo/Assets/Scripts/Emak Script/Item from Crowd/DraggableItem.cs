using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DraggableItem : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player"; // tag player di scene
    public float snapDistance = 1.0f;   // seberapa dekat ke player biar dianggap “drop”
    
    private bool isDragging = false;
    private Camera mainCam;
    private Vector3 startPosition;
    private BattleManager battleManager;

    void Start()
    {
        mainCam = Camera.main;
        startPosition = transform.position;
        battleManager = Object.FindFirstObjectByType<BattleManager>();
    }

    void Update()
    {
        // klik kiri untuk mulai drag
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

            if (hit != null && hit.gameObject == gameObject)
                isDragging = true;
        }

        // geser posisi item
        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            transform.position = new Vector3(mouseWorld.x, mouseWorld.y, 0f);
        }

        // lepas drag
        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            CheckDrop();
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = -mainCam.transform.position.z;
        return mainCam.ScreenToWorldPoint(mousePos);
    }

    void CheckDrop()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null)
        {
            Debug.LogWarning("Player tidak ditemukan!");
            return;
        }

        float dist = Vector2.Distance(transform.position, playerObj.transform.position);
        if (dist <= snapDistance)
        {
            ApplyItemEffect();
            Destroy(gameObject);
        }
        else
        {
            // kalau jauh, balik ke posisi semula
            transform.position = startPosition;
        }
    }

    void ApplyItemEffect()
    {
        var debuffManager = Object.FindFirstObjectByType<DebuffManager>();
        if (debuffManager == null)
        {
            Debug.LogWarning("DebuffManager tidak ditemukan di scene!");
            return;
        }

        string lowerName = name.ToLower();
        if (lowerName.Contains("sandal"))
        {
            debuffManager.QueueDebuff(DebuffType.Sandal);
        }
        else if (lowerName.Contains("sapu"))
        {
            debuffManager.QueueDebuff(DebuffType.SapuLidi);
        }

        Debug.Log($"Item {name} digunakan — efek akan aktif di turn musuh berikutnya!");
    }


    IEnumerator SandalDebuff()
    {
        Debug.Log("🩴 Debuff Sandal aktif! Damage enemy setengah selama 1 turn.");
        // contoh pengaruh sederhana:
        // misal kamu tambahkan multiplier di BattleManager
        float originalDamageMult = 1f;
        float debuffMult = 0.5f;

        battleManager.enemyPrefab.damageMultiplier = debuffMult;
        yield return new WaitForSeconds(2f); // nanti bisa ganti pakai sistem turn
        battleManager.enemyPrefab.damageMultiplier = originalDamageMult;
        Debug.Log("🩴 Debuff Sandal berakhir.");
    }

    IEnumerator SapuDebuff()
    {
        Debug.Log("🧹 Debuff Sapu aktif! Enemy tidak bisa pakai skill 1 turn.");
        battleManager.enemyPrefab.canUseSkill = false;
        yield return new WaitForSeconds(2f);
        battleManager.enemyPrefab.canUseSkill = true;
        Debug.Log("🧹 Debuff Sapu berakhir.");
    }
}
