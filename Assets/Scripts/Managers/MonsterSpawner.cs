using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject monsterPrefab;  // 몬스터 프리팹
    public float spawnInterval = 2f;  // 스폰 간격 (초)
    public int maxMonsters = 10;      // 최대 몬스터 수
    
    [Header("스폰 범위")]
    public float spawnRangeX = 8f;    // X축 범위
    public float spawnRangeY = 4f;    // Y축 범위
    
    private float spawnTimer = 0f;
    public int currentMonsterCount = 0;
    
    void Update()
    {
        // 타이머 증가
        spawnTimer += Time.deltaTime;
        
        // 스폰 시간이 되고, 최대 수를 넘지 않았으면
        if (spawnTimer >= spawnInterval && currentMonsterCount < maxMonsters)
        {
            SpawnMonster();
            spawnTimer = 0f;  // 타이머 리셋
        }
    }
    
    void SpawnMonster()
    {
        // 랜덤 위치 계산
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector2 spawnPosition = new Vector2(randomX, randomY);
        
        // 몬스터 생성
        GameObject obj = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        Monster monster = obj.GetComponent<Monster>();
        
        // 몬스터가 죽을 때 카운트 감소하도록 설정
        if(monster)
        {
            monster.spawner = this;
        }

        currentMonsterCount++;
    }
    
    // 몬스터가 죽었을 때 호출될 함수
    public void OnMonsterDied()
    {
        currentMonsterCount--;
    }
    
    // 에디터에서 스폰 범위 보기 (개발할 때 편함)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRangeX * 2, spawnRangeY * 2, 0));
    }
}