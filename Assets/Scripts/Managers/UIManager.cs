using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("상단 HUD")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI timeText;
    
    [Header("하단 패널들")]
    public GameObject upgradePanel;
    public GameObject shopPanel;
    public GameObject equipmentPanel;
    public GameObject settingsPanel;
    
    [Header("탭 버튼들")]
    public Button upgradeButton;
    public Button shopButton;
    public Button equipmentButton;
    public Button settingsButton;
    
    [Header("버튼 색상")]
    public Color selectedColor = new Color(1f, 0.84f, 0f);  // 금색
    public Color normalColor = new Color(0.7f, 0.7f, 0.7f); // 회색
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    void Start()
    {
        // 버튼 이벤트 연결
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(() => ShowPanel(upgradePanel, upgradeButton));
        
        if (shopButton != null)
            shopButton.onClick.AddListener(() => ShowPanel(shopPanel, shopButton));
        
        if (equipmentButton != null)
            equipmentButton.onClick.AddListener(() => ShowPanel(equipmentPanel, equipmentButton));
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel, settingsButton));
        
        // 초기: 모든 패널 끄기
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // 모든 버튼 회색으로
        if (upgradeButton != null) SetButtonColor(upgradeButton, normalColor);
        if (shopButton != null) SetButtonColor(shopButton, normalColor);
        if (equipmentButton != null) SetButtonColor(equipmentButton, normalColor);
        if (settingsButton != null) SetButtonColor(settingsButton, normalColor);
    }
    
    void Update()
    {
        UpdateHUD();
    }
    
    void UpdateHUD()
    {
        if (GameManager.Instance != null)
        {
            if (goldText != null)
                goldText.text = "Gold: " + GameManager.Instance.currentGold.ToString("N0");
            
            if (killText != null)
                killText.text = "Kill: " + GameManager.Instance.totalKillCount.ToString();
            
            if (timeText != null)
                timeText.text = "Time: " + GameManager.Instance.GetPlayTimeString();
        }
    }
    
    public void ShowPanel(GameObject panel, Button button)
    {
        // 모든 패널 끄기
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // 모든 버튼 색상 리셋
        if (upgradeButton != null) SetButtonColor(upgradeButton, normalColor);
        if (shopButton != null) SetButtonColor(shopButton, normalColor);
        if (equipmentButton != null) SetButtonColor(equipmentButton, normalColor);
        if (settingsButton != null) SetButtonColor(settingsButton, normalColor);
        
        // 선택한 패널만 켜기
        if (panel != null)
        {
            panel.SetActive(true);
            
            // 패널이 활성화되면 새로고침
            if (panel == upgradePanel)
            {
                UpgradePanel upgradeScript = panel.GetComponent<UpgradePanel>();
                if (upgradeScript != null)
                    upgradeScript.RefreshAllUpgrades();
            }
        }
        
        // 선택한 버튼 강조
        if (button != null)
        {
            SetButtonColor(button, selectedColor);
        }
    }
    
    void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.2f;
        colors.pressedColor = color * 0.8f;
        colors.selectedColor = color;
        button.colors = colors;
    }
}
