using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CDUI : UICanvas
{
    // Start is called before the first frame update
    public void Resume()
   {
        UIManager.Instance.ResumeGame();
        SoundManager.Instance.PlayVFXSound(4);
        UIManager.Instance.CloseUI<CDUI>(0f);
   }
   public void menu()
   {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddToTotalMoney();
        }
        
        SceneManager.LoadScene(0);
        
       
        UIManager.Instance.CloseUI<MoneyUI>(0f);
        UIManager.Instance.CloseUI<TimeUI>(0f);
        UIManager.Instance.OpenUI<MoneymenuUI>();
        UIManager.Instance.CloseUI<CDUI>(0f);   
        UIManager.Instance.OpenUI<LevelCanvas>();   
        SoundManager.Instance.PlayVFXSound(4);


   }
    public void playagian(){
     if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.ResetGameMoney();
             MoneyManager.Instance.AddToTotalMoney();
        }
        SoundManager.Instance.PlayVFXSound(4);
        Scene currentScene = SceneManager.GetActiveScene();
        UIManager.Instance.CloseUI<CDUI>(0f);
        
        SceneManager.LoadScene(currentScene.name);
   }
   public void qit(){
     UIManager.Instance.QuitGame();
     SoundManager.Instance.PlayVFXSound(4);
   }
   public void backBtn()
    {
        // SoundManager.Instance.PlayClickSound();
        UIManager.Instance.CloseUI<CDUI>(0);UIManager.Instance.ResumeGame();
        SoundManager.Instance.PlayVFXSound(4);
       
    }
   
}
