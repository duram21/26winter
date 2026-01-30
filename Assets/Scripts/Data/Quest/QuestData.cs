using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 퀘스트 목표 하나
/// </summary>
[System.Serializable]
public class QuestObjective
{
    [Header("목표 정보")]
    public string objectiveId; // "kill_wolf", "collect_wood"
    public string description; // "늑대 5마리 처치"
    
    [Header("타입")]
    public ObjectiveType type;
    public enum ObjectiveType
    {
        Kill,       // 처치
        Collect,    // 수집
        Talk,       // 대화
        Explore     // 탐험/도달
    }
    
    [Header("대상")]
    public string targetId; // "Wolf", "Wood", "Merchant"
    
    [Header("개수")]
    public int requiredAmount = 1;
    [HideInInspector] public int currentAmount = 0;
    
    [Header("상태")]
    [HideInInspector] public bool isCompleted = false;
    
    /// <summary>
    /// 목표 진행도 업데이트
    /// </summary>
    public void UpdateProgress(int amount)
    {
        currentAmount += amount;
        
        if (currentAmount >= requiredAmount)
        {
            currentAmount = requiredAmount;
            isCompleted = true;
        }
    }
    
    /// <summary>
    /// 진행도 텍스트
    /// </summary>
    public string GetProgressText()
    {
        return $"{description} ({currentAmount}/{requiredAmount})";
    }
}

/// <summary>
/// 퀘스트 보상
/// </summary>
[System.Serializable]
public class QuestReward
{
    public int gold = 0;
    public int experience = 0;
    // public List<ItemData> items; // 아이템 시스템 있을 때
}

/// <summary>
/// 퀘스트 데이터 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("기본 정보")]
    public string questId = "quest_001";
    public string questTitle = "새 퀘스트";
    
    [TextArea(3, 10)]
    public string description = "퀘스트 설명";

    [Header("대사 목록")]
    public List<DialogueLine> lines = new List<DialogueLine>();

    [Header("목표")]
    public List<QuestObjective> objectives = new List<QuestObjective>();
    
    [Header("보상")]
    public QuestReward reward = new QuestReward();
    
    [Header("조건")]
    public int requiredLevel = 1;
    public QuestData prerequisiteQuest; // 선행 퀘스트
    
    [Header("다음 퀘스트 (체인)")]
    public QuestData nextQuest; // 완료 후 자동 시작
    
    [Header("상태 (런타임)")]
    [HideInInspector] public QuestState state = QuestState.NotStarted;
    
    public enum QuestState
    {
        NotStarted,    // 시작 전
        InProgress,    // 진행 중
        Completed,     // 완료 (보상 안 받음)
        Claimed        // 보상 받음
    }
    
    /// <summary>
    /// 퀘스트 시작 (목표 초기화)
    /// </summary>
    public void StartQuest()
    {
        state = QuestState.InProgress;
        
        foreach (var objective in objectives)
        {
            objective.currentAmount = 0;
            objective.isCompleted = false;
        }
    }
    
    /// <summary>
    /// 모든 목표 완료되었는지
    /// </summary>
    public bool AreAllObjectivesComplete()
    {
        foreach (var objective in objectives)
        {
            if (!objective.isCompleted)
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// 퀘스트 완료 처리
    /// </summary>
    public void CompleteQuest()
    {
        if (AreAllObjectivesComplete())
        {
            state = QuestState.Completed;
        }
    }
    
    /// <summary>
    /// 특정 타입의 목표 찾기
    /// </summary>
    public QuestObjective FindObjective(QuestObjective.ObjectiveType type, string targetId)
    {
        foreach (var objective in objectives)
        {
            if (objective.type == type && objective.targetId == targetId)
            {
                return objective;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 진행도 퍼센트
    /// </summary>
    public float GetProgressPercentage()
    {
        if (objectives.Count == 0) return 0f;
        
        int completedCount = 0;
        foreach (var objective in objectives)
        {
            if (objective.isCompleted)
                completedCount++;
        }
        
        return (float)completedCount / objectives.Count * 100f;
    }
}