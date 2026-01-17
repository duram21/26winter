using UnityEngine;

public class Monster : MonoBehaviour
{
    [HideInInspector]
    public MonsterSpawner spawner;
    
    [Header("스탯")]
    public float maxHealth = 10f;
    public int goldReward = 5;
    public float moveSpeed = 0.5f;        // 몬스터 이동 속도 (느리게 설정)
    
    [Header("상태")]
    public bool isDead = false;           // 죽었는지 확인 (Warrior가 체크용)
    private float currentHealth;
    
    private Vector2 moveDirection;        // 이동 방향
    private float decisionTimer;          // 방향 전환 타이머
    private Animator anim;                // 애니메이션 (있을 경우 대비)

    void Start()
    {
        currentHealth = maxHealth;
        gameObject.tag = "Monster";
        anim = GetComponent<Animator>();
        
        // 처음 시작할 때 랜덤한 방향 설정
        SetRandomDirection();
    }
    
    void Update()
    {
        if (isDead) return;

        // 1. 조금씩 이동시키기
        MoveSlowly();

        // 2. 주기적으로 방향 바꾸기 (너무 한쪽으로만 가지 않게)
        decisionTimer += Time.deltaTime;
        if (decisionTimer >= 3f) // 3초마다 방향 전환
        {
            SetRandomDirection();
            decisionTimer = 0;
        }
    }

    void MoveSlowly()
    {
        // 실제로 위치를 옮기는 코드
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 이동 애니메이션이 있다면 파라미터 전달 (기존 Warrior와 동일하게 isMoving 사용 가정)
        if (anim != null) anim.SetBool("isMoving", true);

        // 방향에 따라 좌우 반전
        if (moveDirection.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (moveDirection.x > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    void SetRandomDirection()
    {
        // -1에서 1 사이의 랜덤한 방향 생성
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 피격! 남은 체력: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
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
        
        Destroy(gameObject, 0.1f); // 아주 잠깐의 여유를 두고 파괴
    }
}