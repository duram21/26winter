using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 퀘스트 매니저 - 싱글톤
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    
    [Header("활성 퀘스트")]
    public List<QuestData> activeQuests = new List<QuestData>();
    
    [Header("완료된 퀘스트")]
    public List<QuestData> completedQuests = new List<QuestData>();
    
    [Header("디버그")]
    public bool showDebugLogs = true;
    
    // 이벤트
    public event Action<QuestData> OnQuestAccepted;
    public event Action<QuestData> OnQuestUpdated;
    public event Action<QuestData> OnQuestCompleted;
    public event Action<QuestData> OnQuestClaimed;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // 게임 이벤트 구독
        SubscribeToGameEvents();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromGameEvents();
    }
    
    /// <summary>
    /// 게임 이벤트 구독
    /// </summary>
    void SubscribeToGameEvents()
    {
        // 예시 이벤트들 (실제 게임에 맞게 수정)
        // GameEvents.OnEnemyKilled += OnEnemyKilled;
        // GameEvents.OnItemCollected += OnItemCollected;
        // GameEvents.OnNPCTalked += OnNPCTalked;
        // GameEvents.OnLocationDiscovered += OnLocationDiscovered;
    }
    
    void UnsubscribeFromGameEvents()
    {
        // GameEvents.OnEnemyKilled -= OnEnemyKilled;
        // GameEvents.OnItemCollected -= OnItemCollected;
        // GameEvents.OnNPCTalked -= OnNPCTalked;
        // GameEvents.OnLocationDiscovered -= OnLocationDiscovered;
    }
    
    /// <summary>
    /// 퀘스트 수락 가능한지 확인
    /// </summary>
    public bool CanAcceptQuest(QuestData quest)
    {
        // 이미 진행 중이거나 완료한 퀘스트
        if (activeQuests.Contains(quest) || completedQuests.Contains(quest))
        {
            DebugLog($"이미 진행 중이거나 완료한 퀘스트: {quest.questTitle}");
            return false;
        }
        
        // 레벨 확인 (GameManager 있을 때)
        // if (GameManager.Instance.PlayerLevel < quest.requiredLevel)
        // {
        //     DebugLog($"레벨 부족: {quest.requiredLevel} 필요");
        //     return false;
        // }
        
        // 선행 퀘스트 확인
        if (quest.prerequisiteQuest != null)
        {
            if (!IsQuestCompleted(quest.prerequisiteQuest))
            {
                DebugLog($"선행 퀘스트 미완료: {quest.prerequisiteQuest.questTitle}");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 퀘스트 수락
    /// </summary>
    public void AcceptQuest(QuestData quest)
    {
        if (!CanAcceptQuest(quest))
        {
            DebugLog($"퀘스트 수락 불가: {quest.questTitle}");
            return;
        }
        
        quest.StartQuest();
        activeQuests.Add(quest);
        
        DebugLog($"퀘스트 수락: {quest.questTitle}");
        OnQuestAccepted?.Invoke(quest);
    }
    
    /// <summary>
    /// 적 처치 시
    /// </summary>
    public void OnEnemyKilled(string enemyId, int count = 1)
    {
        DebugLog($"적 처치: {enemyId} x{count}");
        
        foreach (var quest in activeQuests)
        {
            var objective = quest.FindObjective(QuestObjective.ObjectiveType.Kill, enemyId);
            
            if (objective != null && !objective.isCompleted)
            {
                objective.UpdateProgress(count);
                DebugLog($"목표 업데이트: {objective.GetProgressText()}");
                
                OnQuestUpdated?.Invoke(quest);
                CheckQuestCompletion(quest);
            }
        }
    }
    
    /// <summary>
    /// 아이템 수집 시
    /// </summary>
    public void OnItemCollected(string itemId, int count = 1)
    {
        DebugLog($"아이템 수집: {itemId} x{count}");
        
        foreach (var quest in activeQuests)
        {
            var objective = quest.FindObjective(QuestObjective.ObjectiveType.Collect, itemId);
            
            if (objective != null && !objective.isCompleted)
            {
                objective.UpdateProgress(count);
                DebugLog($"목표 업데이트: {objective.GetProgressText()}");
                
                OnQuestUpdated?.Invoke(quest);
                CheckQuestCompletion(quest);
            }
        }
    }
    
    /// <summary>
    /// NPC와 대화 시
    /// </summary>
    public void OnNPCTalked(string npcId)
    {
        DebugLog($"NPC 대화: {npcId}");
        
        foreach (var quest in activeQuests)
        {
            var objective = quest.FindObjective(QuestObjective.ObjectiveType.Talk, npcId);
            
            if (objective != null && !objective.isCompleted)
            {
                objective.UpdateProgress(1);
                DebugLog($"목표 업데이트: {objective.GetProgressText()}");
                
                OnQuestUpdated?.Invoke(quest);
                CheckQuestCompletion(quest);
            }
        }
    }
    
    /// <summary>
    /// 장소 발견 시
    /// </summary>
    public void OnLocationDiscovered(string locationId)
    {
        DebugLog($"장소 발견: {locationId}");
        
        foreach (var quest in activeQuests)
        {
            var objective = quest.FindObjective(QuestObjective.ObjectiveType.Explore, locationId);
            
            if (objective != null && !objective.isCompleted)
            {
                objective.UpdateProgress(1);
                DebugLog($"목표 업데이트: {objective.GetProgressText()}");
                
                OnQuestUpdated?.Invoke(quest);
                CheckQuestCompletion(quest);
            }
        }
    }
    
    /// <summary>
    /// 퀘스트 완료 확인
    /// </summary>
    void CheckQuestCompletion(QuestData quest)
    {
        if (quest.AreAllObjectivesComplete())
        {
            quest.CompleteQuest();
            DebugLog($"퀘스트 완료: {quest.questTitle}");
            
            OnQuestCompleted?.Invoke(quest);
        }
    }
    
    /// <summary>
    /// 보상 받기
    /// </summary>
    public void ClaimReward(QuestData quest)
    {
        if (quest.state != QuestData.QuestState.Completed)
        {
            DebugLog("완료되지 않은 퀘스트!");
            return;
        }
        
        // 골드 지급 (GameManager 있을 때)
        // GameManager.Instance.AddGold(quest.reward.gold);
        
        // 경험치 지급
        // GameManager.Instance.AddExp(quest.reward.experience);
        
        // 아이템 지급 (Inventory 있을 때)
        // foreach (var item in quest.reward.items)
        // {
        //     Inventory.Instance.AddItem(item);
        // }
        
        DebugLog($"보상 지급: 골드 {quest.reward.gold}, 경험치 {quest.reward.experience}");
        
        // 상태 변경
        quest.state = QuestData.QuestState.Claimed;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        
        OnQuestClaimed?.Invoke(quest);
        
        // 다음 퀘스트 자동 시작 (체인)
        if (quest.nextQuest != null)
        {
            AcceptQuest(quest.nextQuest);
        }
    }
    
    /// <summary>
    /// 퀘스트 완료 여부
    /// </summary>
    public bool IsQuestCompleted(QuestData quest)
    {
        return completedQuests.Contains(quest);
    }
    
    /// <summary>
    /// 특정 ID의 활성 퀘스트 찾기
    /// </summary>
    public QuestData GetActiveQuest(string questId)
    {
        return activeQuests.Find(q => q.questId == questId);
    }
    
    void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[QuestManager] {message}");
        }
    }
}