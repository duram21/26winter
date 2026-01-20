using UnityEngine;

public class Archer : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 3f;
    
    [Header("공격 설정")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float detectionRange = 12f;
    public float attackRange = 6f;
    public float attackCooldown = 1f;
    public float attackAnimationDuration = 0.5f;
    
    [Header("체력")]
    public int maxHealth = 100;
    public int currentHealth;
    
    private Transform targetEnemy;
    private float lastAttackTime;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;  // ← 추가!
    private bool isAttacking = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();  // ← 추가!
        currentHealth = maxHealth;
        
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0.5f, 0.2f, 0);
            firePoint = fp.transform;
        }
        
        if (GameManager.Instance != null)
        {
            float baseMoveSpeed = 3f;
            float upgradeBonus = GameManager.Instance.archerSpeedLevel * 0.5f;
            moveSpeed = baseMoveSpeed + upgradeBonus;
        }
    }
    
    void Update()
    {
        if (isAttacking)
            return;
        
        FindNearestEnemy();
        
        if (targetEnemy != null)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);
            
            if (distance <= attackRange)
            {
                Attack();
            }
            else if (distance <= detectionRange)
            {
                // MoveTowardsEnemy는 FixedUpdate에서 처리
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
    
    void FixedUpdate()  // ← 추가!
    {
        // 이동은 FixedUpdate에서!
        if (targetEnemy != null && !isAttacking)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);
            
            if (distance > attackRange && distance <= detectionRange)
            {
                MoveTowardsEnemy();
            }
        }
    }
    
    void FindNearestEnemy()
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
    
    void MoveTowardsEnemy()
    {
        if (targetEnemy == null) return;
        
        Vector2 direction = (targetEnemy.position - transform.position).normalized;
        
        // ========== Rigidbody2D로 이동! ==========
        if (rb != null)
        {
            Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
        // =========================================
        
        // 방향 전환
        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
    }
    
    void SetIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
        
        // ========== 멈추기! ==========
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        // =============================
    }
    
    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;
        
        if (targetEnemy == null)
            return;
        
        if (isAttacking)
            return;
        
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // ========== 멈추기! ==========
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        // =============================
        
        // 타겟 방향 보기
        if (targetEnemy != null)
        {
            if (targetEnemy.position.x > transform.position.x)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }
        
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("attack");
        }
        
        Invoke(nameof(EndAttack), attackAnimationDuration);
    }
    
    public void FireArrow()
    {
        if (targetEnemy == null || arrowPrefab == null)
            return;
        
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetTarget(targetEnemy);
            
            if (GameManager.Instance != null)
            {
                int baseAttack = 5;
                int upgradeBonus = GameManager.Instance.archerAttackLevel * 2;
                arrowScript.damage = baseAttack + upgradeBonus;
            }
        }
    }
    
    void EndAttack()
    {
        isAttacking = false;
    }
    
    public void TakeDamage(int damage)
    {
        return;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(firePoint.position, 0.15f);
        }
    }
}