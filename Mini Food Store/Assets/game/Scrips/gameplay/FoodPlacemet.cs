using UnityEngine;

public class FoodPlacement : MonoBehaviour
{
    public Transform Food;
    // Thêm tag "PlaceFood" vào GameObject này trong Inspector
    
    void Start()
    {
        // Kiểm tra xem GameObject có tag "PlaceFood" chưa
        if (!gameObject.CompareTag("PlaceFood"))
        {
            Debug.LogWarning("GameObject " + gameObject.name + " cần được gán tag 'PlaceFood' để hoạt động đúng.");
        }
    }
}