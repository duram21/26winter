using UnityEngine;

/// <summary>
/// 모든 아군 유닛의 베이스 클래스
/// Warrior, Monk, Archer가 이를 상속받음
/// </summary>
public abstract class BaseAlly : MonoBehaviour
{
    [Header("기본 스탯")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public float attackAnimationDuration = 0.5f;
    
    [Header("체력")]
    public int maxHealth = 100;
    public int currentHealth;
    
    // 공통 변수들
    protected Transform targetEnemy;
    protected float lastAttackTime;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected bool isAttacking = false;
    
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        
        // AllyManager에 등록
        if (AllyManager.Instance != null)
        {
            AllyManager.Instance.RegisterAlly(this);
        }
    }
    
    protected virtual void OnDestroy()
    {
        // AllyManager에서 제거
        if (AllyManager.Instance != null)
        {
            AllyManager.Instance.UnregisterAlly(this);
        }
    }
    
    protected virtual void Update()
    {
        if (isAttacking)
            return;
        
        // 타겟이 없으면 찾기
        if (targetEnemy == null)
        {
            FindNearestEnemy();
        }
        
        if (targetEnemy != null)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);
            
            if (distance <= attackRange)
            {
                Attack();
            }
            else if (distance <= detectionRange)
            {
                if (animator != null)
                {
                    animator.SetBool("isMoving", true);
                }
            }
            else
            {
                SetIdleAnimation();
            }
        }
        else
        {
            SetIdleAnimation();
        }
    }
    
    protected virtual void FixedUpdate()
    {
        // 이동은 FixedUpdate에서 처리
        if (targetEnemy != null && !isAttacking)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);
            
            if (distance > attackRange && distance <= detectionRange)
            {
                MoveTowardsEnemy();
            }
        }
    }
    
    /// <summary>
    /// 타겟 강제 설정 (엘리트 소환 시 사용)
    /// </summary>
    public virtual void SetTarget(Transform target)
    {
        targetEnemy = target;
        Debug.Log($"{gameObject.name}의 타겟이 {target.name}으로 변경됨");
    }
    
    /// <summary>
    /// 가장 가까운 적 찾기
    /// </summary>
    protected virtual void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");
        
        if (enemies.Length == 0)
        {
            targetEnemy = null;
            return;
        }
        
        float minDistance = Mathf.Infinity;
        Transform nearest = null;
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            
            if (distance < minDistance && distance <= detectionRange)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }
        
        targetEnemy = nearest;
    }
    
    /// <summary>
    /// 적을 향해 이동
    /// </summary>
    protected virtual void MoveTowardsEnemy()
    {
        if (targetEnemy == null) return;
        
        Vector2 direction = (targetEnemy.position - transform.position).normalized;
        
        // Rigidbody2D로 이동
        if (rb != null)
        {
            Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
        
        // 방향 전환
        FlipSprite(direction.x);
    }
    
    /// <summary>
    /// 스프라이트 좌우 반전
    /// </summary>
    protected virtual void FlipSprite(float directionX)
    {
        if (spriteRenderer != null)
        {
            if (directionX > 0)
                spriteRenderer.flipX = false;
            else if (directionX < 0)
                spriteRenderer.flipX = true;
        }
    }
    
    /// <summary>
    /// Idle 애니메이션 설정
    /// </summary>
    protected virtual void SetIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    /// <summary>
    /// 공격 (각 클래스에서 오버라이드)
    /// </summary>
    protected virtual void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;
        
        if (targetEnemy == null)
            return;
        
        if (isAttacking)
            return;
        
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // 멈추기
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // 타겟 방향 보기
        if (targetEnemy != null)
        {
            float directionX = targetEnemy.position.x - transform.position.x;
            FlipSprite(directionX);
        }
        
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("attack");
        }
        
        Invoke(nameof(EndAttack), attackAnimationDuration);
    }
    
    /// <summary>
    /// 공격 종료
    /// </summary>
    protected virtual void EndAttack()
    {
        isAttacking = false;
    }
    
    /// <summary>
    /// 데미지 받기
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 사망
    /// </summary>
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Gizmo 그리기
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}