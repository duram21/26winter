using UnityEngine;

/// <summary>
/// 금광석 - Pickaxe로 채굴 가능, 고정된 위치
/// </summary>
public class GoldOreObject : MonoBehaviour, IInteractable
{
    [Header("광석 설정")]
    public int maxHealth = 5; // 채굴 횟수
    public Pawn.ItemType requiredTool = Pawn.ItemType.Pickaxe;
    
    [Header("상호작용 시간")]
    public float interactionTimePerHit = 2f; // 1회 채굴 시간
    
    [Header("보상 - CollectibleItem 사용")]
    public GameObject itemPrefab; // Gold 프리팹
    public CollectibleItem.ItemType dropItemType = CollectibleItem.ItemType.Gold;
    public int minGoldCount = 3; // 골드는 많이!
    public int maxGoldCount = 8;
    public float dropRadius = 1.5f;
    public float dropForce = 3f; // 금은 무거워서 적게 튐
    
    [Header("시각적 피드백")]
    public GameObject hitEffectPrefab; // 타격 효과
    public bool showProgressBar = true;
    public Color damageColor = new Color(1f, 0.8f, 0f); // 황금색
    
    [Header("상태")]
    [SerializeField] private int currentHealth;
    [SerializeField] private float currentProgress = 0f;
    
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
        gameObject.tag = "GoldOre";
    }
    
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
        isBeingInteracted = true;
        currentProgress = 0f;
        
        Debug.Log($"금광석 채굴 시작! 남은 체력: {currentHealth}/{maxHealth}");
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(originalColor, damageColor, 0.3f);
        }
    }
    
    public void OnInteractionProgress(float progress)
    {
        currentProgress = progress;
        
        if (spriteRenderer != null)
        {
            // 진행도에 따라 황금색으로 변함
            float colorLerp = progress * 0.5f;
            spriteRenderer.color = Color.Lerp(originalColor, damageColor, colorLerp);
        }
    }
    
    public void OnInteractionComplete(Pawn pawn)
    {
        isBeingInteracted = false;
        currentProgress = 0f;
        
        currentHealth--;
        
        Debug.Log($"채굴 완료! 남은 체력: {currentHealth}/{maxHealth}");
        
        SpawnHitEffect();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        if (currentHealth <= 0)
        {
            Mine();
        }
    }
    
    public void OnInteractionCancel(Pawn pawn)
    {
        isBeingInteracted = false;
        currentProgress = 0f;
        
        Debug.Log("채굴 취소됨!");
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    public float GetInteractionDuration()
    {
        return interactionTimePerHit;
    }
    
    void Mine()
    {
        Debug.Log("금광석이 완전히 채굴되었습니다!");
        
        DropGold();
        SpawnMineEffect();
        
        Destroy(gameObject);
    }
    
    void DropGold()
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("골드 프리팹이 설정되지 않았습니다!");
            return;
        }
        
        int goldCount = Random.Range(minGoldCount, maxGoldCount + 1);
        
        Debug.Log($"골드 {goldCount}개 획득!");
        
        for (int i = 0; i < goldCount; i++)
        {
            // 랜덤 위치
            Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
            Vector3 dropPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // 골드 생성
            GameObject gold = Instantiate(itemPrefab, dropPosition, Quaternion.identity);
            
            // CollectibleItem 설정
            CollectibleItem collectible = gold.GetComponent<CollectibleItem>();
            if (collectible != null)
            {
                collectible.SetItem(dropItemType, 1);
            }
            
            // 물리 효과 (금은 무거워서 적게 튐)
            Rigidbody2D rb = gold.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
            }
        }
    }
    
    void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            
            // 황금색 이펙트 (옵션)
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = damageColor;
            }
        }
    }
    
    void SpawnMineEffect()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 2f;
            
            // 황금색 이펙트
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = Color.yellow;
            }
        }
    }
    
    void OnGUI()
    {
        if (!showProgressBar || !isBeingInteracted) return;
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        
        if (screenPos.z > 0)
        {
            float barWidth = 100f;
            float barHeight = 10f;
            
            Rect bgRect = new Rect(screenPos.x - barWidth / 2, Screen.height - screenPos.y - barHeight / 2, barWidth, barHeight);
            Rect fgRect = new Rect(screenPos.x - barWidth / 2, Screen.height - screenPos.y - barHeight / 2, barWidth * currentProgress, barHeight);
            
            GUI.color = Color.gray;
            GUI.DrawTexture(bgRect, Texture2D.whiteTexture);
            
            // 황금색 진행도 바
            GUI.color = Color.yellow;
            GUI.DrawTexture(fgRect, Texture2D.whiteTexture);
            
            GUI.color = Color.black;
            GUI.Box(bgRect, "");
            
            GUI.color = Color.white;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // 드롭 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }
}