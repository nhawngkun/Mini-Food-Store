using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : UICanvas
{
    public Text moneyInGameText; // Text UI để hiển thị tiền trong game hiện tại
    
    void Start()
    {
        // Kết nối Text UI với MoneyManager
        if (moneyInGameText != null && MoneyManager.Instance != null)
        {
            MoneyManager.Instance.moneyInGameText = moneyInGameText;
            MoneyManager.Instance.UpdateMoneyDisplay();
        }
    }
    
    void OnEnable()
    {
        // Cập nhật display khi UI được hiển thị
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.UpdateMoneyDisplay();
        }
    }
}