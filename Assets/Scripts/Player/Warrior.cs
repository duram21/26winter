using UnityEngine;

public class Warrior : MonoBehaviour
{
    [Header("스탯")]
    public float moveSpeed = 3f;           // 이동 속도
    public float attackDamage = 5f;        // 공격력
    public float attackSpeed = 1f;         // 공격 속도 (초당 공격 횟수)
    public float attackRange = 0.5f;       // 공격 범위
    
    [Header("설정")]
    public float detectionRange = 20f;     // 몬스터 탐지 범위
    
    // 내부 변수
    public Monster currentTarget;          // 현재 타겟
    private float attackTimer = 1f;        // 공격 쿨타임 타이머
    private Animator anim;                 // 애니메이터 변수 추가

    void Start()
    {
        // 시작할 때 애니메이터 컴포넌트를 가져옵니다.
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Warrior_Attack1_Black")) 
        {
            // 공격 중일 때는 이동 애니메이션도 꺼줍니다.
            anim.SetBool("isMoving", false);
            return; 
        }
        // 1. 타겟이 없거나 죽었으면 새로운 타겟 찾기
        if (currentTarget == null)
        {
            FindNearestMonster();
            // 타겟이 아예 없을 때는 이동 애니메이션을 끕니다.
            if (currentTarget == null) anim.SetBool("isMoving", false);
        }
        
        // 2. 타겟이 있으면 행동
        if (currentTarget != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
            
            // 공격 범위 안이면 공격
            if (distanceToTarget <= attackRange)
            {
                AttackTarget();
            }
            // 공격 범위 밖이면 이동
            else
            {
                MoveToTarget();
            }
        }
        
        // 3. 공격 타이머 감소
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
        
        // [애니메이션] 이동 중이므로 isMoving을 true로 설정
        anim.SetBool("isMoving", true);

        Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        
        // 방향에 따라 스프라이트 뒤집기
        if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
    }
    
    void AttackTarget()
    {
        if (currentTarget == null) return;

        // [애니메이션] 공격 사거리 안이므로 이동 애니메이션은 끕니다.
        anim.SetBool("isMoving", false);
        
        if (attackTimer <= 0)
        {
            // [애니메이션] 공격 트리거 실행
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