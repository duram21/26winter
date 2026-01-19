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
    
    private GameObject currentPanel;   // 현재 열린 패널
    private Button currentButton;      // 현재 선택된 버튼
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    void Start()
    {
        // 버튼 이벤트 연결
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(() => TogglePanel(upgradePanel, upgradeButton));
        
        if (shopButton != null)
            shopButton.onClick.AddListener(() => TogglePanel(shopPanel, shopButton));
        
        if (equipmentButton != null)
            equipmentButton.onClick.AddListener(() => TogglePanel(equipmentPanel, equipmentButton));
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => TogglePanel(settingsPanel, settingsButton));
        
        // 초기: 모든 패널 끄기
        CloseAllPanels();
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
    
    // 토글 방식!
    public void TogglePanel(GameObject panel, Button button)
    {
        if (panel == null || button == null)
            return;
        
        // 같은 버튼을 다시 클릭한 경우
        if (currentPanel == panel && panel.activeSelf)
        {
            // 패널 끄기
            panel.SetActive(false);
            SetButtonColor(button, normalColor);
            
            currentPanel = null;
            currentButton = null;
        }
        else
        {
            // 다른 버튼을 클릭한 경우 (또는 처음 클릭)
            
            // 1. 모든 패널 끄기
            CloseAllPanels();
            
            // 2. 선택한 패널만 켜기
            panel.SetActive(true);
            SetButtonColor(button, selectedColor);
            
            // 3. 현재 상태 저장
            currentPanel = panel;
            currentButton = button;
            
            // 4. 패널이 활성화되면 새로고침
            RefreshPanel(panel);
        }
    }
    
    // 모든 패널 끄기
    void CloseAllPanels()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        if (upgradeButton != null) SetButtonColor(upgradeButton, normalColor);
        if (shopButton != null) SetButtonColor(shopButton, normalColor);
        if (equipmentButton != null) SetButtonColor(equipmentButton, normalColor);
        if (settingsButton != null) SetButtonColor(settingsButton, normalColor);
    }
    
    // 패널 새로고침
    void RefreshPanel(GameObject panel)
    {
        if (panel == upgradePanel)
        {
            UpgradePanel upgradeScript = panel.GetComponent<UpgradePanel>();
            if (upgradeScript != null)
                upgradeScript.RefreshAllUpgrades();
        }
        // 나중에 다른 패널들도 추가
        // else if (panel == shopPanel)
        // {
        //     ShopPanel shopScript = panel.GetComponent<ShopPanel>();
        //     if (shopScript != null)
        //         shopScript.RefreshShop();
        // }
    }
    
    // 버튼 색상 변경
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
