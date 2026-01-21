using UnityEngine;

public class Warrior : MonoBehaviour
{
    [Header("스탯")]
    public float moveSpeed = 3f;
    public float attackDamage = 5f;
    public float attackSpeed = 1f;
    public float attackRange = 0.5f;
    
    [Header("설정")]
    public float detectionRange = 20f;
    
    public Monster currentTarget;
    private float attackTimer = 1f;
    private Animator anim;
    private Rigidbody2D rb;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Warrior_Attack1_Black")) 
        {
            anim.SetBool("isMoving", false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return; 
        }
        
        if (currentTarget == null)
        {
            FindNearestMonster();
            
            if (currentTarget == null)
            {
                anim.SetBool("isMoving", false);
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
        
        if (currentTarget != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
            
            if (distanceToTarget <= attackRange)
            {
                AttackTarget();
            }
            else
            {
                MoveToTarget();
            }
        }
        
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }
    
    void FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        if (monsters.Length == 0)
        {
            currentTarget = null;
            return;
        }
        
        float nearestDistance = Mathf.Infinity;
        Monster nearestMonster = null;
        
        foreach (GameObject monsterObj in monsters)
        {
            float distance = Vector2.Distance(transform.position, monsterObj.transform.position);
            if (distance < nearestDistance && distance <= detectionRange)
            {
                nearestDistance = distance;
                nearestMonster = monsterObj.GetComponent<Monster>();
            }
        }
        currentTarget = nearestMonster;
    }
    
    void MoveToTarget()
    {
        if (currentTarget == null) return;
        
        anim.SetBool("isMoving", true);
        
        Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
        
        // velocity 사용 (부드러움!)
        if (rb != null)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        
        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
    
    void AttackTarget()
    {
        if (currentTarget == null) return;
        
        anim.SetBool("isMoving", false);
        
        // 멈추기
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        if (attackTimer <= 0)
        {
            anim.SetTrigger("attack");
            currentTarget.TakeDamage(attackDamage);
            attackTimer = 1f / attackSpeed;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}