using UnityEngine;

public class Warrior : BaseAlly
{
    [Header("전사 고유 설정")]
    public float attackDamage = 5f;
    
    protected override void Start()
    {
        base.Start(); // BaseAlly의 Start 호출
        
        // 전사 고유 초기화
        attackRange = 0.5f; // 근접 공격
        detectionRange = 20f;
    }
    
    protected override void Update()
    {
        // 공격 애니메이션 중에는 멈춤
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Warrior_Attack1_Black"))
        {
            if (animator != null) animator.SetBool("isMoving", false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }
        
        base.Update(); // BaseAlly의 Update 호출
    }
    
    protected override void Attack()
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
        
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("attack");
        }
        
        // 데미지 적용
        BaseMonster monster = targetEnemy.GetComponent<BaseMonster>();
        if (monster != null)
        {
            monster.TakeDamage(attackDamage);
        }
        
        // 공격 속도에 따라 쿨다운 설정
        float attackTimer = 1f / (attackCooldown > 0 ? 1f / attackCooldown : 1f);
        Invoke(nameof(EndAttack), attackTimer);
    }
    
    protected override void FlipSprite(float directionX)
    {
        // Warrior는 localScale로 방향 전환
        if (directionX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (directionX > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}