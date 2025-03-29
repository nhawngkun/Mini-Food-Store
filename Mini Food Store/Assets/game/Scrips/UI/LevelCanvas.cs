using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelCanvas : UICanvas
{
    [SerializeField] public List<LevelData> levels = new List<LevelData>();
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button levelButtonPrefab;

    [Header("Level Number")]
    public Color unlockedTextColor = Color.white;
    [SerializeField] private Color lockedTextColor = Color.gray;

    [Header("Scene Loading")]
    [SerializeField] private float loadDelay = 0.1f; // Delay trước khi load scene mới
    [SerializeField] private GameObject loadingScreen; // Optional: màn hình loading

    [Header("Layout Settings")]
    [SerializeField] private Vector2 buttonSize; // Kích thước của button
    private Coroutine loadLevelCoroutine;

    [Header("Text Settings")]
    [SerializeField] public int fontSize = 14; // Cỡ chữ
    [SerializeField] public Font font; // Phông chữ

    [Header("Level Purchase")]
    [SerializeField] private Button purchaseButtonPrefab; // Prefab cho nút mua
    [SerializeField] private Text priceTextPrefab; // Prefab cho text giá
    [SerializeField] private Color priceTextColor = Color.yellow; // Màu chữ giá tiền
// 1. Thêm biến này vào class LevelCanvas
[SerializeField] private Button resetButton;

// 2. Trong phương thức OnEnable, thêm dòng sau để thiết lập listener cho nút Reset
private void OnEnable()
{
    InitializeLevelButtons();

    // Thiết lập listener cho nút Reset nếu có
    if (resetButton != null)
    {
        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(ResetAllLevels);
    }

    // Ẩn loading screen nếu có
    if (loadingScreen != null)
    {
        loadingScreen.SetActive(false);
    }
}
   
    private void OnDisable()
    {
        if (loadLevelCoroutine != null)
        {
            StopCoroutine(loadLevelCoroutine);
            loadLevelCoroutine = null;
        }
    }

    public void backBtn()
    {
        // SoundManager.Instance.PlayClickSound();
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<HomeCanvas>();
    }
    // Thêm phương thức này vào class LevelCanvas
public void ResetAllLevels()
{
    // Reset tất cả các key lưu trạng thái mở khóa level
    for (int i = 0; i < levels.Count; i++)
    {
        string key = $"Level_{i}_Unlocked";
        // Xóa key khỏi PlayerPrefs
        PlayerPrefs.DeleteKey(key);
        
        // Reset lại trạng thái isUnlocked trong đối tượng LevelData
        if (i == 0)
        {
            // Level đầu tiên vẫn được mở khóa
            levels[i].isUnlocked = true;
            PlayerPrefs.SetInt(key, 1);
        }
        else
        {
            levels[i].isUnlocked = false;
        }
    }
    
    // Xóa danh sách scene đã lưu trong LevelManager
    PlayerPrefs.DeleteKey("SavedScenes");
    if (LevelManager.Instance != null)
    {
        // Chỉ giữ lại level đầu tiên trong danh sách scene (nếu có)
        LevelManager.Instance.sceneNames.Clear();
        if (levels.Count > 0)
        {
            LevelManager.Instance.sceneNames.Add(levels[0].sceneName);
        }
        LevelManager.Instance.SaveGame();
    }
    
    // Lưu thay đổi vào PlayerPrefs
    PlayerPrefs.Save();
    
    // Cập nhật lại giao diện
    InitializeLevelButtons();
    
    Debug.Log("Đã reset tất cả các level về trạng thái ban đầu");
}

    private void InitializeLevelButtons()
    {
        // Đầu tiên, tải trạng thái mở khóa của các level
        LoadLevelProgress();
        
        // Xoá các button hiện có
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < levels.Count; i++)
        {
            Button btn = Instantiate(levelButtonPrefab, buttonContainer);
            levels[i].levelButton = btn;

            RectTransform rectTransform = btn.GetComponent<RectTransform>();
            rectTransform.sizeDelta = buttonSize;

            Image buttonImage = btn.GetComponent<Image>();
            Text buttonText = btn.GetComponentInChildren<Text>();

            if (buttonImage != null)
            {
                // Sử dụng sprite riêng cho từng nút
                buttonImage.sprite = levels[i].isUnlocked ? levels[i].unlockedSprite : levels[i].lockedSprite;
            }

            if (buttonText != null)
            {
                buttonText.text = levels[i].levelName;
                buttonText.color = levels[i].isUnlocked ? unlockedTextColor : lockedTextColor;
                buttonText.fontSize = fontSize; // Thiết lập cỡ chữ
                buttonText.font = font; // Thiết lập phông chữ
            }

            // Kiểm tra xem level đã mở khóa chưa
            if (levels[i].isUnlocked)
            {
                btn.interactable = true;
                int levelIndex = i;
                btn.onClick.AddListener(() => LoadLevel(levelIndex));
            }
            else
            {
                btn.interactable = false;
                // Hiển thị giá và nút mua cho level chưa mở khóa
                AddPurchaseButton(btn.gameObject, i);
            }

            PlaySpawnAnimation(btn.gameObject, i * 0.1f);
        }
        
        // Kiểm tra thêm từ SavedScenes trong LevelManager
        UpdateLevelsFromSavedScenes();
    }

    private void LoadLevelProgress()
    {
        // Level đầu tiên luôn mở khóa
        if (levels.Count > 0)
        {
            levels[0].isUnlocked = true;
        }
        
        // Tải trạng thái mở khóa từ PlayerPrefs
        for (int i = 0; i < levels.Count; i++)
        {
            string key = $"Level_{i}_Unlocked";
            // Level 0 mặc định mở khóa, các level khác kiểm tra từ PlayerPrefs
            levels[i].isUnlocked = PlayerPrefs.GetInt(key, i == 0 ? 1 : 0) == 1;
            
            // Debug
            if (levels[i].isUnlocked)
            {
                Debug.Log($"Level {i} ({levels[i].levelName}) đã được mở khóa từ PlayerPrefs");
            }
        }
    }

    private void UpdateLevelsFromSavedScenes()
    {
        // Kiểm tra thêm từ danh sách cảnh đã lưu trong LevelManager
        string savedScenes = PlayerPrefs.GetString("SavedScenes", string.Empty);
        if (!string.IsNullOrEmpty(savedScenes))
        {
            List<string> savedSceneList = new List<string>(savedScenes.Split(','));
            
            for (int i = 0; i < levels.Count; i++)
            {
                // Nếu scene đã lưu trong danh sách savedScenes nhưng level chưa được đánh dấu mở khóa
                if (!levels[i].isUnlocked && savedSceneList.Contains(levels[i].sceneName))
                {
                    levels[i].isUnlocked = true;
                    
                    // Cập nhật trạng thái mở khóa trong PlayerPrefs
                    PlayerPrefs.SetInt($"Level_{i}_Unlocked", 1);
                    
                    // Cập nhật giao diện nút
                    UpdateButtonAppearance(i);
                    
                    Debug.Log($"Level {i} ({levels[i].levelName}) đã được mở khóa từ SavedScenes");
                }
            }
            
            PlayerPrefs.Save();
        }
    }

    private void UpdateButtonAppearance(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count && levels[levelIndex].levelButton != null)
        {
            Button btn = levels[levelIndex].levelButton;
            
            // Cập nhật hình ảnh
            Image buttonImage = btn.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = levels[levelIndex].unlockedSprite;
            }
            
            // Cập nhật màu chữ
            Text buttonText = btn.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.color = unlockedTextColor;
            }
            
            // Bật tương tác cho nút
            btn.interactable = true;
            
            // Xóa nút mua và text giá nếu có
            foreach (Transform child in btn.transform)
            {
                if (child.name == "PriceText" || (child.GetComponent<Button>() != null && child.GetComponent<Button>() != btn))
                {
                    Destroy(child.gameObject);
                }
            }
            
            // Thêm listener cho nút level
            int index = levelIndex;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => LoadLevel(index));
        }
    }

    private void AddPurchaseButton(GameObject levelButton, int levelIndex)
    {
        // Sử dụng prefab cho text giá thay vì tạo mới
        if (priceTextPrefab != null)
        {
            Text priceText = Instantiate(priceTextPrefab, levelButton.transform, false);
            priceText.text = levels[levelIndex].price.ToString("F0") + " $";
            priceText.color = priceTextColor;
            
            // Vẫn có thể chỉnh sửa font và fontSize nếu cần
            priceText.fontSize = fontSize;
            priceText.font = font;
            
            // Đảm bảo giữ tên là "PriceText" để dễ dàng tìm và xóa sau này
            priceText.gameObject.name = "PriceText";
        }
        
        // Tạo nút mua
        if (purchaseButtonPrefab != null)
        {
            Button purchaseBtn = Instantiate(purchaseButtonPrefab, levelButton.transform);
            purchaseBtn.transform.SetAsLastSibling(); // Đặt nút mua lên trên cùng
            
            // Thiết lập vị trí cho nút mua (phía dưới nút level)
            RectTransform purchaseRectTransform = purchaseBtn.GetComponent<RectTransform>();
            purchaseRectTransform.anchorMin = new Vector2(0.5f, 0);
            purchaseRectTransform.anchorMax = new Vector2(0.5f, 0);
            purchaseRectTransform.anchoredPosition = new Vector2(0, -60);
            purchaseRectTransform.sizeDelta = new Vector2(130, 80);
            
            // Thêm listener cho nút mua
            int index = levelIndex;
            purchaseBtn.onClick.AddListener(() => PurchaseLevel(index));
            
            // Thêm text "Mua" cho nút
            Text buyText = purchaseBtn.GetComponentInChildren<Text>();
            if (buyText != null)
            {
                buyText.text = "Mua";
                buyText.fontSize = fontSize;
                buyText.font = font;
            }
        }
    }

    private void PurchaseLevel(int levelIndex)
    {
        // Kiểm tra xem người chơi có đủ tiền không
        float totalMoney = MoneyManager.Instance.GetTotalMoney();
        float levelPrice = levels[levelIndex].price;
        
        if (totalMoney >= levelPrice)
        {
            // Trừ tiền
            MoneyManager.Instance.DeductFromTotalMoney(levelPrice);
            
            // Mở khóa level
            levels[levelIndex].isUnlocked = true;
            SoundManager.Instance.PlayVFXSound(3);
            
            // Cập nhật giao diện
            UpdateButtonAppearance(levelIndex);
            
            // Lưu trạng thái đã mở khóa
            PlayerPrefs.SetInt($"Level_{levelIndex}_Unlocked", 1);
            PlayerPrefs.Save();
            
            Debug.Log($"Đã mua và mở khóa level {levelIndex} ({levels[levelIndex].levelName})");
            
            // Thêm scene vào danh sách đã mở khóa trong LevelManager
            if (LevelManager.Instance != null)
            {
                if (!LevelManager.Instance.sceneNames.Contains(levels[levelIndex].sceneName))
                {
                    LevelManager.Instance.sceneNames.Add(levels[levelIndex].sceneName);
                    LevelManager.Instance.SaveGame();
                }
            }
        }
        else
        {
            // Hiển thị thông báo không đủ tiền
            Debug.Log("Không đủ tiền để mua level này!");
            // Có thể thêm hiệu ứng hoặc thông báo UI ở đây
        }
    }

    private IEnumerator CheckListenerAdded(Button btn, string levelName)
    {
        yield return null; // Đợi một khung hình

        if (btn.onClick.GetPersistentEventCount() == 0)
        {
            Debug.LogWarning($"Chưa thêm listener cho nút của cấp độ {levelName}");
        }
    }

    private void PlaySpawnAnimation(GameObject button, float delay)
    {
        // Set initial scale to zero
        button.transform.localScale = Vector3.zero;

        // Start spawn animation
        StartCoroutine(SpawnAnimationRoutine(button, delay));
    }

    private IEnumerator SpawnAnimationRoutine(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Elastic ease out effect
            float scale = Mathf.Sin(-13f * (progress + 1) * Mathf.PI * 0.5f) * Mathf.Pow(2f, -10f * progress) + 1f;
            button.transform.localScale = Vector3.one * scale;

            yield return null;
        }
        button.transform.localScale = Vector3.one;
    }

    private void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count && levels[levelIndex].isUnlocked)
        {
            // SoundManager.Instance.PlayClickSound();
           
            StartCoroutine(LoadLevelSequence(levelIndex));
        }
    }

    private IEnumerator LoadLevelSequence(int levelIndex)
    {
        // 1. Show loading screen if available
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        UIManager.Instance.CloseUI<MoneymenuUI>(0f);
       

        yield return new WaitForSeconds(loadDelay);
      
         
        // 4. Load new scene asynchronously
        SceneManager.LoadScene(levels[levelIndex].sceneName);
        UIManager.Instance.OpenUI<GamePlayCanvas>();
        UIManager.Instance.OpenUI<MoneyUI>();
        UIManager.Instance.OpenUI<TimeUI>();
        

        // Đợi vài khung hình để đảm bảo scene mới đã được khởi tạo hoàn toàn
        yield return null; // Đợi một khung hình
        yield return null; // Đợi thêm một khung hình

        // Đóng tất cả UI hiện tại
        
        
        UIManager.Instance.OpenUI<GamePlayCanvas>();
         

        // Thiết lập UI cho gameplay
        SetupGameplayUI();
        UIManager.Instance.CloseUIDirectly<LevelCanvas>();
    }

    private void SetupGameplayUI()
    {
        // Ensure we're on the main thread
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }

        // Open Gameplay Canvas
        // UIManager.Instance.OpenUI<GamePlayCanvas>();
    }

    public void UnlockNextLevel(int currentLevelIndex)
    {
        if (currentLevelIndex + 1 < levels.Count)
        {
            int nextLevelIndex = currentLevelIndex + 1;
            levels[nextLevelIndex].isUnlocked = true;

            // Cập nhật giao diện nút
            UpdateButtonAppearance(nextLevelIndex);
            
            // Lưu trạng thái mở khóa
            PlayerPrefs.SetInt($"Level_{nextLevelIndex}_Unlocked", 1);
            PlayerPrefs.Save();
            
            Debug.Log($"Đã mở khóa level tiếp theo: {nextLevelIndex} ({levels[nextLevelIndex].levelName})");
        }
    }
}

[System.Serializable]
public class LevelData
{
    public string levelName;
    public string sceneName;
    [HideInInspector] public Button levelButton;
    public bool isUnlocked = false;
    
    // Price for unlocking the level
    public float price = 100f;
    
    // Sprite riêng cho từng nút
    public Sprite unlockedSprite;
    public Sprite lockedSprite;
}