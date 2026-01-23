using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    // 필요하면 ShopItem 배열로 관리
    public ShopItem[] shopItems;
    
    void OnEnable()
    {
        RefreshAll();
    }
    
    public void RefreshAll()
    {
        // 모든 ShopItem 새로고침
        ShopItem[] items = GetComponentsInChildren<ShopItem>();
        foreach (ShopItem item in items)
        {
            if (item != null)
            {
                item.Refresh();
            }
        }
    }
}