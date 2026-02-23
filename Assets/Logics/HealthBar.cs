using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text healthText; 

    private float maxHpValue;

    public void SetMaxHealth(float health)
    {
        if (slider != null)
        {
            slider.maxValue = health;
            slider.value = health;
            maxHpValue = health; 
            UpdateText(health);
        }
    }

    public void SetHealth(float health)
    {
        if (slider != null)
        {
            slider.value = health;
            UpdateText(health);
        }
    }

    void UpdateText(float currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = Mathf.Max(0, currentHealth).ToString("F0") + " / " + maxHpValue.ToString("F0");
        }
    }
}