using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayCanvas : UICanvas
{
    // Start is called before the first frame update
   public void Pause()
   {
        UIManager.Instance.PauseGame();
        SoundManager.Instance.PlayVFXSound(4);


   }
   public void Resume()
   {
        UIManager.Instance.ResumeGame();
        SoundManager.Instance.PlayVFXSound(4);
   }

   public void menu()
   {
        SceneManager.LoadScene(0);
        UIManager.Instance.OpenUI<LevelCanvas>();
        
        SoundManager.Instance.PlayVFXSound(4);


   }

      public void playagian(){
      Scene currentScene = SceneManager.GetActiveScene();
     SceneManager.LoadScene(currentScene.name);
   }
   public void Back(){
    int nextSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
    
    if(nextSceneIndex ==0){
     SceneManager.LoadScene(0);
     UIManager.Instance.OpenUI<LevelCanvas>();
     UIManager.Instance.CloseUIDirectly<GamePlayCanvas>();
     SoundManager.Instance.PlayVFXSound(4);

    }
  
        SceneManager.LoadScene(nextSceneIndex);
        SoundManager.Instance.PlayVFXSound(4);
       
       
   }
   public void qit(){
     UIManager.Instance.QuitGame();
     SoundManager.Instance.PlayVFXSound(4);
   }
   public void caidat(){
    UIManager.Instance.OpenUI<CDUI>();
    UIManager.Instance.PauseGame();
     SoundManager.Instance.PlayVFXSound(4);

   }
   public void hd(){
     UIManager.Instance.OpenUI<menuHDUI>();
     SoundManager.Instance.PlayVFXSound(2);
      
   }
   public void cd(){
     UIManager.Instance.OpenUI<chidan>();
     SoundManager.Instance.PlayVFXSound(2);
      
   }

   // Thêm hàm mới để điều khiển âm thanh bật/tắt
   public void ToggleSound()
   {
      // Đảo ngược trạng thái TurnOn của SoundManager
      SoundManager.Instance.TurnOn = !SoundManager.Instance.TurnOn;
      
      // Phát âm thanh khi nhấn nút (chỉ phát nếu âm thanh đang bật)
      if (SoundManager.Instance.TurnOn)
      {
          SoundManager.Instance.PlayVFXSound(4);
      }
   }
   
}