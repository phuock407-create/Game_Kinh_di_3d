using UnityEngine;
using UnityEngine.InputSystem;

// Gắn script này vào object Player.
// Yêu cầu package "Input System" (Active Input Handling đang để chế độ mới).
public class SaltBagThrower : MonoBehaviour
{
    public PlayerInventory inventory;

    // Prefab viên túi muối sẽ bay ra (cần có Rigidbody + Collider + script SaltBagProjectile)
    public GameObject saltBagProjectilePrefab;

    // Vị trí xuất phát của túi muối khi ném (ví dụ 1 object rỗng đặt trước mặt player)
    public Transform throwPoint;

    // Camera để lấy hướng ném. Nếu để trống, script tự lấy Camera.main lúc Start.
    public Transform cameraTransform;

    // Khoảng cách phía trước camera để spawn viên đạn, tránh việc nó xuất hiện
    // ngay trong người player rồi va chạm luôn khi hướng ném lệch với ThrowPoint.
    public float spawnDistanceFromCamera = 1f;

    public float throwForce = 15f;

    // Phím ném, đổi trong Inspector nếu muốn (danh sách đầy đủ xem enum UnityEngine.InputSystem.Key)
    public Key throwKey = Key.G;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current[throwKey].wasPressedThisFrame)
        {
            TryThrow();
        }
    }

    void TryThrow()
    {
        if (inventory == null || saltBagProjectilePrefab == null || throwPoint == null)
        {
            Debug.LogWarning("SaltBagThrower thiếu tham chiếu (inventory/prefab/throwPoint)!");
            return;
        }

        if (!inventory.UseSaltBag())
        {
            Debug.Log("Hết túi muối, không thể ném!");
            return;
        }

        // Hướng ném theo hướng camera đang nhìn (nếu có gán camera), không thì dùng throwPoint.forward
        Vector3 throwDirection =
            cameraTransform != null ? cameraTransform.forward : throwPoint.forward;

        // Xuất phát từ ThrowPoint (vị trí tay) - dùng Physics.IgnoreCollision bên dưới
        // để tránh việc nó tự đụng ngay vào người player khi hướng bay lệch với vị trí tay.
        GameObject projectile = Instantiate(
            saltBagProjectilePrefab,
            throwPoint.position,
            Quaternion.LookRotation(throwDirection)
        );

        // Bỏ qua va chạm giữa viên đạn vừa ra đời với chính Player (và mọi Collider con của Player,
        // ví dụ Collider trên tay), tránh trường hợp nó va ngay lúc vừa sinh ra và tự huỷ liền lập tức.
        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider[] playerColliders = GetComponentsInChildren<Collider>();

        if (projectileCollider != null)
        {
            foreach (Collider col in playerColliders)
            {
                Physics.IgnoreCollision(projectileCollider, col);
            }
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogWarning("Prefab túi muối không có Rigidbody nên sẽ không bay!");
        }

        Debug.Log($"Đã ném túi muối, còn lại: {inventory.saltBagCount}");
    }
}