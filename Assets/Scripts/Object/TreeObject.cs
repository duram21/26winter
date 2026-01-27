using UnityEngine;

/// <summary>
/// 나무 - 지속적으로 상호작용하여 벌목 가능
/// </summary>
public class Tree : MonoBehaviour, IInteractable
{
    [Header("나무 설정")]
    public int maxHealth = 3; // 몇 번 벌목해야 하는지
    public Pawn.ItemType requiredTool = Pawn.ItemType.Axe; // 필요한 도구
    
    [Header("상호작용 시간")]
    public float interactionTimePerHit = 2f; // 1회 벌목에 걸리는 시간
    
    [Header("보상")]
    public GameObject woodPrefab; // 나무 아이템 프리팹
    public int minWoodCount = 2; // 최소 드롭 개수
    public int maxWoodCount = 5; // 최대 드롭 개수
    public float dropRadius = 1.5f; // 드롭 반경
    public float dropForce = 2f; // 드롭 힘
    
    [Header("시각적 피드백")]
    public GameObject hitEffectPrefab; // 타격 효과
    public bool showProgressBar = true; // 진행도 표시
    
    [Header("상태")]
    [SerializeField] private int currentHealth;
    [SerializeField] private float currentProgress = 0f; // 현재 벌목 진행도 (0~1)
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isBeingInteracted = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Tag 설정
        gameObject.tag = "Tree";
    }
    
    /// <summary>
    /// 상호작용 가능한지 확인
    /// </summary>
    public bool CanInteract(Pawn pawn)
    {
        // 이미 파괴되었으면 불가
        if (currentHealth <= 0)
            return false;
        
        // 올바른 도구를 들고 있는지 확인
        if (pawn.GetCurrentItem() != requiredTool)
        {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 상호작용 시작
    /// </summary>
    public void OnInteractionStart(Pawn pawn)
    {
        isBeingInteracted = true;
        currentProgress = 0f;
        
        Debug.Log($"나무 벌목 시작! 남은 체력: {currentHealth}/{maxHealth}");
        
        // 시각적 피드백 - 색상 변경
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(originalColor, Color.red, 0.3f);
        }
    }
    
    /// <summary>
    /// 상호작용 진행 중
    /// </summary>
    public void OnInteractionProgress(float progress)
    {
        currentProgress = progress;
        
        // 진행도에 따라 색상 변경
        if (spriteRenderer != null)
        {
            float colorLerp = progress * 0.5f;
            spriteRenderer.color = Color.Lerp(originalColor, Color.red, colorLerp);
        }
    }
    
    /// <summary>
    /// 상호작용 완료 (벌목 1회 완료)
    /// </summary>
    public void OnInteractionComplete(Pawn pawn)
    {
        isBeingInteracted = false;
        currentProgress = 0f;
        
        // 체력 감소
        currentHealth--;
        
        Debug.Log($"벌목 완료! 남은 체력: {currentHealth}/{maxHealth}");
        
        // 타격 효과
        SpawnHitEffect();
        
        // 색상 원상복구
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        // 나무가 완전히 베어졌는지 확인
        if (currentHealth <= 0)
        {
            ChopDown();
        }
    }
    
    /// <summary>
    /// 상호작용 취소
    /// </summary>
    public void OnInteractionCancel(Pawn pawn)
    {
        isBeingInteracted = false;
        currentProgress = 0f;
        
        Debug.Log("벌목 취소됨!");
        
        // 색상 원상복구
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    /// <summary>
    /// 나무 베어짐
    /// </summary>
    void ChopDown()
    {
        Debug.Log("나무가 완전히 베어졌습니다!");
        
        // 나무 아이템 드롭
        DropWood();
        
        // 파괴 효과 (옵션)
        SpawnChopEffect();
        
        // 나무 오브젝트 파괴
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 나무 아이템 드롭
    /// </summary>
    void DropWood()
    {
        if (woodPrefab == null)
        {
            Debug.LogWarning("나무 프리팹이 설정되지 않았습니다!");
            return;
        }
        
        // 랜덤 개수 결정
        int woodCount = Random.Range(minWoodCount, maxWoodCount + 1);
        
        Debug.Log($"나무 {woodCount}개 드롭!");
        
        for (int i = 0; i < woodCount; i++)
        {
            // 랜덤 위치
            Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
            Vector3 dropPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // 나무 아이템 생성
            GameObject wood = Instantiate(woodPrefab, dropPosition, Quaternion.identity);
            
            // 물리 효과
            Rigidbody2D rb = wood.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
            }
        }
    }
    
    /// <summary>
    /// 타격 효과 생성
    /// </summary>
    void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// 벌목 완료 효과
    /// </summary>
    void SpawnChopEffect()
    {
        // 더 큰 효과 (옵션)
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            // 크기 2배
            effect.transform.localScale = Vector3.one * 2f;
        }
    }
    
    /// <summary>
    /// 진행도 UI 표시 (간단한 버전)
    /// </summary>
    void OnGUI()
    {
        if (!showProgressBar || !isBeingInteracted) return;
        
        // 화면 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        
        if (screenPos.z > 0) // 카메라 앞에 있을 때만
        {
            // 진행도 바 그리기
            float barWidth = 100f;
            float barHeight = 10f;
            
            Rect bgRect = new Rect(screenPos.x - barWidth / 2, Screen.height - screenPos.y - barHeight / 2, barWidth, barHeight);
            Rect fgRect = new Rect(screenPos.x - barWidth / 2, Screen.height - screenPos.y - barHeight / 2, barWidth * currentProgress, barHeight);
            
            // 배경 (회색)
            GUI.color = Color.gray;
            GUI.DrawTexture(bgRect, Texture2D.whiteTexture);
            
            // 진행도 (녹색)
            GUI.color = Color.green;
            GUI.DrawTexture(fgRect, Texture2D.whiteTexture);
            
            // 테두리 (검은색)
            GUI.color = Color.black;
            GUI.Box(bgRect, "");
            
            GUI.color = Color.white; // 원상복구
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // 드롭 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }
}