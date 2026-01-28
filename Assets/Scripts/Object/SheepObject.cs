using UnityEngine;

/// <summary>
/// 양 - 자동으로 돌아다니며, Knife로 공격 가능
/// </summary>
public class SheepObject : MonoBehaviour, IInteractable
{
    [Header("양 설정")]
    public int maxHealth = 3;
    public Pawn.ItemType requiredTool = Pawn.ItemType.Knife;
    
    [Header("보상 - CollectibleItem 사용")]
    public GameObject itemPrefab; // Meat 프리팹
    public CollectibleItem.ItemType dropItemType = CollectibleItem.ItemType.Meat;
    public int minItemCount = 1;
    public int maxItemCount = 3;
    public float dropRadius = 1.5f;
    public float dropForce = 2f;
    
    [Header("AI 이동 설정")]
    public float moveSpeed = 1.0f; // 이동 속도
    public float wanderRadius = 5f; // 배회 범위
    public float idleTimeMin = 2f; // 최소 대기 시간
    public float idleTimeMax = 5f; // 최대 대기 시간
    public float arrivalDistance = 0.3f; // 도착 판정 거리
    
    [Header("시각적 피드백")]
    public GameObject hitEffectPrefab;
    public float hitFlashDuration = 0.2f; // 맞았을 때 깜빡임
    
    [Header("상태")]
    [SerializeField] private int currentHealth;
    [SerializeField] private AIState currentState = AIState.Idle;
    
    private Vector2 startPosition; // 시작 위치
    private Vector2 targetPosition; // 목표 위치
    private float idleTimer = 0f;
    private float idleTime = 0f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private bool isFlashing = false;
    
    /// <summary>
    /// AI 상태
    /// </summary>
    enum AIState
    {
        Idle,    // 대기
        Walking  // 이동
    }
    
    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // 첫 대기 시간 설정
        SetRandomIdleTime();
        currentState = AIState.Idle;
        
        gameObject.tag = "Sheep";
    }
    
    void Update()
    {
        UpdateAI();
    }
    
    void FixedUpdate()
    {
        if (currentState == AIState.Walking)
        {
            MoveToTarget();
        }
        else
        {
            // 대기 중에는 멈춤
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    
    void UpdateAI()
    {
        switch (currentState)
        {
            case AIState.Idle:
                UpdateIdle();
                break;
                
            case AIState.Walking:
                UpdateWalking();
                break;
        }
    }
    
    void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        
        // 대기 시간이 끝나면 이동
        if (idleTimer >= idleTime)
        {
            StartWalking();
        }
    }
    
    void UpdateWalking()
    {
        // 목표 지점에 도착했는지 확인
        float distance = Vector2.Distance(transform.position, targetPosition);
        
        if (distance <= arrivalDistance)
        {
            StartIdle();
        }
        
        // 스프라이트 방향 전환
        if (spriteRenderer != null && rb != null)
        {
            if (rb.linearVelocity.x > 0.1f)
                spriteRenderer.flipX = false;
            else if (rb.linearVelocity.x < -0.1f)
                spriteRenderer.flipX = true;
        }
    }
    
    void StartIdle()
    {
        currentState = AIState.Idle;
        idleTimer = 0f;
        SetRandomIdleTime();
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
        
    }
    
    void StartWalking()
    {
        currentState = AIState.Walking;
        
        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }
        // 시작 위치 기준으로 랜덤 위치 선택
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + randomDirection;
        
    }
    
    void MoveToTarget()
    {
        if (rb == null) return;
        
        // 목표 방향
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        
        // 이동
        rb.linearVelocity = direction * moveSpeed;
    }
    
    void SetRandomIdleTime()
    {
        idleTime = Random.Range(idleTimeMin, idleTimeMax);
    }
    
    // ===== IInteractable 구현 =====
    
    public bool CanInteract(Pawn pawn)
    {
        if (currentHealth <= 0)
            return false;
        
        if (pawn.GetCurrentItem() != requiredTool)
        {
            return false;
        }
        
        return true;
    }
    
    public void OnInteractionStart(Pawn pawn)
    {
        // 즉시 공격!
        TakeDamage();
        
        // 잠시 멈춤 (맞았을 때)
        StopMovementBriefly();
    }
    
    public void OnInteractionProgress(float progress)
    {
        // 사용 안 함 (즉시 공격이므로)
    }
    
    public void OnInteractionComplete(Pawn pawn)
    {
        // 사용 안 함 (즉시 공격이므로)
    }
    
    public void OnInteractionCancel(Pawn pawn)
    {
        // 사용 안 함
    }
    
    public float GetInteractionDuration()
    {
        // 0 = 즉시 공격
        return 0f;
    }
    
    void TakeDamage()
    {
        currentHealth--;
        
        Debug.Log($"양 공격! 남은 체력: {currentHealth}/{maxHealth}");
        
        // 맞았을 때 효과
        PlayHitEffect();
        FlashRed();
        
        // 죽음
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void StopMovementBriefly()
    {
        // 맞으면 잠시 멈춤
        if (currentState == AIState.Walking)
        {
            StartIdle();
            idleTime = 0.5f; // 0.5초만 멈춤
        }
    }
    
    void Die()
    {
        Debug.Log("양이 죽었습니다!");
        
        DropItems();
        SpawnDeathEffect();
        
        Destroy(gameObject);
    }
    
    void DropItems()
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("아이템 프리팹이 설정되지 않았습니다!");
            return;
        }
        
        int itemCount = Random.Range(minItemCount, maxItemCount + 1);
        
        Debug.Log($"{GetItemName()} {itemCount}개 드롭!");
        
        for (int i = 0; i < itemCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
            Vector3 dropPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            GameObject item = Instantiate(itemPrefab, dropPosition, Quaternion.identity);
            
            CollectibleItem collectible = item.GetComponent<CollectibleItem>();
            if (collectible != null)
            {
                collectible.SetItem(dropItemType, 1);
            }
            
            Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                itemRb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
            }
        }
    }
    
    void PlayHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    
    void SpawnDeathEffect()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 2f;
        }
    }
    
    void FlashRed()
    {
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(FlashCoroutine());
        }
    }
    
    System.Collections.IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
        
        isFlashing = false;
    }
    
    string GetItemName()
    {
        switch (dropItemType)
        {
            case CollectibleItem.ItemType.Meat: return "고기";
            default: return "아이템";
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // 배회 범위 표시
        Gizmos.color = Color.cyan;
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(center, wanderRadius);
        
        // 아이템 드롭 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
        
        // 목표 지점 (플레이 중일 때만)
        if (Application.isPlaying && currentState == AIState.Walking)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.3f);
        }
    }
}