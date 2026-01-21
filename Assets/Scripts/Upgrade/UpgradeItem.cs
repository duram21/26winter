using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    [Header("업그레이드 데이터")]
    public UpgradeData upgradeData;
    
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
        
        // 항상 표시! (숨기지 않음)
        gameObject.SetActive(true);
        
        // 텍스트 업데이트
        titleText.text = upgradeData.upgradeName;
        
        // 최대 레벨 여부에 따라 다르게 표시
        if (upgradeData.IsMaxLevel())
        {
            levelText.text = "MAX";  // 또는 "최대 레벨"
            descText.text = upgradeData.description;
            buttonText.text = "최대 레벨";
        }
        else
        {
            levelText.text = upgradeData.GetLevelText();
            descText.text = upgradeData.GetDescriptionText();
            
            int cost = upgradeData.GetNextCost();
            buttonText.text = $"{cost}G";
        }
        
        UpdateButtonState();
    }
    
    void UpdateButtonState()
    {
        if (upgradeData == null) return;
        
        // 최대 레벨이면 버튼 비활성화
        if (upgradeData.IsMaxLevel())
        {
            upgradeButton.interactable = false;
            
            Image buttonImage = upgradeButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.2f, 0.5f, 0.2f);  // 어두운 초록 (완료 느낌)
            }
            
            buttonText.color = Color.white;
            return;
        }
        
        // 일반 상태
        bool canAfford = upgradeData.CanUpgrade();
        upgradeButton.interactable = canAfford;
        
        Image buttonImage2 = upgradeButton.GetComponent<Image>();
        if (buttonImage2 != null)
        {
            buttonImage2.color = canAfford ? 
                new Color(0.3f, 0.69f, 0.31f) :  // 초록
                new Color(0.3f, 0.3f, 0.3f);     // 회색
        }
        
        buttonText.color = canAfford ? Color.white : Color.red;
    }
    
    void OnUpgradeClick()
    {
        if (upgradeData == null) return;
        
        if (upgradeData.TryUpgrade())
        {
            Refresh();
            
            UpgradePanel panel = GetComponentInParent<UpgradePanel>();
            if (panel != null)
            {
                panel.RefreshAllUpgrades();
            }
        }
    }
}