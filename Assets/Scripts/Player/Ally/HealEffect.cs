using UnityEngine;

public class HealEffect : MonoBehaviour
{
    [Header("설정")]
    public float lifetime = 1f;  // 이펙트 지속 시간
    
    private Transform target;
    private int damage;
    private bool hasDealtDamage = false;
    
    void Start()
    {
        // lifetime 후 자동 삭제
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Transform targetTransform, int damageAmount)
    {
        target = targetTransform;
        damage = damageAmount;
        
        // 즉시 데미지 (또는 애니메이션 중간에 주고 싶으면 Invoke 사용)
        DealDamage();
    }
    
    void DealDamage()
    {
        if (hasDealtDamage) return;
        
        if (target == null)
        {
            Debug.LogWarning("타겟이 없습니다!");
            return;
        }
        
        BaseMonster monster = target.GetComponent<BaseMonster>();
        if (monster != null)
        {
            monster.TakeDamage(damage);
            hasDealtDamage = true;
            Debug.Log($"힐 이펙트가 {target.name}에게 {damage} 데미지!");
        }
    }
    
    void Update()
    {
        // 타겟이 움직이면 이펙트도 따라가기 (선택사항)
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 0.5f;
        }
    }
}