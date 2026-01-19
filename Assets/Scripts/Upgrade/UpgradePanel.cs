using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("프리팹")]
    public GameObject upgradeItemPrefab;
    
    [Header("컨테이너")]
    public Transform content;
    
    void Start()
    {
        RefreshAllUpgrades();
    }
    
    public void RefreshAllUpgrades()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager is missing!");
            return;
        }
        
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        
        CreateSectionTitle("=== ALLY UPGRADES ===");
        CreateAllyUpgrades();
        
        CreateSectionTitle("=== COMMON UPGRADES ===");
        CreateCommonUpgrades();
        
        CreateSectionTitle("=== ENEMY UPGRADES ===");
        CreateEnemyUpgrades();
    }
    
    void CreateSectionTitle(string titleText)
    {
        GameObject titleObj = new GameObject("SectionTitle");
        titleObj.transform.SetParent(content, false);
        
        // RectTransform 설정 (명시적으로 크기 지정!)
        RectTransform rect = titleObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1000, 50);
        
        // TextMeshPro 추가
        TextMeshProUGUI text = titleObj.AddComponent<TextMeshProUGUI>();
        text.text = titleText;
        text.fontSize = 32;
        text.color = new Color(1f, 0.84f, 0f);
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        
        // Layout Element 추가
        LayoutElement layout = titleObj.AddComponent<LayoutElement>();
        layout.preferredWidth = 1000;
        layout.preferredHeight = 50;
        layout.minHeight = 50;
    }
    
    void CreateAllyUpgrades()
    {
        int warriorAtkLv = GameManager.Instance.warriorAttackLevel;
        if (warriorAtkLv < 10)
        {
            CreateUpgradeItem(
                "Warrior Attack",
                "Lv." + warriorAtkLv + " -> Lv." + (warriorAtkLv + 1),
                "+2 Damage",
                GetAttackUpgradeCost(warriorAtkLv),
                () => {
                    if (GameManager.Instance.UpgradeWarriorAttack("Warrior"))
                    {
                        RefreshAllUpgrades();
                    }
                }
            );
        }
        
        int warriorSpdLv = GameManager.Instance.warriorSpeedLevel;
        if (warriorSpdLv < 10)
        {
            CreateUpgradeItem(
                "Warrior Speed",
                "Lv." + warriorSpdLv + " -> Lv." + (warriorSpdLv + 1),
                "+5 Speed",
                GetSpeedUpgradeCost(warriorSpdLv),
                () => {
                    if (GameManager.Instance.UpgradeWarriorSpeed("Warrior"))
                    {
                        RefreshAllUpgrades();
                    }
                }
            );
        }
    }
    
    void CreateCommonUpgrades()
    {
        int goldLv = GameManager.Instance.goldBonusLevel;
        if (goldLv < 15)
        {
            CreateUpgradeItem(
                "Gold Bonus",
                "Lv." + goldLv + " -> Lv." + (goldLv + 1),
                "+10% Bonus",
                GetGoldBonusCost(goldLv),
                () => {
                    if (GameManager.Instance.UpgradeGoldBonus())
                    {
                        RefreshAllUpgrades();
                    }
                }
            );
        }
    }
    
    void CreateEnemyUpgrades()
    {
        int maxEnemyLv = GameManager.Instance.maxEnemyCountLevel;
        if (maxEnemyLv < 10)
        {
            int currentMax = GameManager.Instance.GetMaxEnemyCount();
            int nextMax = currentMax + 2;
            
            CreateUpgradeItem(
                "Max Enemy Count",
                "Lv." + maxEnemyLv + " -> Lv." + (maxEnemyLv + 1),
                currentMax + " -> " + nextMax + " enemies",
                GetMaxEnemyCost(maxEnemyLv),
                () => {
                    if (GameManager.Instance.UpgradeMaxEnemyCount())
                    {
                        RefreshAllUpgrades();
                    }
                }
            );
        }
        
        int spawnSpeedLv = GameManager.Instance.spawnSpeedLevel;
        if (spawnSpeedLv < 8)
        {
            float currentSpeed = GameManager.Instance.GetSpawnInterval();
            float nextSpeed = Mathf.Max(1.0f, currentSpeed - 0.25f);
            
            CreateUpgradeItem(
                "Spawn Speed",
                "Lv." + spawnSpeedLv + " -> Lv." + (spawnSpeedLv + 1),
                currentSpeed.ToString("F2") + "s -> " + nextSpeed.ToString("F2") + "s",
                GetSpawnSpeedCost(spawnSpeedLv),
                () => {
                    if (GameManager.Instance.UpgradeSpawnSpeed())
                    {
                        RefreshAllUpgrades();
                    }
                }
            );
        }
        
        int killBonusLv = GameManager.Instance.killGoldBonusLevel;
        if (killBonusLv < 10)
        {
            CreateUpgradeItem(
                "Kill Gold Bonus",
                "Lv." + killBonusLv + " -> Lv." + (killBonusLv + 1),
                "+2G per kill",
                GetKillBonusCost(killBonusLv),
                () => {
                    if (GameManager.Instance.UpgradeKillGoldBonus())
                    {
                        RefreshAllUpgrades();
                    }
                }
            );
        }
    }
    
    void CreateUpgradeItem(string title, string levelInfo, string desc, int cost, System.Action onUpgrade)
    {
        GameObject item = Instantiate(upgradeItemPrefab, content);
        
        // 강제로 크기 설정 (안전장치)
        RectTransform rect = item.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(1000, 120);
        }
        
        // Layout Element 확인
        LayoutElement layout = item.GetComponent<LayoutElement>();
        if (layout == null)
        {
            layout = item.AddComponent<LayoutElement>();
        }
        layout.preferredWidth = 1000;
        layout.preferredHeight = 120;
        layout.minHeight = 120;
        
        UpgradeItem itemScript = item.GetComponent<UpgradeItem>();
        itemScript.Setup(title, levelInfo, desc, cost, onUpgrade);
    }
    
    int GetAttackUpgradeCost(int level)
    {
        int[] costs = { 50, 75, 112, 168, 253, 379, 569, 854, 1281, 1922 };
        return level < costs.Length ? costs[level] : 9999;
    }
    
    int GetSpeedUpgradeCost(int level)
    {
        int[] costs = { 40, 56, 78, 109, 153, 215, 301, 421, 590, 826 };
        return level < costs.Length ? costs[level] : 9999;
    }
    
    int GetGoldBonusCost(int level)
    {
        int[] costs = { 100, 160, 256, 409, 655, 1048, 1677, 2684, 4294, 6871, 10995, 17592, 28147, 45035, 72057 };
        return level < costs.Length ? costs[level] : 9999;
    }
    
    int GetMaxEnemyCost(int level)
    {
        int[] costs = { 200, 360, 648, 1166, 2099, 3779, 6802, 12244, 22039, 39671 };
        return level < costs.Length ? costs[level] : 9999;
    }
    
    int GetSpawnSpeedCost(int level)
    {
        int[] costs = { 150, 255, 433, 736, 1252, 2129, 3620, 6155 };
        return level < costs.Length ? costs[level] : 9999;
    }
    
    int GetKillBonusCost(int level)
    {
        int[] costs = { 80, 120, 180, 270, 405, 607, 911, 1366, 2050, 3075 };
        return level < costs.Length ? costs[level] : 9999;
    }
}
