using UnityEngine;

/// <summary>
/// 일반 몬스터 클래스
/// 죽으면 엘리트 토큰 카운트 증가
/// </summary>
public class Monster : BaseMonster
{
    /// <summary>
    /// 사망 시 엘리트 토큰 카운트 증가
    /// </summary>
    protected override void Die()
    {
        if (isDead) return;
        
        // 엘리트 토큰 시스템에 알림
        if (EliteMonsterSpawner.Instance != null)
        {
            EliteMonsterSpawner.Instance.OnMonsterKilled();
        }
        
        // 기본 사망 처리 (골드, 스포너 알림 등)
        base.Die();
    }
}