using UnityEngine;

// Gắn script này vào object túi muối đặt trong scene.
// Object cần có Collider với "Is Trigger" = true.
public class SaltBagPickup : MonoBehaviour
{
    public int amount = 1;

    void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddSaltBag(amount);

            Destroy(gameObject);
        }
    }
}