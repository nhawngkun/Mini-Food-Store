using UnityEngine;

public class DraggableFood : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    private bool isInPlaceFood = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Kiểm tra khi chuột nhấp vào đồ ăn
        if (Input.GetMouseButtonDown(0))
        {
            // Kiểm tra xem có nhấp vào đồ ăn này không
            RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // Bắt đầu kéo khi nhấp vào, bất kể đang ở đâu
                isDragging = true;
                isInPlaceFood = false; // Đặt lại trạng thái khi bắt đầu kéo
            }
        }

        // Xử lý khi đang kéo
        if (isDragging)
        {
            // Luôn di chuyển theo vị trí chuột
            transform.position = GetMouseWorldPosition();

            // Xử lý khi nhấn chuột và đang kéo
            if (Input.GetMouseButtonDown(0) && isDragging)
            {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
                foreach (Collider2D collider in hitColliders)
                {
                    // Bỏ qua nếu va chạm với chính nó
                    if (collider.gameObject == gameObject)
                        continue;
                    
                    // Kiểm tra nếu va chạm với thùng rác
                    if (collider.CompareTag("Trash"))
                    {
                        Destroy(gameObject);
                        return;
                    }
                    
                    // Kiểm tra kết hợp với thức ăn khác
                    DraggableFood otherFood = collider.GetComponent<DraggableFood>();
                    if (otherFood != null && !otherFood.IsDragging())
                    {
                        FoodCombiner combiner = FindObjectOfType<FoodCombiner>();
                        if (combiner != null)
                        {
                            combiner.CheckAndCombine(gameObject, collider.gameObject);
                            return;
                        }
                    }
                }
            }

            // Khi thả chuột
            if (Input.GetMouseButtonUp(0))
            {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
                GameObject placeFoodObject = null;
                bool canPlaceFood = true;
                
                foreach (Collider2D collider in hitColliders)
                {
                    // Tìm vùng PlaceFood
                    if (collider.CompareTag("PlaceFood"))
                    {
                        placeFoodObject = collider.gameObject;
                        
                        // Kiểm tra xem vùng PlaceFood đã có đồ ăn chưa
                        Collider2D[] foodColliders = Physics2D.OverlapCircleAll(placeFoodObject.transform.position, 0.1f);
                        foreach (Collider2D foodCollider in foodColliders)
                        {
                            // Nếu tìm thấy đồ ăn khác (không phải đồ ăn đang kéo) tại vị trí này
                            DraggableFood otherFood = foodCollider.GetComponent<DraggableFood>();
                            if (otherFood != null && otherFood.gameObject != gameObject && otherFood.IsInPlaceFood())
                            {
                                // Kiểm tra xem có thể kết hợp không
                                FoodCombiner combiner = FindObjectOfType<FoodCombiner>();
                                if (combiner != null && combiner.FindMatchingResult(gameObject.tag, otherFood.gameObject.tag) != null)
                                {
                                    // Có thể kết hợp, thực hiện kết hợp
                                    combiner.CheckAndCombine(gameObject, otherFood.gameObject);
                                    return;
                                }
                                else
                                {
                                    // Không thể kết hợp, đánh dấu không thể đặt
                                    canPlaceFood = false;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                
                // Nếu thả trong vùng PlaceFood và có thể đặt
                if (placeFoodObject != null && canPlaceFood)
                {
                    // Đặt đồ ăn vào vị trí cố định của PlaceFood
                    transform.position = placeFoodObject.transform.position;
                    isDragging = false;
                    isInPlaceFood = true;
                }
                else
                {
                    // Nếu không đặt được (vị trí không hợp lệ hoặc đã có đồ ăn), tiếp tục dính theo chuột
                    isDragging = true;
                }
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    public bool IsDragging()
    {
        return isDragging;
    }
    
    public bool IsInPlaceFood()
    {
        return isInPlaceFood;
    }
}