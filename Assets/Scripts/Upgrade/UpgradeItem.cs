using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    [Header("업그레이드 데이터")]
    public UpgradeData upgradeData;  // ← Inspector에서 연결!
    
    [Header("UI 참조")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI buttonText;
    public Button upgradeButton;
    
    void Start()
    {
        if (upgradeData == null)
        {
            Debug.LogWarning($"{gameObject.name}에 UpgradeData가 연결되지 않았습니다!");
            gameObject.SetActive(false);
            return;
        }
        
        // 버튼 이벤트 연결
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClick);
        
        Refresh();
    }
    
    void Update()
    {
        if (upgradeData != null && gameObject.activeSelf)
        {
            UpdateButtonState();
        }
    }
    
    public void Refresh()
    {
        if (upgradeData == null) return;
        
        // 최대 레벨이면 숨기기
        if (upgradeData.IsMaxLevel())
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        
        // 텍스트 업데이트
        titleText.text = upgradeData.upgradeName;
        levelText.text = upgradeData.GetLevelText();
        descText.text = upgradeData.GetDescriptionText();
        
        int cost = upgradeData.GetNextCost();
        buttonText.text = $"{cost}G";
        
        UpdateButtonState();
    }
    
    void UpdateButtonState()
    {
        if (upgradeData == null) return;
        
        bool canAfford = upgradeData.CanUpgrade();
        upgradeButton.interactable = canAfford;
        
        // 버튼 색상
        Image buttonImage = upgradeButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = canAfford ? 
                new Color(0.3f, 0.69f, 0.31f) :  // 초록
                new Color(0.3f, 0.3f, 0.3f);     // 회색
        }
        
        // 텍스트 색상
        buttonText.color = canAfford ? Color.white : Color.red;
    }
    
    void OnUpgradeClick()
    {
        if (upgradeData == null) return;
        
        if (upgradeData.TryUpgrade())
        {
            // 업그레이드 성공!
            Refresh();
            
            // 다른 항목들도 새로고침
            UpgradePanel panel = GetComponentInParent<UpgradePanel>();
            if (panel != null)
            {
                panel.RefreshAllUpgrades();
            }
        }
    }
}