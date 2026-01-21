using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    // ============================================
    // 골드 시스템
    // ============================================
    [Header("💰 골드")]
    public int currentGold = 0;
    
    // ============================================
    // 아군 해금 상태
    // ============================================
    [Header("🛡️ 아군 해금 (Unlock)")]
    public bool warriorUnlocked = true;    // 검사 (기본 제공)
    public bool archerUnlocked = false;    // 궁수 (500G)
    public bool knightUnlocked = false;    // 기사 (1,000G)
    public bool mageUnlocked = false;      // 마법사 (2,000G)
    public bool monkUnlocked = false;

    [Header("🛡️ 아군 해금 비용")]
    public int archerUnlockCost = 500;
    public int knightUnlockCost = 1000;
    public int mageUnlockCost = 2000;
    public int monkUnlockCost = 3000;
    
    // ============================================
    // 적 해금 상태
    // ============================================
    [Header("👹 적 해금 (Unlock)")]
    public bool slimeUnlocked = true;      // 슬라임 (기본 제공)
    public bool goblinUnlocked = false;    // 고블린 (300G)
    public bool orcUnlocked = false;       // 오크 (800G)
    public bool skeletonUnlocked = false;  // 해골전사 (1,500G)
    public bool demonUnlocked = false;     // 악마 (3,000G)
    
    [Header("👹 적 해금 비용")]
    public int goblinUnlockCost = 300;
    public int orcUnlockCost = 800;
    public int skeletonUnlockCost = 1500;
    public int demonUnlockCost = 3000;
    
    // ============================================
    // 아군 개별 업그레이드 레벨 (유닛별)
    // ============================================
    [Header("아군 카운트 레벨 (배치 가능 수)")]
    public int warriorCountLevel = 1;  // 검사 배치 가능 수
    public int archerCountLevel = 0;   // 궁수 배치 가능 수
    public int knightCountLevel = 0;   // 기사 배치 가능 수
    public int mageCountLevel = 0;     // 마법사 배치 가능 수
    public int monkCountLevel = 0;  // ← 추가!


    [Header("⬆️ 아군 공격력 레벨 (유닛별)")]
    public int warriorAttackLevel = 0;     // 검사 공격력 레벨
    public int archerAttackLevel = 0;      // 궁수 공격력 레벨
    public int knightAttackLevel = 0;      // 기사 공격력 레벨
    public int mageAttackLevel = 0;        // 마법사 공격력 레벨
    public int monkAttackLevel = 0;
    
    [Header("⬆️ 아군 이동속도 레벨 (유닛별)")]
    public int warriorSpeedLevel = 0;      // 검사 이동속도 레벨
    public int archerSpeedLevel = 0;       // 궁수 이동속도 레벨
    public int knightSpeedLevel = 0;       // 기사 이동속도 레벨
    public int mageSpeedLevel = 0;         // 마법사 이동속도 레벨
    public int monkSpeedLevel = 0;
    
    // ============================================
    // 공통 업그레이드 레벨
    // ============================================
    [Header("⬆️ 공통 업그레이드")]
    public int goldBonusLevel = 0;         // 골드 획득량 증가 (최대 Lv.15)
    
    [Header("⬆️ 적 관련 업그레이드")]
    public int spawnLevelLevel = 0;        // 적 소환 레벨
    public int maxEnemyCountLevel = 0;     // 최대 적 배치 수 (최대 Lv.10)
    public int spawnSpeedLevel = 0;        // 적 스폰 주기 감소 (최대 Lv.8)
    public int killGoldBonusLevel = 0;     // 적 처치 골드 보너스 (최대 Lv.10)
    
    // ============================================
    // 게임 통계
    // ============================================
    [Header("📊 통계")]
    public int totalKillCount = 0;         // 총 처치 수
    public float totalPlayTime = 0f;       // 총 플레이 시간 (초)
    
    // ============================================
    // 초기화 & 싱글톤
    // ============================================
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();  // 게임 시작 시 자동 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        // 플레이 시간 누적
        totalPlayTime += Time.deltaTime;
    }
    
    // ============================================
    // 골드 관리
    // ============================================
    public void AddGold(int amount)
    {
        // 골드 획득량 보너스 적용
        float bonus = GetGoldBonusMultiplier();
        int finalAmount = Mathf.RoundToInt(amount * bonus);
        
        currentGold += finalAmount;
        Debug.Log($"골드 획득: +{finalAmount}G (보너스: {(bonus-1)*100}%)");
    }
    
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            return true;
        }
        return false;
    }
    
    // ============================================
    // 골드 보너스 계산 (공통 업그레이드)
    // ============================================
    public float GetGoldBonusMultiplier()
    {
        // Lv.1 = +10%, Lv.2 = +20%, ... Lv.15 = +150%
        return 1f + (goldBonusLevel * 0.1f);
    }
    
    // ============================================
    // 적 스폰 설정 계산
    // ============================================
    public float GetSpawnInterval()
    {
        // 기본 3.0초에서 시작
        float baseInterval = 3.0f;
        
        // 레벨당 0.25초씩 감소
        float interval = baseInterval - (spawnSpeedLevel * 0.25f);
        
        // 최소 1.0초 유지
        return Mathf.Max(1.0f, interval);
    }
    
    public int GetMaxEnemyCount()
    {
        // 기본 5마리 + 레벨당 2마리
        return 5 + (maxEnemyCountLevel * 2);
    }
    
    public int GetKillGoldBonus()
    {
        // 레벨당 +2G 보너스
        return killGoldBonusLevel * 2;
    }
    
    // ============================================
    // 아군 해금
    // ============================================
    public bool UnlockWarrior(string warriorType)
    {
        switch (warriorType)
        {
            case "Archer":
                if (!archerUnlocked && SpendGold(archerUnlockCost))
                {
                    archerUnlocked = true;
                    Debug.Log("궁수 해금!");
                    SaveGame();
                    return true;
                }
                break;
                
            case "Knight":
                if (!knightUnlocked && SpendGold(knightUnlockCost))
                {
                    knightUnlocked = true;
                    Debug.Log("기사 해금!");
                    SaveGame();
                    return true;
                }
                break;
                
            case "Mage":
                if (!mageUnlocked && SpendGold(mageUnlockCost))
                {
                    mageUnlocked = true;
                    Debug.Log("마법사 해금!");
                    SaveGame();
                    return true;
                }
                break;
        }
        return false;
    }
    
    // ============================================
    // 적 해금
    // ============================================
    public bool UnlockEnemy(string enemyType)
    {
        switch (enemyType)
        {
            case "Goblin":
                if (!goblinUnlocked && SpendGold(goblinUnlockCost))
                {
                    goblinUnlocked = true;
                    Debug.Log("고블린 해금!");
                    SaveGame();
                    return true;
                }
                break;
                
            case "Orc":
                if (!orcUnlocked && SpendGold(orcUnlockCost))
                {
                    orcUnlocked = true;
                    Debug.Log("오크 해금!");
                    SaveGame();
                    return true;
                }
                break;
                
            case "Skeleton":
                if (!skeletonUnlocked && SpendGold(skeletonUnlockCost))
                {
                    skeletonUnlocked = true;
                    Debug.Log("해골전사 해금!");
                    SaveGame();
                    return true;
                }
                break;
                
            case "Demon":
                if (!demonUnlocked && SpendGold(demonUnlockCost))
                {
                    demonUnlocked = true;
                    Debug.Log("악마 해금!");
                    SaveGame();
                    return true;
                }
                break;
        }
        return false;
    }

    public int GetMaxWarriorCount()
    {
        return 1 + warriorCountLevel;  // 기본 1명 + 업그레이드
    }

    public int GetMaxArcherCount()
    {
        if (!archerUnlocked) return 0;
        return 1 + archerCountLevel;
    }

    public int GetMaxKnightCount()
    {
        if (!knightUnlocked) return 0;
        return 1 + knightCountLevel;
    }

    public int GetMaxMageCount()
    {
        if (!mageUnlocked) return 0;
        return 1 + mageCountLevel;
    }
    
    // ============================================
    // 아군 업그레이드 (공격력)
    // ============================================
    public bool UpgradeWarriorAttack(string warriorType)
    {
        int currentLevel = 0;
        
        switch (warriorType)
        {
            case "Warrior": currentLevel = warriorAttackLevel; break;
            case "Archer": currentLevel = archerAttackLevel; break;
            case "Knight": currentLevel = knightAttackLevel; break;
            case "Mage": currentLevel = mageAttackLevel; break;
        }
        
        // 최대 레벨 체크
        if (currentLevel >= 10) return false;
        
        // 비용 계산 (기획서 기준)
        int cost = GetAttackUpgradeCost(currentLevel);
        
        if (SpendGold(cost))
        {
            switch (warriorType)
            {
                case "Warrior": warriorAttackLevel++; break;
                case "Archer": archerAttackLevel++; break;
                case "Knight": knightAttackLevel++; break;
                case "Mage": mageAttackLevel++; break;
            }
            
            Debug.Log($"{warriorType} 공격력 업그레이드! Lv.{currentLevel + 1}");
            SaveGame();
            return true;
        }
        
        return false;
    }
    
    // ============================================
    // 아군 업그레이드 (이동속도)
    // ============================================
    public bool UpgradeWarriorSpeed(string warriorType)
    {
        int currentLevel = 0;
        
        switch (warriorType)
        {
            case "Warrior": currentLevel = warriorSpeedLevel; break;
            case "Archer": currentLevel = archerSpeedLevel; break;
            case "Knight": currentLevel = knightSpeedLevel; break;
            case "Mage": currentLevel = mageSpeedLevel; break;
        }
        
        if (currentLevel >= 10) return false;
        
        int cost = GetSpeedUpgradeCost(currentLevel);
        
        if (SpendGold(cost))
        {
            switch (warriorType)
            {
                case "Warrior": warriorSpeedLevel++; break;
                case "Archer": archerSpeedLevel++; break;
                case "Knight": knightSpeedLevel++; break;
                case "Mage": mageSpeedLevel++; break;
            }
            
            Debug.Log($"{warriorType} 이동속도 업그레이드! Lv.{currentLevel + 1}");
            SaveGame();
            return true;
        }
        
        return false;
    }
    
    // ============================================
    // 업그레이드 비용 계산 (기획서 기준)
    // ============================================
    int GetAttackUpgradeCost(int level)
    {
        int[] costs = { 50, 75, 112, 168, 253, 379, 569, 854, 1281, 1922 };
        return costs[Mathf.Min(level, costs.Length - 1)];
    }
    
    int GetSpeedUpgradeCost(int level)
    {
        int[] costs = { 40, 56, 78, 109, 153, 215, 301, 421, 590, 826 };
        return costs[Mathf.Min(level, costs.Length - 1)];
    }
    
    // ============================================
    // 공통 업그레이드
    // ============================================
    public bool UpgradeGoldBonus()
    {
        if (goldBonusLevel >= 15) return false;
        
        int[] costs = { 100, 160, 256, 409, 655, 1048, 1677, 2684, 4294, 
                       6871, 10995, 17592, 28147, 45035, 72057 };
        int cost = costs[goldBonusLevel];
        
        if (SpendGold(cost))
        {
            goldBonusLevel++;
            Debug.Log($"골드 획득량 업그레이드! Lv.{goldBonusLevel} (+{goldBonusLevel * 10}%)");
            SaveGame();
            return true;
        }
        
        return false;
    }
    
    public bool UpgradeMaxEnemyCount()
    {
        if (maxEnemyCountLevel >= 10) return false;
        
        int[] costs = { 200, 360, 648, 1166, 2099, 3779, 6802, 12244, 22039, 39671 };
        int cost = costs[maxEnemyCountLevel];
        
        if (SpendGold(cost))
        {
            maxEnemyCountLevel++;
            Debug.Log($"최대 적 배치 수 업그레이드! Lv.{maxEnemyCountLevel} ({GetMaxEnemyCount()}마리)");
            SaveGame();
            return true;
        }
        
        return false;
    }
    
    public bool UpgradeSpawnSpeed()
    {
        if (spawnSpeedLevel >= 8) return false;
        
        int[] costs = { 150, 255, 433, 736, 1252, 2129, 3620, 6155 };
        int cost = costs[spawnSpeedLevel];
        
        if (SpendGold(cost))
        {
            spawnSpeedLevel++;
            Debug.Log($"스폰 속도 업그레이드! Lv.{spawnSpeedLevel} ({GetSpawnInterval()}초)");
            SaveGame();
            return true;
        }
        
        return false;
    }
    
    public bool UpgradeKillGoldBonus()
    {
        if (killGoldBonusLevel >= 10) return false;
        
        int[] costs = { 80, 120, 180, 270, 405, 607, 911, 1366, 2050, 3075 };
        int cost = costs[killGoldBonusLevel];
        
        if (SpendGold(cost))
        {
            killGoldBonusLevel++;
            Debug.Log($"처치 골드 보너스 업그레이드! Lv.{killGoldBonusLevel} (+{GetKillGoldBonus()}G)");
            SaveGame();
            return true;
        }
        
        return false;
    }

        // ========== 해금 함수들 (추가!) ==========
    public bool UnlockArcher()
    {
        int cost = 500;
        
        // 이미 해금되어 있으면 실패
        if (archerUnlocked)
        {
            Debug.Log("궁수는 이미 해금되었습니다!");
            return false;
        }
        
        // 골드가 부족하면 실패
        if (currentGold < cost)
        {
            Debug.Log($"골드가 부족합니다! (필요: {cost}G, 보유: {currentGold}G)");
            return false;
        }
        
        // 해금 성공!
        currentGold -= cost;
        archerUnlocked = true;
        Debug.Log($"궁수 해금 성공! (잔여 골드: {currentGold}G)");
        return true;
    }
    
    public bool UnlockMonk()
    {
        int cost = 700;
        
        if (monkUnlocked)
        {
            Debug.Log("Monk는 이미 해금되었습니다!");
            return false;
        }
        
        if (currentGold < cost)
        {
            Debug.Log($"골드가 부족합니다! (필요: {cost}G, 보유: {currentGold}G)");
            return false;
        }
        
        currentGold -= cost;
        monkUnlocked = true;
        Debug.Log($"Monk 해금 성공! (잔여 골드: {currentGold}G)");
        return true;
    }
    
    public bool UnlockKnight()
    {
        int cost = 1000;
        
        if (knightUnlocked)
        {
            Debug.Log("기사는 이미 해금되었습니다!");
            return false;
        }
        
        if (currentGold < cost)
        {
            Debug.Log($"골드가 부족합니다! (필요: {cost}G, 보유: {currentGold}G)");
            return false;
        }
        
        currentGold -= cost;
        knightUnlocked = true;
        Debug.Log($"기사 해금 성공! (잔여 골드: {currentGold}G)");
        return true;
    }
    
    public bool UnlockMage()
    {
        int cost = 1500;
        
        if (mageUnlocked)
        {
            Debug.Log("마법사는 이미 해금되었습니다!");
            return false;
        }
        
        if (currentGold < cost)
        {
            Debug.Log($"골드가 부족합니다! (필요: {cost}G, 보유: {currentGold}G)");
            return false;
        }
        
        currentGold -= cost;
        mageUnlocked = true;
        Debug.Log($"마법사 해금 성공! (잔여 골드: {currentGold}G)");
        return true;
    }
    // ========================================
    
    // ============================================
    // 통계
    // ============================================
    public void AddKillCount()
    {
        totalKillCount++;
    }
    
    public string GetPlayTimeString()
    {
        int minutes = Mathf.FloorToInt(totalPlayTime / 60f);
        int seconds = Mathf.FloorToInt(totalPlayTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
    
    // ============================================
    // 저장/불러오기
    // ============================================
    public void SaveGame()
    {
        PlayerPrefs.SetInt("CurrentGold", currentGold);
        
        // 아군 해금
        PlayerPrefs.SetInt("WarriorUnlocked", warriorUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("ArcherUnlocked", archerUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("KnightUnlocked", knightUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("MageUnlocked", mageUnlocked ? 1 : 0);
        
        // 적 해금
        PlayerPrefs.SetInt("SlimeUnlocked", slimeUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("GoblinUnlocked", goblinUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("OrcUnlocked", orcUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("SkeletonUnlocked", skeletonUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("DemonUnlocked", demonUnlocked ? 1 : 0);
        
        // 아군 업그레이드
        PlayerPrefs.SetInt("WarriorAttackLevel", warriorAttackLevel);
        PlayerPrefs.SetInt("ArcherAttackLevel", archerAttackLevel);
        PlayerPrefs.SetInt("KnightAttackLevel", knightAttackLevel);
        PlayerPrefs.SetInt("MageAttackLevel", mageAttackLevel);
        
        PlayerPrefs.SetInt("WarriorSpeedLevel", warriorSpeedLevel);
        PlayerPrefs.SetInt("ArcherSpeedLevel", archerSpeedLevel);
        PlayerPrefs.SetInt("KnightSpeedLevel", knightSpeedLevel);
        PlayerPrefs.SetInt("MageSpeedLevel", mageSpeedLevel);
        
        // 공통 업그레이드
        PlayerPrefs.SetInt("GoldBonusLevel", goldBonusLevel);
        PlayerPrefs.SetInt("MaxEnemyCountLevel", maxEnemyCountLevel);
        PlayerPrefs.SetInt("SpawnSpeedLevel", spawnSpeedLevel);
        PlayerPrefs.SetInt("KillGoldBonusLevel", killGoldBonusLevel);
        
        // 통계
        PlayerPrefs.SetInt("TotalKillCount", totalKillCount);
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        
        PlayerPrefs.Save();
        Debug.Log("게임 저장 완료!");
    }


    
    public void LoadGame()
    {
        currentGold = PlayerPrefs.GetInt("CurrentGold", 0);
        
        // 아군 해금
        warriorUnlocked = PlayerPrefs.GetInt("WarriorUnlocked", 1) == 1;
        archerUnlocked = PlayerPrefs.GetInt("ArcherUnlocked", 0) == 1;
        knightUnlocked = PlayerPrefs.GetInt("KnightUnlocked", 0) == 1;
        mageUnlocked = PlayerPrefs.GetInt("MageUnlocked", 0) == 1;
        
        // 적 해금
        slimeUnlocked = PlayerPrefs.GetInt("SlimeUnlocked", 1) == 1;
        goblinUnlocked = PlayerPrefs.GetInt("GoblinUnlocked", 0) == 1;
        orcUnlocked = PlayerPrefs.GetInt("OrcUnlocked", 0) == 1;
        skeletonUnlocked = PlayerPrefs.GetInt("SkeletonUnlocked", 0) == 1;
        demonUnlocked = PlayerPrefs.GetInt("DemonUnlocked", 0) == 1;
        
        // 아군 업그레이드
        warriorAttackLevel = PlayerPrefs.GetInt("WarriorAttackLevel", 0);
        archerAttackLevel = PlayerPrefs.GetInt("ArcherAttackLevel", 0);
        knightAttackLevel = PlayerPrefs.GetInt("KnightAttackLevel", 0);
        mageAttackLevel = PlayerPrefs.GetInt("MageAttackLevel", 0);
        
        warriorSpeedLevel = PlayerPrefs.GetInt("WarriorSpeedLevel", 0);
        archerSpeedLevel = PlayerPrefs.GetInt("ArcherSpeedLevel", 0);
        knightSpeedLevel = PlayerPrefs.GetInt("KnightSpeedLevel", 0);
        mageSpeedLevel = PlayerPrefs.GetInt("MageSpeedLevel", 0);
        
        // 공통 업그레이드
        goldBonusLevel = PlayerPrefs.GetInt("GoldBonusLevel", 0);
        maxEnemyCountLevel = PlayerPrefs.GetInt("MaxEnemyCountLevel", 0);
        spawnSpeedLevel = PlayerPrefs.GetInt("SpawnSpeedLevel", 0);
        killGoldBonusLevel = PlayerPrefs.GetInt("KillGoldBonusLevel", 0);
        
        // 통계
        totalKillCount = PlayerPrefs.GetInt("TotalKillCount", 0);
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
        
        Debug.Log("게임 불러오기 완료!");
    }
    
    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        LoadGame();
        Debug.Log("게임 초기화 완료!");
    }
}