using UnityEngine;

public class Archer : BaseAlly
{
    [Header("궁수 고유 설정")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    
    protected override void Start()
    {
        base.Start(); // BaseAlly의 Start 호출
        
        // 궁수 고유 초기화
        attackRange = 6f;
        detectionRange = 12f;
        attackCooldown = 1f;
        attackAnimationDuration = 0.5f;
        
        // FirePoint 자동 생성
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0.5f, 0.2f, 0);
            firePoint = fp.transform;
        }
        
        // GameManager 업그레이드 적용
        if (GameManager.Instance != null)
        {
            float baseMoveSpeed = 3f;
            float upgradeBonus = GameManager.Instance.archerSpeedLevel * 0.5f;
            moveSpeed = baseMoveSpeed + upgradeBonus;
        }
    }
    
    /// <summary>
    /// 애니메이션 이벤트에서 호출 (화살 발사)
    /// </summary>
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
    
    public override void TakeDamage(int damage)
    {
        // 궁수는 데미지를 받지 않음 (무적)
        return;
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(firePoint.position, 0.15f);
        }
    }
}