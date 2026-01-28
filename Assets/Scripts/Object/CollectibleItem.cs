using UnityEngine;

/// <summary>
/// 수집 가능한 아이템 - Wood, Gold, Meat 등 모든 드롭 아이템
/// Sprite만 바꿔서 재사용 가능
/// </summary>
public class CollectibleItem : MonoBehaviour
{
    [Header("아이템 타입")]
    public ItemType itemType = ItemType.Wood;
    public int amount = 1; // 아이템 개수
    
    [Header("자동 습득")]
    public float pickupRange = 1.5f; // 자동 습득 범위
    public float moveSpeed = 3f; // 플레이어에게 이동하는 속도
    public float dropTime = 0.5f; // 몇 초 뒤부터 먹을 건지 
    public float dropTimer = 0f;
    
    [Header("물리")]
    public bool usePhysics = true; // 물리 효과 사용
    public float stopDelay = 2f; // 멈추는 시간
    
    [Header("자동 제거")]
    public bool autoDestroy = true; // 자동 제거
    public float destroyDelay = 30f; // 30초 후 제거
    
    [Header("효과 (옵션)")]
    public GameObject pickupEffectPrefab;
    public AudioClip pickupSound;
    
    private Transform player;
    private Rigidbody2D rb;
    private bool isMovingToPlayer = false;
    private bool hasBeenPickedUp = false;
    
    /// <summary>
    /// 아이템 종류
    /// </summary>
    public enum ItemType
    {
        Wood,      // 나무
        Stone,     // 돌
        Gold,      // 골드
        Meat,      // 고기
        Ore,       // 광석
        Herb,      // 약초
        Berry      // 열매
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dropTimer = 0f;
        
        // 물리 저항 설정
        if (rb != null && usePhysics)
        {
            rb.linearDamping = 2f;
            rb.angularDamping = 1f;
        }
        
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // 일정 시간 후 완전히 멈춤
        if (usePhysics)
        {
            Invoke(nameof(StopMovement), stopDelay);
        }
        
        // 자동 제거
        if (autoDestroy)
        {
            Invoke(nameof(AutoDestroy), destroyDelay);
        }
    }
    
    void Update()
    {
        if (player == null || hasBeenPickedUp) return;
        
        // 플레이어와의 거리
        float distance = Vector2.Distance(transform.position, player.position);
        
        dropTimer += Time.deltaTime;
        // 범위 내에 들어오면 플레이어에게 이동
        if (distance <= pickupRange && dropTimer >= dropTime)
        {
            isMovingToPlayer = true;
            MoveToPlayer();
        }

    }
    
    void MoveToPlayer()
    {
        if (rb != null)
        {
            rb.linearDamping = 0f; // 저항 제거
            rb.gravityScale = 0f; // 중력 제거
        }
        
        // 플레이어 방향으로 이동
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }
    
    void StopMovement()
    {
        if (rb != null && !isMovingToPlayer)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
    
    void AutoDestroy()
    {
        if (!hasBeenPickedUp)
        {
            // 페이드 아웃 효과 (옵션)
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenPickedUp) return;
        
        // 플레이어와 충돌하면 습득
        if (other.CompareTag("Player") && dropTimer >= dropTime)
        {
            Pickup(other.gameObject);
        }
    }
    
    void Pickup(GameObject player)
    {
        hasBeenPickedUp = true;
        
        // 아이템 타입에 따라 처리
        string itemName = GetItemName();
        Debug.Log($"{itemName} {amount}개 획득!");
        
        // 인벤토리 시스템에 추가 (있다면)
        // GameManager나 InventoryManager에 알림
        NotifyPickup(player);
        
        // 습득 효과
        PlayPickupEffect();
        
        // 아이템 제거
        Destroy(gameObject);
    }
    
    void NotifyPickup(GameObject player)
    {
        // GameManager가 있다면 알림
        // GameManager.Instance.AddItem(itemType, amount);
        
        // Pawn에게 알림 (옵션)
        Pawn pawn = player.GetComponent<Pawn>();
        if (pawn != null)
        {
            // pawn.OnItemCollected(itemType, amount);
        }
        
        // UI 업데이트 (옵션)
        // UIManager.Instance.ShowItemNotification(itemType, amount);
    }
    
    void PlayPickupEffect()
    {
        // 파티클 효과
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // 사운드
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }
    
    /// <summary>
    /// 아이템 이름 가져오기
    /// </summary>
    string GetItemName()
    {
        switch (itemType)
        {
            case ItemType.Wood: return "나무";
            case ItemType.Stone: return "돌";
            case ItemType.Gold: return "골드";
            case ItemType.Meat: return "고기";
            case ItemType.Ore: return "광석";
            case ItemType.Herb: return "약초";
            case ItemType.Berry: return "열매";
            default: return "아이템";
        }
    }
    
    /// <summary>
    /// 외부에서 아이템 설정
    /// </summary>
    public void SetItem(ItemType type, int count)
    {
        itemType = type;
        amount = count;
    }
    
    void OnDrawGizmosSelected()
    {
        // 자동 습득 범위 표시
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}