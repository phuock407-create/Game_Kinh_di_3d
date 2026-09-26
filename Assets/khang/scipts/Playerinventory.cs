using UnityEngine;

// Gắn script này vào object Player.
// Chỉ lưu số lượng túi muối, không biết gì về việc nhặt hay ném.
public class PlayerInventory : MonoBehaviour
{
    public int saltBagCount = 0;

    public void AddSaltBag(int amount = 1)
    {
        saltBagCount += amount;
        Debug.Log($"Nhặt túi muối! Hiện có: {saltBagCount}");
    }

    // Trả về true nếu dùng được (còn hàng), false nếu hết
    public bool UseSaltBag()
    {
        if (saltBagCount <= 0)
            return false;

        saltBagCount--;
        return true;
    }
}