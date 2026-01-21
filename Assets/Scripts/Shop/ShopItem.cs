using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Header("UI 참조")]
    public Image unitIcon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;      // 유닛 설명
    public TextMeshProUGUI countText;      // "현재 보유: X명"

    public Button upgradeButton;
    public TextMeshProUGUI priceText;     // "500G"
    
    public enum Unit { Warrior, Archer, Monk, Knight, Mage }
    
    [Header("설정")]
    public Unit unitType;
    public Sprite unitSprite;
    public string unitName;
    public string unitDesc;
    public int purchasePrice;  // 구매 가격 (unlockCost 대신)
    
    void Start()
    {
        Setup();
    }
    
    void Setup()
    {
        if (unitIcon != null && unitSprite != null)
        {
            unitIcon.sprite = unitSprite;
        }
        
        if (titleText != null)
        {
            titleText.text = unitName;
        }

        if (descText != null)
        {
            descText.text = unitDesc;
        }
        
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnPurchaseClick);
        }


        
        Refresh();
    }
    
    void Update()
    {
        Refresh();
    }
    
    public void Refresh()
    {
        if (GameManager.Instance == null) return;
        
        // 현재 보유 수 표시
        int currentCount = GetCurrentCount();
        if (countText != null)
        {
            countText.text = $"현재 보유: {currentCount}명";
        }
        
        // 구매 가능 여부
        bool canAfford = GameManager.Instance.currentGold >= purchasePrice;
        
        if (upgradeButton != null)
        {
            upgradeButton.interactable = canAfford;
            
            Image btnImage = upgradeButton.GetComponent<Image>();
            if (btnImage != null)
            {
                if (canAfford)
                {
                    // 살 수 있음 - 밝은 초록
                    btnImage.color = new Color(0.2f, 0.8f, 0.2f);
                }
                else
                {
                    // 돈 부족 - 어두운 회색
                    btnImage.color = new Color(0.3f, 0.3f, 0.3f);
                }
            }
        }
        
        if (priceText != null)
        {
            priceText.text = $"{purchasePrice}G";
            priceText.color = canAfford ? Color.white : Color.red;
        }
    }
    
    int GetCurrentCount()
    {
        switch (unitType)
        {
            case Unit.Warrior:
                return GameManager.Instance.warriorCountLevel;
            
            case Unit.Archer:
                return GameManager.Instance.archerCountLevel;
            
            case Unit.Monk:
                return GameManager.Instance.monkCountLevel;
            
            case Unit.Knight:
                return GameManager.Instance.knightCountLevel;
            
            case Unit.Mage:
                return GameManager.Instance.mageCountLevel;
            
            default:
                return 0;
        }
    }
    
    void OnPurchaseClick()
    {
        if (GameManager.Instance == null) return;
        
        // 골드 부족하면 무시
        if (GameManager.Instance.currentGold < purchasePrice)
        {
            Debug.Log("골드가 부족합니다!");
            return;
        }
        
        // 골드 차감
        GameManager.Instance.currentGold -= purchasePrice;
        
        // 유닛 타입별 처리
        bool success = false;
        
        switch (unitType)
        {
            case Unit.Warrior:
                GameManager.Instance.warriorCountLevel++;
                if (WarriorSpawner.Instance != null)
                {
                    WarriorSpawner.Instance.SpawnWarrior();
                    success = true;
                }
                Debug.Log($"전사 구매! (보유: {GameManager.Instance.warriorCountLevel}명)");
                break;
            
            case Unit.Archer:
                GameManager.Instance.archerCountLevel++;
                if (WarriorSpawner.Instance != null)
                {
                    WarriorSpawner.Instance.SpawnArcher();
                    success = true;
                }
                Debug.Log($"궁수 구매! (보유: {GameManager.Instance.archerCountLevel}명)");
                break;
            
            case Unit.Monk:
                GameManager.Instance.monkCountLevel++;
                if (WarriorSpawner.Instance != null)
                {
                    WarriorSpawner.Instance.SpawnMonk();
                    success = true;
                }
                Debug.Log($"소드몽 구매! (보유: {GameManager.Instance.monkCountLevel}명)");
                break;
            
            case Unit.Knight:
                GameManager.Instance.knightCountLevel++;
                // TODO: SpawnKnight()
                Debug.Log("기사는 아직 구현되지 않았습니다!");
                break;
            
            case Unit.Mage:
                GameManager.Instance.mageCountLevel++;
                // TODO: SpawnMage()
                Debug.Log("마법사는 아직 구현되지 않았습니다!");
                break;
        }
        
        if (success)
        {
            ShowPurchaseEffect();
        }
        
        Refresh();
    }
    
    void ShowPurchaseEffect()
    {
        if (upgradeButton != null)
        {
            StartCoroutine(PurchaseAnimation());
        }
    }
    
    System.Collections.IEnumerator PurchaseAnimation()
    {
        Image btnImage = upgradeButton.GetComponent<Image>();
        if (btnImage != null)
        {
            Color originalColor = btnImage.color;
            
            // 한 번만 깜빡임 (빠르게)
            btnImage.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            btnImage.color = originalColor;
        }
    }
}