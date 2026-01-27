using UnityEngine;

public class Monk : BaseAlly
{
    [Header("몽크 고유 설정")]
    public GameObject healEffectPrefab;
    public int baseDamage = 8;
    
    protected override void Start()
    {
        base.Start(); // BaseAlly의 Start 호출
        
        // 몽크 고유 초기화
        attackRange = 5f;
        detectionRange = 10f;
        attackCooldown = 1.5f;
        attackAnimationDuration = 0.6f;
        
        // GameManager 업그레이드 적용
        if (GameManager.Instance != null)
        {
            float baseMoveSpeed = 3f;
            float upgradeBonus = GameManager.Instance.monkSpeedLevel * 0.5f;
            moveSpeed = baseMoveSpeed + upgradeBonus;
        }
    }
    
    /// <summary>
    /// 애니메이션 이벤트에서 호출 (힐 이펙트 생성)
    /// </summary>
    public void SpawnHealEffect()
    {
        if (targetEnemy == null || healEffectPrefab == null)
        {
            Debug.LogWarning("타겟이나 힐 이펙트가 없습니다!");
            return;
        }
        
        Vector3 effectPosition = targetEnemy.position + Vector3.up * 0.5f;
        GameObject effect = Instantiate(healEffectPrefab, effectPosition, Quaternion.identity);
        
        HealEffect healEffect = effect.GetComponent<HealEffect>();
        if (healEffect != null)
        {
            int totalDamage = baseDamage;
            if (GameManager.Instance != null)
            {
                int upgradeBonus = GameManager.Instance.monkAttackLevel * 2;
                totalDamage = baseDamage + upgradeBonus;
            }
            
            healEffect.Initialize(targetEnemy, totalDamage);
            Debug.Log($"힐 이펙트 소환! 타겟: {targetEnemy.name}, 데미지: {totalDamage}");
        }
    }
    
    public override void TakeDamage(int damage)
    {
        // 몽크는 데미지를 받지 않음 (무적)
        return;
    }
}