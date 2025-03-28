using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    public static CoinEffect Instance { get; private set; }
    
    [Header("Coin Settings")]
    public GameObject coinPrefab; // Cần gán prefab của đồng xu trong Inspector
    public Transform targetPosition; // Vị trí đích để bay tới
    public int numberOfCoins = 5; // Số lượng đồng xu
    public float coinSpeed = 5f; // Tốc độ di chuyển của đồng xu
    public float spreadRadius = 1f; // Bán kính lan tỏa ban đầu
    public float delayBetweenCoins = 0.1f; // Thời gian giữa các đồng xu
    public float scaleTime = 0.5f; // Thời gian để phóng to/thu nhỏ đồng xu
    public float rotationSpeed = 500f; // Tốc độ xoay của đồng xu
    
    [Header("Animation Curves")]
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Đường cong di chuyển
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Đường cong phóng to/thu nhỏ
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Kiểm tra component cần thiết
        if (coinPrefab == null)
        {
            Debug.LogError("Coin prefab is not assigned to CoinEffect!");
        }
        
        if (targetPosition == null)
        {
            Debug.LogWarning("Target position is not assigned. Will use random positions.");
        }
    }
    
    // Phương thức để tạo hiệu ứng đồng xu bay từ vị trí khách hàng
    public void PlayCoinEffect(Vector3 startPosition, float amount, System.Action onComplete = null)
    {
        StartCoroutine(SpawnCoins(startPosition, amount, onComplete));
    }
    
    private IEnumerator SpawnCoins(Vector3 startPosition, float amount, System.Action onComplete)
    {
        Vector3 targetPos = targetPosition != null ? targetPosition.position : new Vector3(Random.Range(-5f, 5f), Random.Range(2f, 5f), 0);
        
        int coinsToSpawn = Mathf.Min(numberOfCoins, Mathf.CeilToInt(amount / 10)); // Số lượng đồng xu dựa trên số tiền
        
        for (int i = 0; i < coinsToSpawn; i++)
        {
            // Tạo vị trí bắt đầu ngẫu nhiên xung quanh khách hàng
            Vector3 randomOffset = Random.insideUnitCircle * spreadRadius;
            Vector3 coinStartPos = startPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // Tạo đồng xu mới
            GameObject coin = Instantiate(coinPrefab, coinStartPos, Quaternion.identity);
            
            // Bắt đầu animation bay
            StartCoroutine(AnimateCoin(coin, coinStartPos, targetPos));
            
            // Chờ một chút trước khi tạo đồng xu tiếp theo
            yield return new WaitForSeconds(delayBetweenCoins);
        }
        
        // Chờ cho đồng xu cuối cùng bay xong
        yield return new WaitForSeconds(1.5f); // Thời gian bay + biến mất
        
        // Gọi callback khi hoàn tất
        onComplete?.Invoke();
    }
    
    private IEnumerator AnimateCoin(GameObject coin, Vector3 startPos, Vector3 endPos)
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startPos, endPos);
        float duration = journeyLength / coinSpeed;
        
        // Hiệu ứng phóng to ban đầu
        StartCoroutine(ScaleCoin(coin, 0f, 1f, scaleTime));
        
        // Di chuyển đồng xu theo đường cong
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed = Time.time - startTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            
            // Áp dụng đường cong chuyển động
            float curveValue = movementCurve.Evaluate(normalizedTime);
            
            // Tính toán vị trí hiện tại
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, curveValue);
            
            // Thêm dao động nhỏ theo đường sin để tạo cảm giác tự nhiên
            float sineWave = Mathf.Sin(normalizedTime * Mathf.PI * 2 * 2) * 0.5f * (1 - normalizedTime);
            currentPos += new Vector3(sineWave, sineWave, 0);
            
            // Cập nhật vị trí
            coin.transform.position = currentPos;
            
            // Xoay đồng xu
            coin.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            yield return null;
        }
        
        // Đảm bảo đồng xu đến đúng vị trí cuối
        coin.transform.position = endPos;
        
        // Hiệu ứng thu nhỏ và biến mất
        StartCoroutine(ScaleCoin(coin, 1f, 0f, scaleTime, () => {
            Destroy(coin);
        }));
    }
    
    private IEnumerator ScaleCoin(GameObject coin, float startScale, float endScale, float duration, System.Action onComplete = null)
    {
        float startTime = Time.time;
        Vector3 originalScale = coin.transform.localScale;
        Vector3 targetScale = originalScale * endScale;
        
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed = Time.time - startTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            
            // Áp dụng đường cong phóng to/thu nhỏ
            float curveValue = scaleCurve.Evaluate(normalizedTime);
            
            // Tính toán và cập nhật tỷ lệ
            Vector3 newScale = Vector3.Lerp(originalScale * startScale, targetScale, curveValue);
            coin.transform.localScale = newScale;
            
            yield return null;
        }
        
        // Đảm bảo đồng xu có đúng tỷ lệ cuối cùng
        coin.transform.localScale = targetScale;
        
        // Gọi callback khi hoàn tất
        onComplete?.Invoke();
    }
}