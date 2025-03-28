using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    private Camera mainCamera;
    private GameObject currentFood;
    private bool isSpawning = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Nếu đang ở chế độ spawn và có đồ ăn đang được tạo ra, nó sẽ di chuyển theo chuột
        if (isSpawning && currentFood != null)
        {
            currentFood.transform.position = GetMouseWorldPosition();

            // Kiểm tra nếu nhấn chuột trái
            if (Input.GetMouseButtonDown(0))
            {
                // Kiểm tra xem có đang va chạm với đối tượng nào có tag "PlaceFood" không
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(currentFood.transform.position, 0.5f);
                foreach (Collider2D collider in hitColliders)
                {
                    if (collider.CompareTag("PlaceFood"))
                    {
                        // Đặt đồ ăn vào vị trí hiện tại và ngừng spawn mode
                        isSpawning = false;
                        currentFood = null;
                        return;
                    }
                    
                    if (collider.CompareTag("Trash"))
                    {
                        // Hủy đồ ăn nếu va chạm với thùng rác
                        Destroy(currentFood);
                        currentFood = null;
                        isSpawning = false;
                        return;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // Kiểm tra nếu người dùng nhấn chuột lên spawner
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // Tạo đồ ăn mới và chuyển sang chế độ spawn
                Vector3 spawnPosition = GetMouseWorldPosition();
                currentFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
                isSpawning = true;
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}