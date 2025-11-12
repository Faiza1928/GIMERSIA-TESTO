using UnityEngine;


//INI kenapa multipliernya gak mau 0000 yaampun sumpil aih rawegjhj
public enum DebuffType
{
    None,
    Sandal,
    SapuLidi
}

public class DebuffManager : MonoBehaviour
{
    [Header("References")]
    public BattleManager battleManager;

    private DebuffType pendingDebuff = DebuffType.None;
    private DebuffType activeDebuff = DebuffType.None;

    private bool debuffAppliedThisTurn = false;

    void Start()
    {
        if (battleManager == null)
            battleManager = Object.FindFirstObjectByType<BattleManager>();
    }

    // === Dipanggil dari DraggableItem ===
    public void QueueDebuff(DebuffType debuff)
    {
        pendingDebuff = debuff;
        Debug.Log($"[DebuffManager] Pending debuff ditambahkan: {debuff}");
    }

    // === Saat masuk giliran musuh ===
    public void OnEnemyTurnStart(Character enemy)
    {
        if (pendingDebuff == DebuffType.None)
            return;

        activeDebuff = pendingDebuff;
        pendingDebuff = DebuffType.None;

        switch (activeDebuff)
        {
            case DebuffType.Sandal:
                enemy.damageMultiplier = 0.5f;
                Debug.Log("🩴 Sandal aktif: damage musuh jadi setengah!");
                break;

            case DebuffType.SapuLidi:
                enemy.canUseSkill = false;
                Debug.Log("🧹 Sapu Lidi aktif: musuh tidak bisa menyerang!");
                break;
        }

        debuffAppliedThisTurn = true;
    }

    // === Saat giliran musuh selesai ===
    public void OnEnemyTurnEnd(Character enemy)
    {
        if (!debuffAppliedThisTurn)
            return;

        switch (activeDebuff)
        {
            case DebuffType.Sandal:
                enemy.damageMultiplier = 1f;
                Debug.Log("🩴 Sandal berakhir, damage musuh kembali normal.");
                break;

            case DebuffType.SapuLidi:
                enemy.canUseSkill = true;
                Debug.Log("🧹 Sapu Lidi berakhir, musuh bisa menyerang lagi.");
                break;
        }

        activeDebuff = DebuffType.None;
        debuffAppliedThisTurn = false;
    }
}
