using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeCanvas : UICanvas
{
    // Start is called before the first frame update
    public void playBtn()
    {
        UIManager.Instance.OpenUI<LevelCanvas>();
        UIManager.Instance.OpenUI<MoneymenuUI>();
        UIManager.Instance.OpenUI<GamePlayCanvas>();
              
        UIManager.Instance.CloseUI<HomeCanvas>(0.3f);
        SoundManager.Instance.PlayVFXSound(4);
    }
    
    public void qit()
    {
        UIManager.Instance.QuitGame();
        SoundManager.Instance.PlayVFXSound(4);
    }

    // Add new reset function to reset all levels and money
    public void resetAll()
    {
        // Reset all money to zero
        if (MoneyManager.Instance != null)
        {
            // Reset current game money
            MoneyManager.Instance.ResetGameMoney();
            
            // Reset total money by deducting all current total money
            float currentTotal = MoneyManager.Instance.GetTotalMoney();
            MoneyManager.Instance.DeductFromTotalMoney(currentTotal);
            
            // Update money display
            MoneyManager.Instance.UpdateMoneyDisplay();
            
            // Force save money reset to PlayerPrefs
            PlayerPrefs.SetFloat("TotalMoney", 0);
            
            Debug.Log("All money has been reset to zero");
        }
        
        // Reset all level unlocks directly through PlayerPrefs
        // First, find how many levels might exist by searching for all possible keys
        for (int i = 0; i < 100; i++) // Assuming no more than 100 levels
        {
            string key = $"Level_{i}_Unlocked";
            if (PlayerPrefs.HasKey(key))
            {
                // Delete all level unlock keys except Level_0
                if (i == 0)
                {
                    // Keep level 0 as unlocked
                    PlayerPrefs.SetInt(key, 1);
                }
                else
                {
                    // Reset all other levels to locked
                    PlayerPrefs.SetInt(key, 0);
                }
                Debug.Log($"Reset level {i} unlock state");
            }
        }
        
        // Reset saved scenes list
        PlayerPrefs.DeleteKey("SavedScenes");
        
        // Make sure changes are saved
        PlayerPrefs.Save();
        
        // Play sound effect
        SoundManager.Instance.PlayVFXSound(4);
        
        Debug.Log("Complete reset of game progress successful");
        
        // Force the LevelCanvas to reload if it's active
        LevelCanvas levelCanvas = FindObjectOfType<LevelCanvas>();
        if (levelCanvas != null)
        {
            // Notify user that they need to restart the level screen to see changes
            Debug.Log("Please reopen the level screen to see reset changes");
        }
    }
}