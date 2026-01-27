using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("설정")]
    public float speed = 10f;
    public int damage = 5;
    public float lifetime = 3f;
    public float rotationSpeed = 300f;
    
    private Rigidbody2D rb;
    private Transform target;
    private bool hasHit = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }
    
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }
    
    void Update()
    {
        if (hasHit) return;
        
        if (target == null)
        {
            // 타겟이 죽었으면 직진
            rb.linearVelocity = transform.right * speed;
            return;
        }
        
        // 타겟 추적
        Vector2 direction = (target.position - transform.position).normalized;
        Vector2 currentDirection = rb.linearVelocity.normalized;
        Vector2 newDirection = Vector2.Lerp(currentDirection, direction, rotationSpeed * Time.deltaTime / speed);
        
        rb.linearVelocity = newDirection * speed;
        
        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        
        // 타겟한테만 맞음!
        if (target != null && collision.transform == target)
        {
            hasHit = true;
            
            BaseMonster monster = collision.GetComponent<BaseMonster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                Debug.Log($"화살이 타겟 {target.name}에게 {damage} 데미지!");
            }
            
            Destroy(gameObject);
        }
        // 다른 적은 관통! (아무것도 안 함)
    }
}