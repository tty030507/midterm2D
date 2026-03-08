using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public GameObject levelUpPanel;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;
    public PlayerController player;

    private List<string> upgradePool = new List<string> { "FireSpeed", "MaxHealth", "FireDamage", "MoveSpeed" };

    public void ShowUpgradeOptions()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game

        // Create a temporary list to pick unique upgrades
        List<string> currentOptions = new List<string>(upgradePool);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            // Pick a random index from the available options
            int randomIndex = Random.Range(0, currentOptions.Count);
            string selectedUpgrade = currentOptions[randomIndex];

            // Remove it so the next card doesn't pick the same thing
            currentOptions.RemoveAt(randomIndex);

            choiceTexts[i].text = GetUpgradeDescription(selectedUpgrade);

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => ApplyUpgrade(selectedUpgrade));
        }
    }

    // Helper function to make the text look nicer
    string GetUpgradeDescription(string type)
    {
        switch (type)
        {
            case "FireSpeed": return "Fast Hands: +10% Fire Rate";
            case "MaxHealth": return "Buffer: +20 Max HP";
            case "FireDamage": return "Heavy Gas: +2 Damage";
            case "MoveSpeed": return "Agile: +1 Move Speed";
            default: return "Upgrade";
        }
    }

    void ApplyUpgrade(string type)
    {
        switch (type)
        {
            case "FireSpeed": player.shootSoundInterval *= 0.9f; break; //
            case "MaxHealth": player.maxHp += 20f; player.currentHp = player.maxHp; break; //
            case "FireDamage": player.attackPower += 2f; break; //
            case "MoveSpeed": player.moveSpeed += 1f; break; //
        }

        if (player.healthBar != null) player.healthBar.SetMaxHealth(player.maxHp); //

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }
}