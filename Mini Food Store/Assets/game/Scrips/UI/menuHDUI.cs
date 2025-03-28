using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuHDUI : UICanvas 
{
    [System.Serializable]
    public class SceneImageMapping
    {
        public string sceneName;
        public Sprite sceneImage;
    }

    [Tooltip("List of scene to image mappings")]
    public SceneImageMapping[] sceneImageList;

    [Tooltip("Image component to display the source image")]
    public Image displayImage;

    void Start()
    {
        // Initial image setup when the script starts
        DisplayImageForCurrentScene();
    }

    void OnEnable()
    {
        // Subscribe to scene loading event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from scene loading event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Change the image when a new scene is loaded
        DisplayImageForCurrentScene();
    }

    void DisplayImageForCurrentScene()
    {
        // Get the current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if we have an image component and scene image list
        if (displayImage != null && sceneImageList != null && sceneImageList.Length > 0)
        {
            // Find the image mapping for the current scene name
            SceneImageMapping sceneMapping = null;
            foreach (var mapping in sceneImageList)
            {
                if (string.Equals(mapping.sceneName, currentSceneName, System.StringComparison.OrdinalIgnoreCase))
                {
                    sceneMapping = mapping;
                    break;
                }
            }

            // If a mapping is found, display the corresponding image
            if (sceneMapping != null && sceneMapping.sceneImage != null)
            {
                displayImage.sprite = sceneMapping.sceneImage;
            }
            else if (sceneImageList.Length > 0 && sceneImageList[0].sceneImage != null)
            {
                // If no mapping is found, default to the first image in the list
                displayImage.sprite = sceneImageList[0].sceneImage;
            }
        }
    }

    public void backBtn()
    {
        // Uncomment if you want to play a click sound
        // SoundManager.Instance.PlayClickSound();
        
        UIManager.Instance.CloseUI<menuHDUI>(0.3f);
        SoundManager.Instance.PlayVFXSound(4);
    }
}