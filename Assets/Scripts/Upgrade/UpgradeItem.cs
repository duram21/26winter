using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    [Header("UI 참조")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI buttonText;
    public Button upgradeButton;
    
    private System.Action onUpgradeCallback;
    private int currentCost;
    
    public void Setup(string title, string levelInfo, string description, int cost, System.Action onUpgrade)
    {
        titleText.text = title;
        levelText.text = levelInfo;
        descText.text = description;
        buttonText.text = $"{cost}G";
        
        currentCost = cost;
        onUpgradeCallback = onUpgrade;
        
        // 버튼 이벤트 연결
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnButtonClick);
        
        UpdateButtonState();
    }
    
    void Update()
    {
        UpdateButtonState();
    }
    
    void UpdateButtonState()
    {
        if (GameManager.Instance == null) return;
        
        // 골드 부족 체크
        bool canAfford = GameManager.Instance.currentGold >= currentCost;
        
        upgradeButton.interactable = canAfford;
        
        // 색상 변경
        Image buttonImage = upgradeButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = canAfford ? 
                new Color(0.3f, 0.69f, 0.31f) :  // 초록 (살 수 있음)
                new Color(0.3f, 0.3f, 0.3f);     // 회색 (골드 부족)
        }
        
        buttonText.color = canAfford ? Color.white : Color.red;
    }
    
    void OnButtonClick()
    {
        if (onUpgradeCallback != null)
        {
            onUpgradeCallback.Invoke();
        }
    }
}