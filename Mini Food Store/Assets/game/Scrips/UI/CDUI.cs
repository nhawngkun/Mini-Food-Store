using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    UIManager.Instance.ResumeGame();
          
      
       UIManager.Instance.OpenUI<MoneymenuUI>();
        UIManager.Instance.CloseUI<MoneyUI>(0f);
       
        UIManager.Instance.CloseUI<TimeUI>(0f);
       
        
         
       SoundManager.Instance.PlayVFXSound(4);
        StartCoroutine(Load());


   }
   IEnumerator Load(){
    yield return new WaitForSeconds(0.2f);
    SceneManager.LoadScene(0);
    UIManager.Instance.OpenUI<LevelCanvas>();  
    UIManager.Instance.CloseUIDirectly<CDUI>();  
     
   }
    public void playagian(){
         UIManager.Instance.ResumeGame();
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
