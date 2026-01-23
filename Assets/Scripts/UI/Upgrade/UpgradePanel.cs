using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [Header("컨테이너")]
    public Transform content;  // Content만 연결하면 됨!
    
    void Start()
    {
        RefreshAllUpgrades();
    }
    
    public void RefreshAllUpgrades()
    {
        // Content의 모든 UpgradeItem 찾아서 새로고침
        UpgradeItem[] items = content.GetComponentsInChildren<UpgradeItem>(true);
        
        foreach (UpgradeItem item in items)
        {
            item.Refresh();
        }
    }
}