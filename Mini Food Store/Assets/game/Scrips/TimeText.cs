using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeText : MonoBehaviour 
{
    [System.Serializable]
    public class SceneTimeMapping
    {
        public string sceneName;
        public float countdownTime = 120f;
    }

    public Text textTime;

    [Tooltip("Countdown times for different scenes")]
    public SceneTimeMapping[] sceneTimeMappings;

    private float currtime;
    private bool gameEnded = false;

    void Start()
    {
        // Set initial countdown time based on current scene
        SetCountdownTimeForCurrentScene();
        ResetTimer();
    }

    void SetCountdownTimeForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Find the matching scene time mapping
        foreach (var mapping in sceneTimeMappings)
        {
            if (string.Equals(mapping.sceneName, currentSceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                // Set the countdown time for the current scene
                currtime = mapping.countdownTime;
                return;
            }
        }

        // If no mapping found, use a default time (e.g., 120 seconds)
        currtime = 120f;
    }

    void Update()
    {
        // If time remains and game hasn't ended
        if (currtime > 0 && !gameEnded)
        {
            currtime -= Time.deltaTime;
            if (currtime <= 0)
            {
                currtime = 0; // Ensure no negative values
                EndGame();
            }
            UpdateTime();
        }
    }

    void UpdateTime()
    {
        int minutes = ((int)currtime / 60);
        int seconds = ((int)currtime % 60);
        textTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void EndGame()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            UIManager.Instance.OpenUI<WinUI>();
        }
    }

    // Method to reset timer
    public void ResetTimer()
    {
        SetCountdownTimeForCurrentScene();
        gameEnded = false;
        UpdateTime();
    }

    // Register scene loaded event listener
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unregister scene loaded event listener
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Handle scene load
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetTimer();
    }
}