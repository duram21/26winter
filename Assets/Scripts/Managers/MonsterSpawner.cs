using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;  // ← 추가! (싱글톤)
    
    [Header("스폰 설정")]
    public GameObject monsterPrefab;
    
    [Header("기본 밸런스 (업그레이드 전)")]
    public float baseSpawnInterval = 2f;    // 기본 스폰 간격
    public int baseMaxMonsters = 10;        // 기본 최대 수
    
    [Header("스폰 범위")]
    public float spawnRangeX = 8f;
    public float spawnRangeY = 4f;
    
    private float spawnTimer = 0f;
    public int currentMonsterCount = 0;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        spawnTimer += Time.deltaTime;
        
        // ========== 실시간 계산! ==========
        float currentSpawnInterval = GetCurrentSpawnInterval();
        int currentMaxMonsters = GetCurrentMaxMonsters();
        // ==================================
        
        if (spawnTimer >= currentSpawnInterval && currentMonsterCount < currentMaxMonsters)
        {
            SpawnMonster();
            spawnTimer = 0f;
        }
    }
    
    // ========== 실시간 계산 함수들 (추가!) ==========
    
    public int GetCurrentMaxMonsters()
    {
        if (GameManager.Instance == null)
            return baseMaxMonsters;
        
        // 기본값 + (레벨 × 증가량)
        int bonus = GameManager.Instance.maxEnemyCountLevel * 2;  // 레벨당 +2마리
        return baseMaxMonsters + bonus;
    }
    
    public float GetCurrentSpawnInterval()
    {
        if (GameManager.Instance == null)
            return baseSpawnInterval;
        
        // 기본값 - (레벨 × 감소량)
        float reduction = GameManager.Instance.spawnSpeedLevel * 0.2f;  // 레벨당 -0.2초
        float result = baseSpawnInterval - reduction;
        
        // 최소값 제한 (너무 빨라지지 않도록)
        return Mathf.Max(0.5f, result);
    }
    
    // ===============================================
    
    void SpawnMonster()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector2 spawnPosition = new Vector2(randomX, randomY);
        
        GameObject obj = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        Monster monster = obj.GetComponent<Monster>();
        
        if(monster)
        {
            monster.spawner = this;
        }

        currentMonsterCount++;
        
        // 디버그 로그 (선택사항)
        Debug.Log($"몬스터 스폰! (현재: {currentMonsterCount}/{GetCurrentMaxMonsters()}마리, 간격: {GetCurrentSpawnInterval():F1}초)");
    }
    
    public void OnMonsterDied()
    {
        currentMonsterCount--;
    }
    
    // UI나 디버그용 (선택사항)
    public string GetStatusText()
    {
        return $"몬스터: {currentMonsterCount}/{GetCurrentMaxMonsters()}마리\n" +
               $"스폰 간격: {GetCurrentSpawnInterval():F1}초";
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRangeX * 2, spawnRangeY * 2, 0));
    }
}