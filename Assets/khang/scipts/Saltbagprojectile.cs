using UnityEngine;

// Gắn script này vào prefab viên túi muối bay ra.
// Prefab cần có Rigidbody và Collider (không tick Is Trigger, để va chạm vật lý thật).
public class SaltBagProjectile : MonoBehaviour
{
    // Tự huỷ sau chừng này giây nếu không trúng gì, tránh rác trong scene
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("NÉM TRÚNG ENEMY!");

            // TODO: chỗ này sau này có thể thêm hiệu ứng, gây choáng,
            // hoặc tạo thêm 1 điểm gây tiếng động để enemy khác chú ý tới.

            Destroy(gameObject);
        }

        // Va chạm với vật khác (bàn, sàn, tường...) thì để nó rơi/nằm lại
        // bình thường theo vật lý, KHÔNG huỷ ngay - chỉ huỷ khi hết lifeTime.
    }
}