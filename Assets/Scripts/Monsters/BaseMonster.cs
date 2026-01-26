using UnityEngine;

/// <summary>
/// 모든 몬스터의 베이스 클래스
/// Monster, EliteMonster가 이를 상속받음
/// </summary>
public abstract class BaseMonster : MonoBehaviour
{
    [Header("Spawner Info")]
    public MonsterSpawner spawner;
    
    [Header("스탯")]
    public float maxHealth = 10f;
    public int goldReward = 5;
    public float moveSpeed = 0.5f;
    
    [Header("상태")]
    public bool isDead = false;
    protected float currentHealth;
    
    // 이동 관련
    protected Vector2 moveDirection;
    protected float decisionTimer;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        gameObject.tag = "Monster";
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 처음 시작할 때 랜덤한 방향 설정
        SetRandomDirection();
    }
    
    protected virtual void Update()
    {
        if (isDead) return;

        // 조금씩 이동
        MoveSlowly();

        // 주기적으로 방향 바꾸기
        decisionTimer += Time.deltaTime;
        if (decisionTimer >= 3f) // 3초마다 방향 전환
        {
            SetRandomDirection();
            decisionTimer = 0;
        }
    }

    /// <summary>
    /// 천천히 이동
    /// </summary>
    protected virtual void MoveSlowly()
    {
        // 실제로 위치를 옮기는 코드
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 이동 애니메이션
        if (anim != null) 
            anim.SetBool("isMoving", true);

        // 방향에 따라 좌우 반전
        if (moveDirection.x < 0) 
            spriteRenderer.flipX = true;
        else if (moveDirection.x > 0) 
            spriteRenderer.flipX = false;
    }

    /// <summary>
    /// 랜덤 방향 설정
    /// </summary>
    protected virtual void SetRandomDirection()
    {
        // -1에서 1 사이의 랜덤한 방향 생성
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    
    /// <summary>
    /// 데미지 받기
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 피격! 남은 체력: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 사망 (각 몬스터 타입에서 오버라이드 가능)
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} 사망!");
        
        // 태그를 바꿔서 워리어가 더 이상 추적하지 않게 함
        gameObject.tag = "Untagged";

        // 골드 지급
        if (GameManager.Instance != null)
            GameManager.Instance.AddGold(goldReward);
        
        // 스포너에게 알림
        if (spawner != null)
            spawner.OnMonsterDied();
        
        // 파괴
        Destroy(gameObject, 0.1f);
    }
}