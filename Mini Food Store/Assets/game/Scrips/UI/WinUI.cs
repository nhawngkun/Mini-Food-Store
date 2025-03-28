using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUI : UICanvas
{
    // Add a Text component to display earned money
    public Text earnedMoneyText;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        // Update the earned money text when WinUI is enabled
        if (earnedMoneyText != null && MoneyManager.Instance != null)
        {
            earnedMoneyText.text = MoneyManager.Instance.GetCurrentGameMoney().ToString("F0") + " $";
        }
    }
    
    public void menu()
    {
        // Cộng tiền từ game vào tổng tiền trước khi chuyển về menu
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddToTotalMoney();
        }
        
        SceneManager.LoadScene(0);
        UIManager.Instance.OpenUI<LevelCanvas>();
        UIManager.Instance.CloseUI<WinUI>(0f);
        UIManager.Instance.CloseUI<MoneyUI>(0f);
        UIManager.Instance.CloseUI<TimeUI>(0f);
        SoundManager.Instance.PlayVFXSound(4);
        UIManager.Instance.OpenUI<MoneymenuUI>();

        
        SoundManager.Instance.PlayVFXSound(4);
    }
    
    public void playagian()
    {
        // Nếu chơi lại, KHÔNG cộng tiền vào tổng tiền
        // Reset tiền trong game về 0
        if (MoneyManager.Instance != null)
        {
           
            MoneyManager.Instance.AddToTotalMoney();
        }
        SoundManager.Instance.PlayVFXSound(4);
        
        Scene currentScene = SceneManager.GetActiveScene();
        UIManager.Instance.CloseUI<WinUI>(0f);
        
        SceneManager.LoadScene(currentScene.name);
    }
}