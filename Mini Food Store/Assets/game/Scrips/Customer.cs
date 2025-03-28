using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FoodImage
{
    public string foodTag;
    public Image image;
    public bool isMatched = false;
    public bool isLeftSide;
}

[System.Serializable]
public class EmotionImage
{
    public Image image;
    public bool isHappy;
}

public class Customer : MonoBehaviour
{
    public countDown timerController;

    public List<FoodImage> foodImages = new List<FoodImage>();
    public int maxImagesShown = 2;
    private List<FoodImage> activeImages = new List<FoodImage>();

    [Header("Emotion Images")]
    public List<EmotionImage> emotionImages = new List<EmotionImage>();
    public float emotionDisplayTime = 2f;
    private Coroutine currentEmotionCoroutine = null;

    public Transform orderArea1;
    public Transform orderArea2;
    public Transform waitArea;
    public Transform exitArea;
    public float moveSpeed = 2f;

    private bool isMovingToOrder1 = false;
    private bool isMovingToOrder2 = false;
    private bool isMovingToWaitArea = false;
    private bool isMovingToExit = false;
    private bool isWaiting = false;
    private bool hasOrdered = false;
    private bool hasCompleted = false;
    public Image imgchat;
    private bool timedOut = false;

    private static bool isOrderArea1Occupied = false;
    private static bool isOrderArea2Occupied = false;
    private bool isUsingOrderArea1 = false;
    private bool isUsingOrderArea2 = false;

    private static List<Customer> waitingCustomers = new List<Customer>();

    private bool leftImageDisplayed = false;
    private bool rightImageDisplayed = false;

    [Header("Di chuyển ngẫu nhiên trong khu vực chờ")]
    public float waitAreaRadius = 2f;
    public float minWanderTime = 2f;
    public float maxWanderTime = 5f;
    private Vector2 randomWaitPosition;
    private bool isWandering = false;
    private float nextWanderTime = 0f;

    public Text moneyText; // Text UI để hiển thị tiền
    private float moneyEarned = 0; // Số tiền kiếm được
    private static float totalMoneyEarned = 0; // Tổng số tiền kiếm được từ tất cả khách hàng

    void Start()
    {
        imgchat.enabled = false;

        if (timerController == null)
        {
            timerController = FindObjectOfType<countDown>();
            if (timerController == null)
            {
                Debug.LogWarning("No countdown timer found! Please assign the timer in the inspector.");
            }
            else
            {
                timerController.SetCustomer(this);
            }
        }
        else
        {
            timerController.SetCustomer(this);
        }

        foreach (FoodImage foodImage in foodImages)
        {
            if (foodImage.image != null)
            {
                foodImage.image.enabled = false;
            }
        }

        foreach (EmotionImage emotionImage in emotionImages)
        {
            if (emotionImage.image != null)
            {
                emotionImage.image.enabled = false;
            }
        }

        // Hiển thị tổng tiền hiện tại khi khách hàng mới xuất hiện
        if (moneyText != null)
        {
            UpdateMoneyDisplay();
        }

        ChooseOrderArea();
    }

    private void ShowEmotion(bool isHappy)
    {
        if (currentEmotionCoroutine != null)
        {
            StopCoroutine(currentEmotionCoroutine);
        }

        foreach (EmotionImage emotionImage in emotionImages)
        {
            emotionImage.image.enabled = false;
        }

        List<EmotionImage> matchingEmotions = emotionImages.FindAll(e => e.isHappy == isHappy);

        if (matchingEmotions.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingEmotions.Count);
            EmotionImage selectedEmotion = matchingEmotions[randomIndex];

            selectedEmotion.image.enabled = true;

            currentEmotionCoroutine = StartCoroutine(HideEmotionAfterDelay(selectedEmotion, emotionDisplayTime));
        }
    }

    private IEnumerator HideEmotionAfterDelay(EmotionImage emotionImage, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (emotionImage.image != null)
        {
            emotionImage.image.enabled = false;
        }
        currentEmotionCoroutine = null;
    }

    public void HandleTimeout()
{
    if ((isUsingOrderArea1 || isUsingOrderArea2) && !hasCompleted && !isMovingToExit)
    {
        Debug.Log("Time ran out! Customer is leaving.");
        timedOut = true;

        ShowEmotion(false);

        HideAllFoodImages();

        imgchat.enabled = false;

        // Trừ tiền khi thời gian hết và chưa phục vụ xong
        DeductMoneyForTimeout();

        StartMovingToExitArea();
    }
}

// Phương thức mới để trừ tiền khi timeout
private void DeductMoneyForTimeout()
{
    // Mức phạt cố định khi không phục vụ được khách hàng
    float penaltyAmount = -20f; // Số âm để biểu thị việc bị trừ tiền
    
    // Sử dụng MoneyManager để trừ tiền trong game
    if (MoneyManager.Instance != null)
    {
        MoneyManager.Instance.AddGameMoney(penaltyAmount);
        Debug.Log("Bị phạt $" + (-penaltyAmount) + " vì không phục vụ kịp thời!");
    }
    else
    {
        Debug.LogWarning("MoneyManager instance not found!");
    }
}

    private void HideAllFoodImages()
    {
        foreach (FoodImage foodImage in foodImages)
        {
            if (foodImage.image != null)
            {
                foodImage.image.enabled = false;
            }
        }
        activeImages.Clear();
    }

    void ChooseOrderArea()
    {
        if (!isOrderArea1Occupied && orderArea1 != null)
        {
            isOrderArea1Occupied = true;
            isUsingOrderArea1 = true;
            StartMovingToOrderArea1();
        }
        else if (!isOrderArea2Occupied && orderArea2 != null)
        {
            isOrderArea2Occupied = true;
            isUsingOrderArea2 = true;
            StartMovingToOrderArea2();
        }
        else
        {
            StartMovingToWaitArea();
        }
    }

    void Update()
    {
        if (isMovingToOrder1 && orderArea1 != null)
        {
            MoveTowards(orderArea1.position);

            if (Vector2.Distance(transform.position, orderArea1.position) < 0.1f)
            {
                isMovingToOrder1 = false;
                hasOrdered = true;

                ShowRandomImages();

                if (timerController != null)
                {
                    timerController.StartCountdown();
                }
            }
        }

        if (isMovingToOrder2 && orderArea2 != null)
        {
            MoveTowards(orderArea2.position);

            if (Vector2.Distance(transform.position, orderArea2.position) < 0.1f)
            {
                isMovingToOrder2 = false;
                hasOrdered = true;

                ShowRandomImages();

                if (timerController != null)
                {
                    timerController.StartCountdown();
                }
            }
        }

        if (isMovingToWaitArea && waitArea != null)
        {
            MoveTowards(waitArea.position);

            if (Vector2.Distance(transform.position, waitArea.position) < 0.1f)
            {
                isMovingToWaitArea = false;
                isWaiting = true;

                if (!waitingCustomers.Contains(this))
                {
                    waitingCustomers.Add(this);
                }

                StartCoroutine(CheckForAvailableOrderArea());

                PickNewWanderDestination();
            }
        }

        if (isWaiting && !isWandering)
        {
            if (Time.time >= nextWanderTime)
            {
                PickNewWanderDestination();
            }
        }

        if (isWandering && isWaiting)
        {
            MoveTowards(randomWaitPosition);

            if (Vector2.Distance(transform.position, randomWaitPosition) < 0.1f)
            {
                isWandering = false;

                nextWanderTime = Time.time + Random.Range(minWanderTime, maxWanderTime);
            }
        }

        if (isMovingToExit && exitArea != null)
        {
            MoveTowards(exitArea.position);

            if (Vector2.Distance(transform.position, exitArea.position) < 0.1f)
            {
                isMovingToExit = false;

                if (timerController != null)
                {
                    timerController.StopCountdown();
                    timerController.ResetCountdown();
                }

                if (isUsingOrderArea1)
                {
                    isOrderArea1Occupied = false;
                    isUsingOrderArea1 = false;

                    CheckWaitingCustomers();
                }
                if (isUsingOrderArea2)
                {
                    isOrderArea2Occupied = false;
                    isUsingOrderArea2 = false;

                    CheckWaitingCustomers();
                }

                Destroy(gameObject);
            }
        }
    }

    void PickNewWanderDestination()
    {
        if (waitArea != null)
        {
            Vector2 randomOffset = Random.insideUnitCircle * waitAreaRadius;
            randomWaitPosition = (Vector2)waitArea.position + randomOffset;

            isWandering = true;
        }
    }

    static void CheckWaitingCustomers()
    {
        if (waitingCustomers.Count > 0)
        {
            Customer nextCustomer = waitingCustomers[0];

            waitingCustomers.RemoveAt(0);

            if (nextCustomer != null && nextCustomer.isWaiting)
            {
                nextCustomer.isWaiting = false;
                nextCustomer.isWandering = false;
                nextCustomer.ChooseOrderArea();
            }
        }
    }

    IEnumerator CheckForAvailableOrderArea()
    {
        while (isWaiting)
        {
            yield return new WaitForSeconds(0.5f);

            if (waitingCustomers.Count > 0 && waitingCustomers[0] == this)
            {
                if (!isOrderArea1Occupied && orderArea1 != null)
                {
                    waitingCustomers.RemoveAt(0);
                    isWaiting = false;
                    isWandering = false;
                    isOrderArea1Occupied = true;
                    isUsingOrderArea1 = true;
                    StartMovingToOrderArea1();
                    break;
                }
                else if (!isOrderArea2Occupied && orderArea2 != null)
                {
                    waitingCustomers.RemoveAt(0);
                    isWaiting = false;
                    isWandering = false;
                    isOrderArea2Occupied = true;
                    isUsingOrderArea2 = true;
                    StartMovingToOrderArea2();
                    break;
                }
            }
        }
    }

    void MoveTowards(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    void StartMovingToOrderArea1()
    {
        if (orderArea1 != null)
        {
            isMovingToOrder1 = true;
            isMovingToOrder2 = false;
            isMovingToWaitArea = false;
            isMovingToExit = false;
        }
        else
        {
            Debug.LogError("Chưa được gán khu vực đặt món 1 (orderArea1)!");
        }
    }

    void StartMovingToOrderArea2()
    {
        if (orderArea2 != null)
        {
            isMovingToOrder1 = false;
            isMovingToOrder2 = true;
            isMovingToWaitArea = false;
            isMovingToExit = false;
        }
        else
        {
            StartMovingToWaitArea();
        }
    }

    void StartMovingToWaitArea()
    {
        if (waitArea != null)
        {
            isMovingToOrder1 = false;
            isMovingToOrder2 = false;
            isMovingToWaitArea = true;
            isMovingToExit = false;
        }
        else
        {
            Debug.LogError("Chưa được gán khu vực chờ (waitArea)!");
        }
    }

    void StartMovingToExitArea()
    {
        if (exitArea != null)
        {
            if (timerController != null)
            {
                timerController.StopCountdown();
                timerController.ResetCountdown();
            }

            isMovingToOrder1 = false;
            isMovingToOrder2 = false;
            isMovingToWaitArea = false;
            isMovingToExit = true;
        }
        else
        {
            Debug.LogError("Chưa được gán khu vực thoát (exitArea)!");
        }
    }
    

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasCompleted || timedOut)
            return;

        if (hasOrdered && !collision.CompareTag("oder"))
        {
            string foodTag = collision.tag;

            bool foodMatched = false;
            foreach (FoodImage foodImage in activeImages)
            {
                if (foodImage.foodTag == foodTag && !foodImage.isMatched)
                {
                    foodMatched = true;
                    break;
                }
            }

            if (foodMatched)
            {
                ShowEmotion(true);
            }
            else
            {
                ShowEmotion(false);
            }

            CheckFoodMatch(foodTag, collision.gameObject);
        }

        if (collision.CompareTag("oder"))
        {
            imgchat.enabled = true;
        }
    }
    

    void ResetImages()
    {
        foreach (FoodImage foodImage in foodImages)
        {
            foodImage.image.enabled = false;
            foodImage.isMatched = false;
        }
        activeImages.Clear();

        leftImageDisplayed = false;
        rightImageDisplayed = false;
    }

    void ShowRandomImages()
    {
        ResetImages();

        if (foodImages.Count == 0)
        {
            Debug.LogWarning("Không có hình ảnh đồ ăn nào trong danh sách!");
            return;
        }

        List<FoodImage> leftImages = new List<FoodImage>();
        List<FoodImage> rightImages = new List<FoodImage>();

        foreach (FoodImage foodImage in foodImages)
        {
            if (foodImage.isLeftSide)
            {
                leftImages.Add(foodImage);
            }
            else
            {
                rightImages.Add(foodImage);
            }
        }

        int imagesToShow = Random.Range(1, Mathf.Min(maxImagesShown + 1, 3));

        if (imagesToShow == 2 && leftImages.Count > 0 && rightImages.Count > 0)
        {
            int leftIndex = Random.Range(0, leftImages.Count);
            FoodImage leftImage = leftImages[leftIndex];
            leftImage.image.enabled = true;
            activeImages.Add(leftImage);
            leftImageDisplayed = true;

            int rightIndex = Random.Range(0, rightImages.Count);
            FoodImage rightImage = rightImages[rightIndex];
            rightImage.image.enabled = true;
            activeImages.Add(rightImage);
            rightImageDisplayed = true;
        }
        else
        {
            bool showLeftSide = (leftImages.Count > 0) && (rightImages.Count == 0 || Random.Range(0, 2) == 0);

            if (showLeftSide && leftImages.Count > 0)
            {
                int index = Random.Range(0, leftImages.Count);
                FoodImage selectedImage = leftImages[index];
                selectedImage.image.enabled = true;
                activeImages.Add(selectedImage);
                leftImageDisplayed = true;
            }
            else if (rightImages.Count > 0)
            {
                int index = Random.Range(0, rightImages.Count);
                FoodImage selectedImage = rightImages[index];
                selectedImage.image.enabled = true;
                activeImages.Add(selectedImage);
                rightImageDisplayed = true;
            }
        }
    }

    void CheckFoodMatch(string foodTag, GameObject foodObject)
    {
        foreach (FoodImage foodImage in activeImages)
        {
            if (foodImage.foodTag == foodTag && !foodImage.isMatched)
            {
                foodImage.image.enabled = false;
                foodImage.isMatched = true;

                if (foodImage.isLeftSide)
                {
                    leftImageDisplayed = false;
                }
                else
                {
                    rightImageDisplayed = false;
                }

                Destroy(foodObject);

                CheckAllMatched();

                break;
            }
        }
    }

    void CheckAllMatched()
{
    bool allMatched = true;
    foreach (FoodImage foodImage in activeImages)
    {
        if (!foodImage.isMatched)
        {
            allMatched = false;
            break;
        }
    }

    if (allMatched && activeImages.Count > 0)
    {
        Debug.Log("Tất cả đồ ăn đã được phục vụ!");
        imgchat.enabled = false;

        ShowEmotion(true);

        if (timerController != null)
        {
            timerController.StopCountdown();
        }
        
        // Tạm tính tiền nhưng chưa thêm vào tổng tiền
       
        if (timerController != null)
        {
            float remainingTime = timerController.currtime;
            timerController.StopCountdown();
            
            // Tạm tính tiền nhưng chưa thêm vào tổng tiền
            float earnedAmount = remainingTime * 10;
            
            // Hiệu ứng đồng xu bay with the correct time
            PlayCoinEffect(earnedAmount);
        }
        
        // Hiệu ứng đồng xu bay
       

        hasCompleted = true;

        // Giải phóng khu vực order ngay lập tức để khách đang chờ có thể đến
        if (isUsingOrderArea1)
        {
           
            isOrderArea1Occupied = false;
            isUsingOrderArea1 = false;

            // Kiểm tra khách hàng đang chờ
            CheckWaitingCustomers();
        }
        if (isUsingOrderArea2)
        {
           
            isOrderArea2Occupied = false;
            isUsingOrderArea2 = false;

            // Kiểm tra khách hàng đang chờ
            CheckWaitingCustomers();
        }

        StartCoroutine(ExitAfterShowingEmotion());
    }
}

void PlayCoinEffect(float amount)
{
    if (CoinEffect.Instance != null)
    {
        // Gọi hiệu ứng coins bay và đợi hoàn thành rồi mới tính tiền
        CoinEffect.Instance.PlayCoinEffect(transform.position, amount, () => {
            // Callback này sẽ được gọi khi hiệu ứng hoàn tất
            // Lúc này mới thêm tiền vào tổng, use the amount directly
            CalculateMoney(amount / 10); // Convert back to time value
        });
    }
    else
    {
        // Nếu không tìm thấy CoinEffect thì tính tiền ngay
        CalculateMoney(amount / 10); // Convert back to time value
    }
}

    // Cập nhật phương thức tính tiền để tích lũy tổng tiền
 void CalculateMoney(float remainingTime)
{
    Debug.Log(remainingTime);
    // Tính tiền cho khách hàng hiện tại
    moneyEarned = remainingTime * 10;
    
    // Cộng vào tiền trong game, không phải tổng tiền
    if (MoneyManager.Instance != null)
    {
        MoneyManager.Instance.AddGameMoney(moneyEarned);
    }
    else
    {
        Debug.LogWarning("MoneyManager instance not found!");
        totalMoneyEarned += moneyEarned; // Fallback to old method
    }

    Debug.Log("Khách hàng kiếm được $" + moneyEarned);
}
    // Phương thức mới để cập nhật hiển thị tiền
    void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = totalMoneyEarned.ToString("F0") + " $";
            Debug.Log(" | Tổng tiền hiện tại: $" + totalMoneyEarned);
    
        }
    }

    IEnumerator ExitAfterShowingEmotion()
    {
        yield return new WaitForSeconds(emotionDisplayTime);
        StartMovingToExitArea();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("waitoder") && isWaiting)
        {
        }
    }

    void OnDestroy()
    {
        if (timerController != null && (isUsingOrderArea1 || isUsingOrderArea2))
        {
            timerController.StopCountdown();
            timerController.ResetCountdown();
        }

        // Chỉ giải phóng khu vực order nếu vẫn đang sử dụng
        if (isUsingOrderArea1)
        {
            isOrderArea1Occupied = false;
            CheckWaitingCustomers();
        }
        if (isUsingOrderArea2)
        {
            isOrderArea2Occupied = false;
            CheckWaitingCustomers();
        }

        if (waitingCustomers.Contains(this))
        {
            waitingCustomers.Remove(this);
        }
    }
}