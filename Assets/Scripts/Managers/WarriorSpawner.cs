using UnityEngine;
using System.Collections.Generic;

public class WarriorSpawner : MonoBehaviour
{
    public static WarriorSpawner Instance;
    
    [Header("아군 프리팹")]
    public GameObject warriorPrefab;
    public GameObject archerPrefab;
    public GameObject knightPrefab;
    public GameObject magePrefab;
    public GameObject monkPrefab;

    
    [Header("스폰 위치")]
    public Transform spawnPoint;
    public float spawnRadius = 2f;
    
    [Header("부모 오브젝트")]
    public Transform allyParent;  // ← 추가!
    
    // 스폰된 유닛들
    private List<GameObject> warriors = new List<GameObject>();
    private List<GameObject> archers = new List<GameObject>();
    private List<GameObject> knights = new List<GameObject>();
    private List<GameObject> mages = new List<GameObject>();
    private List<GameObject> monks = new List<GameObject>();

    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    void Start()
    {
        SpawnWarrior();
    }
    
    public void SpawnWarrior()
    {
        if (warriorPrefab == null)
        {
            Debug.LogWarning("Warrior Prefab이 없습니다!");
            return;
        }
        
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject warrior = Instantiate(warriorPrefab, spawnPos, Quaternion.identity);
        warrior.name = $"Warrior_{warriors.Count + 1}";
        
        // 부모 설정! ← 핵심
        if (allyParent != null)
        {
            warrior.transform.SetParent(allyParent);
        }
        
        warriors.Add(warrior);
        
        Debug.Log($"검사 스폰! 현재 총 {warriors.Count}명");
        AllyManager.Instance.RegisterAlly(warrior.GetComponent<BaseAlly>());
    }
    
    public void SpawnArcher()
    {
        if (archerPrefab == null)
        {
            Debug.LogWarning("Archer Prefab이 없습니다!");
            return;
        }
        
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject archer = Instantiate(archerPrefab, spawnPos, Quaternion.identity);
        archer.name = $"Archer_{archers.Count + 1}";
        
        // 부모 설정!
        if (allyParent != null)
        {
            archer.transform.SetParent(allyParent);
        }
        
        archers.Add(archer);
        
        Debug.Log($"궁수 스폰! 현재 총 {archers.Count}명");
        AllyManager.Instance.RegisterAlly(archer.GetComponent<BaseAlly>());

    }
    
    public void SpawnKnight()
    {
        if (knightPrefab == null)
        {
            Debug.LogWarning("Knight Prefab이 없습니다!");
            return;
        }
        
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject knight = Instantiate(knightPrefab, spawnPos, Quaternion.identity);
        knight.name = $"Knight_{knights.Count + 1}";
        
        // 부모 설정!
        if (allyParent != null)
        {
            knight.transform.SetParent(allyParent);
        }
        
        knights.Add(knight);
        
        Debug.Log($"기사 스폰! 현재 총 {knights.Count}명");
    }
    
    public void SpawnMage()
    {
        if (magePrefab == null)
        {
            Debug.LogWarning("Mage Prefab이 없습니다!");
            return;
        }
        
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject mage = Instantiate(magePrefab, spawnPos, Quaternion.identity);
        mage.name = $"Mage_{mages.Count + 1}";
        
        // 부모 설정!
        if (allyParent != null)
        {
            mage.transform.SetParent(allyParent);
        }
        
        mages.Add(mage);
        
        Debug.Log($"마법사 스폰! 현재 총 {mages.Count}명");
    }

    public void SpawnMonk()
    {
        if (monkPrefab == null)
        {
            Debug.LogWarning("Monk Prefab이 없습니다!");
            return;
        }
        
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject monk = Instantiate(monkPrefab, spawnPos, Quaternion.identity);
        monk.name = $"Monk_{monks.Count + 1}";
        
        // 부모 설정!
        if (allyParent != null)
        {
            monk.transform.SetParent(allyParent);
        }
        
        monks.Add(monk);
        
        Debug.Log($"마법사 스폰! 현재 총 {monks.Count}명");
        AllyManager.Instance.RegisterAlly(monk.GetComponent<BaseAlly>());

    }
    
    Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoint == null)
        {
            return new Vector3(-8f, -3f, 0f);
        }
        
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        return spawnPoint.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
    }
    

}