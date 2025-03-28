
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
              
        UIManager.Instance.CloseUI<HomeCanvas>(0.3f);
        SoundManager.Instance.PlayVFXSound(4);
    }

}
