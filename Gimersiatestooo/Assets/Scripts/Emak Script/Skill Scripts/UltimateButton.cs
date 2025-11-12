using UnityEngine;
using UnityEngine.UI;

public class UltimateButton : MonoBehaviour
{
    public Button button;
    public BattleManager battleManager;

    void Start()
    {
        button.onClick.AddListener(OnClickUltimate);
    }

    void Update()
    {
        // tombol aktif hanya jika cukup chakra (3 bar)
        if (battleManager != null && battleManager.chakraManager != null)
            button.interactable = battleManager.chakraManager.HasEnoughChakra(3);
    }

    void OnClickUltimate()
    {
        battleManager.UseUltimate();
    }
}
