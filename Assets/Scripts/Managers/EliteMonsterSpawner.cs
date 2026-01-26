using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // TextMeshPro 사용 시

public class EliteMonsterSpawner : MonoBehaviour
{
    public static EliteMonsterSpawner Instance;
    

    [Header("토큰 설정")]
    [SerializeField] private int maxTokens = 10; // 최대 토큰 (10마리)
    public int currentTokens = 0; // 현재 토큰

    public int currentKills = 0; // 현재 몇 마리 게이지 채웠는 지
    public int killsPerToken = 10; // 잡아야 하는 몬스터 수
    
    [Header("UI 요소")]
    [SerializeField] private Slider tokenSlider; // 슬라이더
    [SerializeField] private TextMeshProUGUI tokenText; // TextMeshPro 사용
    [SerializeField] private Button spawnButton; // 소환 버튼
    
    [Header("엘리트 몬스터 설정")]
    [SerializeField] private GameObject eliteMonsterPrefab; // 엘리트 몬스터 프리팹
    [SerializeField] private Transform spawnPoint; // 소환 위치
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        // 초기화
        UpdateUI();
        
        // 버튼 클릭 이벤트 연결
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnEliteMonster);
            // spawnButton.interactable = false; // 처음엔 비활성화
        }
        
        // 슬라이더 설정
        if (tokenSlider != null)
        {
            tokenSlider.maxValue = maxTokens;
            tokenSlider.value = 0;
        }
    }
    

    public void OnMonsterKilled()
    {
        currentKills++;

        if(currentTokens < maxTokens)
        {
            if(currentKills >= killsPerToken)
            {
                currentKills = 0;
                currentTokens++;
            }
        }
        else
        {
            currentKills = Math.Min(currentKills, killsPerToken);
        }
        
        UpdateUI();
        Debug.Log("업데이트됨");
    }
    
    public void SpawnEliteMonster()
    {
        if(currentTokens <= 0) return;


        currentTokens--;

        // 엘리트 몬스터 생성
        if (eliteMonsterPrefab != null && spawnPoint != null)
        {
            GameObject eliteMonster = Instantiate(eliteMonsterPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("엘리트 몬스터 소환!");
        }
        else if (eliteMonsterPrefab != null)
        {
            // spawnPoint가 없으면 플레이어 앞에 소환
            Vector3 spawnPos = transform.position + transform.forward * 3f;
            GameObject eliteMonster = Instantiate(eliteMonsterPrefab, spawnPos, Quaternion.identity);
            Debug.Log("엘리트 몬스터 소환!");
        }
        else
        {
            Debug.LogWarning("엘리트 몬스터 프리팹이 설정되지 않았습니다!");
        }

        UpdateUI();

        // 모든 아군이 엘리트 몬스터 때리도록 구현해볼까 ?
        

    }
    private void UpdateUI()
    {
        // 슬라이더 업데이트
        if (tokenSlider != null)
        {
            tokenSlider.value = currentKills;
        }
        
        // 텍스트 업데이트 (TextMeshPro)
        if (tokenText != null)
        {
            tokenText.text = $"{currentTokens} / {maxTokens}";
        }

    }
    
    public int GetCurrentTokens()
    {
        return currentTokens;
    }

    public bool IsTokenFull()
    {
        return currentTokens >= maxTokens;
    }
}