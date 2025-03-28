using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    
    public Text moneyMenuText;    // Hiển thị tổng tiền (menu)
    public Text moneyInGameText;  // Hiển thị tiền trong game hiện tại
    
    private static float totalMoney = 0;       // Tổng tiền tích lũy
    private static float currentGameMoney = 0; // Tiền kiếm được trong lượt chơi hiện tại

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Tải tiền từ PlayerPrefs khi khởi tạo
        LoadMoney();
        
        UpdateMoneyDisplay();
    }

    // Thêm tiền vào lượt chơi hiện tại
    public void AddGameMoney(float amount)
{
    currentGameMoney += amount;
    // Không cần kiểm tra tiền âm vì yêu cầu cho phép tiền âm
    UpdateMoneyDisplay();
}

// Cập nhật phương thức AddToTotalMoney để xử lý khi kết thúc game
public void AddToTotalMoney()
{
    // Kiểm tra nếu hiện tại đang âm, có thể xử lý theo ý của bạn:
    // Phương án 1: Vẫn cộng vào tổng (cho phép người chơi bị nợ)
    totalMoney += currentGameMoney;
    
    // Phương án 2: Chỉ cộng khi dương, bỏ qua khi âm
    // if (currentGameMoney > 0)
    //     totalMoney += currentGameMoney;
    
    // Reset tiền trong game về 0 sau khi tính toán
    currentGameMoney = 0;
    SaveMoney();
    UpdateMoneyDisplay();
}

    // Reset tiền trong game về 0 (khi bắt đầu lượt chơi mới)
    public void ResetGameMoney()
    {
        currentGameMoney = 0;
        UpdateMoneyDisplay();
    }

    // Phương thức mới: Trừ tiền từ tổng tiền (dùng khi mua level)
    public void DeductFromTotalMoney(float amount)
    {
        if (totalMoney >= amount)
        {
            totalMoney -= amount;
            SaveMoney(); // Lưu tổng tiền mới
            UpdateMoneyDisplay();
        }
    }

    // Lưu tiền vào PlayerPrefs
    private void SaveMoney()
    {
        PlayerPrefs.SetFloat("TotalMoney", totalMoney);
        PlayerPrefs.Save();
    }

    // Tải tiền từ PlayerPrefs
    private void LoadMoney()
    {
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 0);
    }

    public float GetTotalMoney()
    {
        return totalMoney;
    }

    public float GetCurrentGameMoney()
    {
        return currentGameMoney;
    }

public void UpdateMoneyDisplay()
{
    if (moneyMenuText != null)
    {
        moneyMenuText.text = totalMoney.ToString("F0") + " $";
    }
    
    if (moneyInGameText != null)
    {
        // Hiển thị dấu trừ phía trước khi là số âm
        if (currentGameMoney < 0)
            moneyInGameText.text = currentGameMoney.ToString("F0") + " $";
        else
            moneyInGameText.text = currentGameMoney.ToString("F0") + " $";
    }
}
}