using UnityEngine;

public class EliteMonster : BaseMonster
{
    [Header("보상 설정")]
    public GameObject rewardPrefab; // 드롭할 보상 프리팹들
    public int minRewardCount = 3; // 최소 보상 개수
    public int maxRewardCount = 7; // 최대 보상 개수
    public float dropRadius = 2f; // 보상이 흩어지는 반경
    public float dropForce = 3f; // 보상이 튀어나가는 힘



    protected override void Die()
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

        DropRewards();
    }

    void DropRewards()
    {
        if (rewardPrefab == null)
        {
            Debug.LogWarning("보상 프리팹이 설정되지 않았습니다!");
            return;
        }
        
        Vector3 dropPosition = transform.position;
        
        // 랜덤 보상 개수 결정
        int rewardCount = Random.Range(minRewardCount, maxRewardCount + 1);
        
        Debug.Log($"엘리트 몬스터가 {rewardCount}개의 보상을 드롭합니다!");
        
        // 보상 생성
        for (int i = 0; i < rewardCount; i++)
        {
            // 랜덤 위치 (원형으로 흩어짐)
            Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
            Vector3 spawnPosition = dropPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // 보상 생성
            GameObject reward = Instantiate(rewardPrefab, spawnPosition, Quaternion.identity);
            
            // 물리 효과 (튀어나가는 효과)
            Rigidbody2D rb = reward.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
            }
        }
    }
}