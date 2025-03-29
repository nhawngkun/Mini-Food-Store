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

    // Singleton instance để có thể truy cập từ bất kỳ script nào
    private static TimeText _instance;
    public static TimeText Instance { get { return _instance; } }

    void Awake()
    {
        // Tạo singleton pattern để TimeText không bị hủy khi load scene mới
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Giữ đối tượng này khi chuyển scene
        }
        else
        {
            Destroy(gameObject); // Hủy nếu đã tồn tại một instance khác
            return;
        }
    }

    void Start()
    {
        // Khởi tạo thời gian dựa trên scene hiện tại
        Debug.Log("TimeText Start called");
        InitializeTimer();
    }

    // Phương thức khởi tạo timer
    void InitializeTimer()
    {
        // Tìm lại tham chiếu Text nếu cần
        if (textTime == null)
        {
            textTime = GetComponent<Text>();
            Debug.LogWarning("textTime was null, tried to find it again");
        }

        // Đặt thời gian đếm ngược dựa trên scene hiện tại
        SetCountdownTimeForCurrentScene();
        gameEnded = false;
        UpdateTime();
        
        Debug.Log("Timer initialized for scene: " + SceneManager.GetActiveScene().name + " with time: " + currtime);
    }

    void SetCountdownTimeForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Setting countdown time for scene: " + currentSceneName);

        // Kiểm tra mảng sceneMappings có tồn tại không
        if (sceneTimeMappings == null || sceneTimeMappings.Length == 0)
        {
            Debug.LogWarning("No scene time mappings defined. Using default time of 120 seconds.");
            currtime = 120f;
            return;
        }

        // Tìm mapping phù hợp với scene hiện tại
        bool mappingFound = false;
        foreach (var mapping in sceneTimeMappings)
        {
            if (mapping == null)
            {
                Debug.LogWarning("Found a null mapping in sceneTimeMappings array");
                continue;
            }

            Debug.Log("Checking mapping: " + mapping.sceneName);
            if (string.Equals(mapping.sceneName, currentSceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                // Thiết lập thời gian đếm ngược cho scene hiện tại
                currtime = mapping.countdownTime;
                Debug.Log("Found mapping! Setting time to: " + currtime);
                mappingFound = true;
                break;
            }
        }

        // Nếu không tìm thấy mapping, sử dụng thời gian mặc định
        if (!mappingFound)
        {
            Debug.LogWarning("No mapping found for scene: " + currentSceneName + ". Using default time of 120 seconds.");
            currtime = 120f;
        }
    }

    void Update()
    {
        // Nếu còn thời gian và game chưa kết thúc
        if (currtime > 0 && !gameEnded)
        {
            currtime -= Time.deltaTime;
            if (currtime <= 0)
            {
                currtime = 0; // Đảm bảo không có giá trị âm
                EndGame();
            }
            UpdateTime();
        }
    }

    void UpdateTime()
    {
        if (textTime == null)
        {
            Debug.LogError("textTime is null! Cannot update UI.");
            return;
        }

        int minutes = ((int)currtime / 60);
        int seconds = ((int)currtime % 60);
        textTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void EndGame()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            Debug.Log("Game ended! Time ran out.");
            
            // Kiểm tra UIManager tồn tại trước khi gọi
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenUI<WinUI>();
            }
            else
            {
                Debug.LogError("UIManager.Instance is null! Cannot open WinUI.");
            }
        }
    }

    // Phương thức public để reset timer từ bên ngoài
    public void ResetTimer()
    {
        Debug.Log("ResetTimer called");
        gameEnded = false;
        SetCountdownTimeForCurrentScene();
        UpdateTime();
    }

    // Đăng ký event listener cho scene loaded
    void OnEnable()
    {
        Debug.Log("TimeText OnEnable - Registering scene loaded event");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Hủy đăng ký event listener
    void OnDisable()
    {
        Debug.Log("TimeText OnDisable - Unregistering scene loaded event");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Xử lý khi scene được load
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name + " with mode: " + mode);
        Debug.Log("gameEnded before reset: " + gameEnded);
        
        // Đợi một frame để đảm bảo scene đã load xong
        StartCoroutine(ResetTimerNextFrame());
    }
    
    // Đợi một frame trước khi reset timer
    private IEnumerator ResetTimerNextFrame()
    {
        yield return null; // Đợi đến frame tiếp theo
        
        // Tìm lại tham chiếu Text nếu cần 
        // (trong trường hợp UI được tạo lại sau khi scene load)
        if (textTime == null)
        {
            // Tìm text UI trong scene mới
            Text[] textComponents = FindObjectsOfType<Text>();
            foreach (Text txt in textComponents)
            {
                if (txt.gameObject.name.Contains("Time") || txt.gameObject.CompareTag("TimeText"))
                {
                    textTime = txt;
                    Debug.Log("Found new textTime reference: " + txt.gameObject.name);
                    break;
                }
            }
            
            if (textTime == null)
            {
                Debug.LogError("Could not find any suitable Text component for timer!");
            }
        }
        
        ResetTimer();
        Debug.Log("currtime after reset: " + currtime);
        Debug.Log("gameEnded after reset: " + gameEnded);
    }
    
    // Phương thức để debug
    public void DebugTimerStatus()
    {
        Debug.Log("Current scene: " + SceneManager.GetActiveScene().name);
        Debug.Log("Current time: " + currtime);
        Debug.Log("gameEnded: " + gameEnded);
        Debug.Log("textTime null?: " + (textTime == null));
    }
}