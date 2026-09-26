using UnityEngine;
using UnityEngine.UI;

// Gắn script này vào 1 object Text (UI > Legacy > Text) trong Canvas.
// Chỉ đọc dữ liệu từ PlayerInventory và hiển thị lên màn hình, không chứa logic gì khác.
public class SaltBagInventoryUI : MonoBehaviour
{
    public PlayerInventory inventory;

    private Text label;

    void Start()
    {
        label = GetComponent<Text>();
    }

    void Update()
    {
        if (inventory == null || label == null)
            return;
        
        label.text = $"Inventory \n" +
            $"Túi muối: {inventory.saltBagCount}";
    }
}