using UnityEngine;
using System.Collections;

public class SimpleCollision : MonoBehaviour
{
    public GameObject spawnPrefab; // Prefab của vật thể thứ 3 sẽ được tạo ra
    public string targetTag = ""; // Tag của vật thể thứ 2
    public countDown countdownScript; // Tham chiếu đến script countDown

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            Vector3 spawnPosition = (transform.position + other.transform.position) / 2;
            Debug.Log("cham");

            // Kiểm tra và dừng countdown hiện tại nếu đang chạy
            if (countdownScript != null)
            {
                // Dừng và reset countdown hiện tại
                countdownScript.StopCountdown();
                countdownScript.ResetCountdown();

                // Bắt đầu đếm ngược mới
                Destroy(other.gameObject);
                countdownScript.StartCountdown();
                StartCoroutine(SpawnAfterCountdown(spawnPosition, other.gameObject));
            }
            else
            {
                // Nếu không có script countDown, tạo vật thể ngay lập tức
                SpawnObject(spawnPosition, other.gameObject);
            }
        }
    }

    IEnumerator SpawnAfterCountdown(Vector3 spawnPosition, GameObject otherObject)
    {
        // Chờ cho đến khi đếm ngược kết thúc
        while (countdownScript.currtime > 0)
        {
            yield return null; // Chờ một frame
        }

        // Tạo vật thể mới và ẩn panel đếm ngược
        SpawnObject(spawnPosition, otherObject);
        countdownScript.HideCountdownPanel();
    }

    void SpawnObject(Vector3 spawnPosition, GameObject otherObject)
    {
        if (spawnPrefab != null)
        {
            Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
        }

        Destroy(otherObject);
    }
}