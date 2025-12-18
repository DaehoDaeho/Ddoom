using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    public Health playerHealth;
    public Image imageHP;

    private void Start()
    {
        if(playerHealth != null)
        {
            HandlePlayerHP(playerHealth.GetCurrentHp(), playerHealth.GetMaxHp());
        }
    }

    void OnEnable()
    {
        if(playerHealth != null)
        {
            playerHealth.OnChangedHP += HandlePlayerHP;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnChangedHP -= HandlePlayerHP;
        }
    }

    void HandlePlayerHP(float currentHP, float maxHP)
    {
        if(imageHP == null)
        {
            return;
        }

        imageHP.fillAmount = currentHP / maxHP;
    }
}
