
using System.Collections.Generic;
using UnityEngine;

public class FoodCombiner : MonoBehaviour
{
    [System.Serializable]
    public class FoodResult
    {
        public string food2Tag;
        public GameObject resultPrefab;
    }

    [System.Serializable]
    public class FoodCombination
    {
        public string food1Tag;
        public List<FoodResult> possibleResults = new List<FoodResult>();
    }

    public List<FoodCombination> foodCombinations = new List<FoodCombination>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là thức ăn có thể kéo không
        DraggableFood draggableFood = collision.GetComponent<DraggableFood>();
        
        if (draggableFood != null && draggableFood.IsDragging())
        {
            // Lấy thông tin về 2 đối tượng đang va chạm
            GameObject food1 = gameObject;
            GameObject food2 = collision.gameObject;
            
            // Tìm kiếm kết quả phù hợp
            GameObject resultPrefab = FindMatchingResult(food1.tag, food2.tag);
            
            if (resultPrefab != null)
            {
                // Tạo ra đối tượng mới tại vị trí giữa 2 đối tượng va chạm
                Vector3 spawnPosition = (food1.transform.position + food2.transform.position) / 2f;
                GameObject resultFood = Instantiate(resultPrefab, spawnPosition, Quaternion.identity);
                
                // Xóa 2 đối tượng cũ
                Destroy(food1);
                Destroy(food2);
            }
        }
    }
    
    public GameObject FindMatchingResult(string tag1, string tag2)
    {
        // Tìm trong cả 2 chiều
        GameObject result = TryFindResult(tag1, tag2);
        if (result == null)
        {
            result = TryFindResult(tag2, tag1);
        }
        return result;
    }
    
    private GameObject TryFindResult(string food1Tag, string food2Tag)
    {
        // Tìm food1Tag trong danh sách combinations
        foreach (FoodCombination combination in foodCombinations)
        {
            if (combination.food1Tag == food1Tag)
            {
                // Tìm food2Tag trong danh sách possibleResults
                foreach (FoodResult result in combination.possibleResults)
                {
                    if (result.food2Tag == food2Tag)
                    {
                        return result.resultPrefab;
                    }
                }
            }
        }
        return null;
    }
    
    // Phương thức để kiểm tra và kết hợp thức ăn (sử dụng cho hệ thống kéo thả)
    public void CheckAndCombine(GameObject food1, GameObject food2)
    {
        GameObject resultPrefab = FindMatchingResult(food1.tag, food2.tag);
        
        if (resultPrefab != null)
        {
            // Tạo ra đối tượng mới tại vị trí giữa 2 đối tượng
            Vector3 spawnPosition = (food1.transform.position + food2.transform.position) / 2f;
            GameObject resultFood = Instantiate(resultPrefab, spawnPosition, Quaternion.identity);
            
            // Xóa 2 đối tượng cũ
            Destroy(food1);
            Destroy(food2);
        }
    }
}