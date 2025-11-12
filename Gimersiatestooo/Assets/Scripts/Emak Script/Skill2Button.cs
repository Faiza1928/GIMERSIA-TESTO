using UnityEngine;
using UnityEngine.UI;

public class Skill2Button : MonoBehaviour
{
    public Button button;
    public BattleManager battleManager;

    void Start()
    {
        button.onClick.AddListener(OnClickSkill2);
    }

    void Update()
    {
        // aktifkan tombol hanya jika player turn & chakra cukup
        if (battleManager != null && battleManager.chakraManager != null)
            button.interactable = battleManager.chakraManager.HasEnoughChakra(1);
    }

    void OnClickSkill2()
    {
        battleManager.UseSkill2();
    }
}
