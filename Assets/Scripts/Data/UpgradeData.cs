using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("기본 정보")]
    public string upgradeName = "검사 공격력";
    [TextArea(2, 4)]
    public string description = "+2 데미지";
    public Sprite icon;
    
    [Header("레벨 설정")]
    public int maxLevel = 10;
    public int[] costs = new int[] { 50, 75, 112, 168, 253, 379, 569, 854, 1281, 1922 };
    
    [Header("효과")]
    public float valuePerLevel = 2f;
    public string valueUnit = "데미지";
    
    [Header("타입")]
    public UpgradeType upgradeType;
    public string targetWarriorType = "Warrior";  // Warrior, Archer, Knight, Mage
    
    // 현재 레벨 가져오기
    public int GetCurrentLevel()
    {
        if (GameManager.Instance == null) return 0;
        
        switch (upgradeType)
        {
            // 아군 카운트
            case UpgradeType.WarriorCount:
                return GameManager.Instance.warriorCountLevel;
            case UpgradeType.ArcherCount:
                return GameManager.Instance.archerCountLevel;
            case UpgradeType.KnightCount:
                return GameManager.Instance.knightCountLevel;
            case UpgradeType.MageCount:
                return GameManager.Instance.mageCountLevel;
            
            // 아군 공격력
            case UpgradeType.AllyAttack:
                switch (targetWarriorType)
                {
                    case "Warrior": return GameManager.Instance.warriorAttackLevel;
                    case "Archer": return GameManager.Instance.archerAttackLevel;
                    case "Knight": return GameManager.Instance.knightAttackLevel;
                    case "Mage": return GameManager.Instance.mageAttackLevel;
                    default: return 0;
                }
            
            // 아군 이동속도
            case UpgradeType.AllySpeed:
                switch (targetWarriorType)
                {
                    case "Warrior": return GameManager.Instance.warriorSpeedLevel;
                    case "Archer": return GameManager.Instance.archerSpeedLevel;
                    case "Knight": return GameManager.Instance.knightSpeedLevel;
                    case "Mage": return GameManager.Instance.mageSpeedLevel;
                    default: return 0;
                }
            
            // 공통
            case UpgradeType.GoldBonus:
                return GameManager.Instance.goldBonusLevel;
            
            // 적 관련
            case UpgradeType.SpawnLevel:
                return GameManager.Instance.spawnLevelLevel;
            case UpgradeType.MaxEnemyCount:
                return GameManager.Instance.maxEnemyCountLevel;
            case UpgradeType.SpawnSpeed:
                return GameManager.Instance.spawnSpeedLevel;
            case UpgradeType.KillGoldBonus:
                return GameManager.Instance.killGoldBonusLevel;
            
            default:
                return 0;
        }
    }
    
    // 비용 계산
    public int GetCost(int level)
    {
        if (level < 0 || level >= costs.Length)
            return 9999;
        return costs[level];
    }
    
    // 다음 레벨 비용
    public int GetNextCost()
    {
        return GetCost(GetCurrentLevel());
    }
    
    // 최대 레벨 체크
    public bool IsMaxLevel()
    {
        return GetCurrentLevel() >= maxLevel;
    }
    
    // 업그레이드 가능 여부
    public bool CanUpgrade()
    {
        if (IsMaxLevel()) return false;
        if (GameManager.Instance == null) return false;
        
        return GameManager.Instance.currentGold >= GetNextCost();
    }
    
    // 업그레이드 실행!
    public bool TryUpgrade()
    {
        if (!CanUpgrade()) return false;
        
        int cost = GetNextCost();
        GameManager.Instance.currentGold -= cost;
        
        // 레벨 증가
        switch (upgradeType)
        {
            case UpgradeType.WarriorCount:
                GameManager.Instance.warriorCountLevel++;
                // 검사 1명 즉시 스폰!
                if (WarriorSpawner.Instance != null)
                {
                    WarriorSpawner.Instance.SpawnWarrior();
                }
                break;
            
            case UpgradeType.ArcherCount:
                GameManager.Instance.archerCountLevel++;
                // 궁수 1명 즉시 스폰!
                if (WarriorSpawner.Instance != null)
                {
                    WarriorSpawner.Instance.SpawnArcher();
                }
                break;
            
            case UpgradeType.KnightCount:
                GameManager.Instance.knightCountLevel++;
                // 기사 1명 즉시 스폰!
                if (WarriorSpawner.Instance != null)
                {
                    WarriorSpawner.Instance.SpawnKnight();
                }
                break;
            
            case UpgradeType.MageCount:
                GameManager.Instance.mageCountLevel++;
                // 마법사 1명 즉시 스폰!
                if (WarriorSpawner.Instance != null)
                {
                    WarriorSpawner.Instance.SpawnMage();
                }
                break;
            
            case UpgradeType.AllyAttack:
                switch (targetWarriorType)
                {
                    case "Warrior": GameManager.Instance.warriorAttackLevel++; break;
                    case "Archer": GameManager.Instance.archerAttackLevel++; break;
                    case "Knight": GameManager.Instance.knightAttackLevel++; break;
                    case "Mage": GameManager.Instance.mageAttackLevel++; break;
                }
                break;
            
            case UpgradeType.AllySpeed:
                switch (targetWarriorType)
                {
                    case "Warrior": GameManager.Instance.warriorSpeedLevel++; break;
                    case "Archer": GameManager.Instance.archerSpeedLevel++; break;
                    case "Knight": GameManager.Instance.knightSpeedLevel++; break;
                    case "Mage": GameManager.Instance.mageSpeedLevel++; break;
                }
                break;
            
            case UpgradeType.GoldBonus:
                GameManager.Instance.goldBonusLevel++;
                break;
            
            case UpgradeType.MaxEnemyCount:
                GameManager.Instance.maxEnemyCountLevel++;
                break;
            
            case UpgradeType.SpawnLevel:
                GameManager.Instance.spawnLevelLevel++;
                break;
            
            case UpgradeType.SpawnSpeed:
                GameManager.Instance.spawnSpeedLevel++;
                break;
            
            case UpgradeType.KillGoldBonus:
                GameManager.Instance.killGoldBonusLevel++;
                break;
        }
        
        return true;
    }
    
    // 레벨 정보 텍스트
    public string GetLevelText()
    {
        int current = GetCurrentLevel();
        return $"Lv.{current} -> Lv.{current + 1}";
    }
    
    // 설명 텍스트
    public string GetDescriptionText()
    {
        return $"+{valuePerLevel} {valueUnit}";
    }
}

public enum UpgradeType
{
    // 아군 카운트
    WarriorCount,
    ArcherCount,
    KnightCount,
    MageCount,
    
    // 아군 스탯
    AllyAttack,
    AllySpeed,
    
    // 공통
    GoldBonus,
    
    // 적 관련
    SpawnLevel,
    MaxEnemyCount,
    SpawnSpeed,
    KillGoldBonus
}