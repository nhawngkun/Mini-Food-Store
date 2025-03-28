using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chidan : UICanvas
{
    // Start is called before the first frame update
     public void backBtn()
    {
        // Uncomment if you want to play a click sound
         SoundManager.Instance.PlayVFXSound(4);
        
        UIManager.Instance.CloseUI<chidan>(0.3f);
    }
}
