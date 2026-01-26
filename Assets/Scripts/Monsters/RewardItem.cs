using UnityEngine;

/// <summary>
/// 보상 아이템 베이스 클래스
/// </summary>
public class RewardItem : MonoBehaviour
{
    [Header("보상 설정")]
    public RewardType rewardType;
    public int rewardAmount = 10;
    
    [Header("자동 습득 설정")]
    public bool autoPickup = true; // 플레이어 근처 가면 자동 습득
    public float pickupRange = 2f; // 자동 습득 범위
    public float moveSpeed = 5f; // 플레이어에게 이동하는 속도
    
    [Header("효과")]
    public GameObject pickupEffectPrefab; // 습득 시 이펙트
    public AudioClip pickupSound; // 습득 소리
    
    private Transform player;
    private bool isMovingToPlayer = false;
    private Rigidbody2D rb;
    
    public enum RewardType
    {
        Gold,           // 골드
        Experience,     // 경험치
        HealthPotion,   // 체력 물약
        SpecialItem,    // 특별 아이템
        UpgradeToken    // 업그레이드 토큰
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // 일정 시간 후 사라지도록 (옵션)
        Invoke(nameof(AutoDestroy), 30f); // 30초 후 자동 제거
    }
    
    void Update()
    {
        if (!autoPickup || player == null)
            return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        // 범위 안에 들어오면 플레이어에게 이동
        if (distance <= pickupRange)
        {
            isMovingToPlayer = true;
        }
        
        // 플레이어에게 이동
        if (isMovingToPlayer)
        {
            MoveToPlayer();
            
            // 플레이어와 닿으면 습득
            if (distance <= 0.5f)
            {
                Pickup();
            }
        }
    }
    
    void MoveToPlayer()
    {
        if (player == null) return;
        
        // Rigidbody2D 중력 끄기
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }
        
        // 플레이어 방향으로 이동
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }
    
    /// <summary>
    /// 보상 습득
    /// </summary>
    void Pickup()
    {
        Debug.Log($"{rewardType} 획득! 수량: {rewardAmount}");
        
        // 보상 적용
        ApplyReward();
        
        // 습득 효과
        PlayPickupEffect();
        
        // 아이템 제거
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 보상 적용
    /// </summary>
    void ApplyReward()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager가 없습니다!");
            return;
        }
        
        switch (rewardType)
        {
            case RewardType.Gold:
                GameManager.Instance.AddGold(rewardAmount);
                Debug.Log($"골드 +{rewardAmount}");
                break;
                
            case RewardType.Experience:
                // GameManager.Instance.AddExperience(rewardAmount);
                Debug.Log($"경험치 +{rewardAmount}");
                break;
                
            case RewardType.HealthPotion:
                // GameManager.Instance.AddHealthPotion(1);
                Debug.Log("체력 물약 획득!");
                break;
                
            case RewardType.SpecialItem:
                Debug.Log("특별 아이템 획득!");
                // 특별 아이템 처리
                break;
                
            case RewardType.UpgradeToken:
                Debug.Log($"업그레이드 토큰 +{rewardAmount}");
                // 업그레이드 토큰 처리
                break;
        }
    }
    
    /// <summary>
    /// 습득 효과 재생
    /// </summary>
    void PlayPickupEffect()
    {
        // 이펙트 생성
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // 소리 재생
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }
    
    /// <summary>
    /// 자동 제거 (시간 초과)
    /// </summary>
    void AutoDestroy()
    {
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 마우스 클릭으로도 습득 가능 (옵션)
    /// </summary>
    void OnMouseDown()
    {
        if (!isMovingToPlayer)
        {
            Pickup();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // 자동 습득 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}