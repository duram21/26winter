using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 대화 가능한 NPC
/// </summary>
public class DialogueNPC : MonoBehaviour, IInteractable
{
    [Header("NPC 정보")]
    public string npcName = "NPC";
    
    [Header("대화 데이터")]
    public DialogueData dialogue;
    
    [Header("퀘스트 데이터")]
    public List<QuestData> quests = new List<QuestData>();

    [Header("완료 대화")]
    public DialogueData questCompleteDialogue; // 퀘스트 완료 시 대화

    [Header("상호작용 설정")]
    public Pawn.ItemType requiredTool = Pawn.ItemType.None; // 필요한 도구 (없음)
    public float interactionDuration = 0f; // 즉시 상호작용
    
    [Header("시각적 피드백")]
    public GameObject interactionPrompt; // "E를 눌러 대화" UI
    
    private bool playerInRange = false;
    
    void Start()
    {
        // 상호작용 프롬프트 숨김
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void Update()
    {
        // 플레이어가 범위 내에 있을 때 프롬프트 표시
        UpdateInteractionPrompt();
    }
    
    void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null) return;
        
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        // 거리 계산
        float distance = Vector2.Distance(transform.position, player.transform.position);
        playerInRange = distance <= 2f; // 2m 범위
        
        // 프롬프트 표시/숨김
        bool showPrompt = playerInRange && !DialogueManager.Instance.IsDialogueActive();
        interactionPrompt.SetActive(showPrompt);
    }
    
    // IInteractable 구현
    
    public bool CanInteract(Pawn pawn)
    {
        // 대화 데이터가 있으면 상호작용 가능
        if (dialogue == null)
        {
            Debug.LogWarning($"{npcName}: 대화 데이터가 없습니다!");
            return false;
        }
        
        // 이미 대화 중이면 불가
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            return false;
        }
        
        // 도구 확인 (필요한 경우)
        if (requiredTool != Pawn.ItemType.None && pawn.GetCurrentItem() != requiredTool)
        {
            return false;
        }
        
        return true;
    }
    
    public void OnInteractionStart(Pawn pawn)
    {
        // 프롬프트 숨김
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // 메이플스토리 스타일 퀘스트 시스템
        HandleNPCInteraction();
    }

    /// <summary>
    /// NPC 상호작용 처리 (메이플스토리 스타일)
    /// </summary>
    void HandleNPCInteraction()
    {
        if (QuestManager.Instance == null || DialogueManager.Instance == null)
        {
            Debug.LogError("QuestManager 또는 DialogueManager가 없습니다!");
            return;
        }

        // 1단계: 완료 가능한 퀘스트 확인 (진행 중 + 목표 완료)
        QuestData completableQuest = GetCompletableQuest();
        if (completableQuest != null)
        {
            // 완료 가능한 퀘스트가 있으면 완료 대화
            ShowQuestCompleteDialogue(completableQuest);
            return;
        }

        // 2단계: 수락 가능한 퀘스트 확인
        List<QuestData> availableQuests = GetAvailableQuests();
        if (availableQuests.Count > 0)
        {
            // 수락 가능한 퀘스트가 있으면 선택지 표시
            ShowQuestChoices(availableQuests);
            return;
        }

        // 3단계: 퀘스트가 없으면 일반 대화
        if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            Debug.Log($"{npcName}와 대화 시작");
        }
        else
        {
            Debug.LogWarning($"{npcName}: 대화 데이터가 없습니다!");
        }
    }

    /// <summary>
    /// 완료 가능한 퀘스트 가져오기
    /// </summary>
    QuestData GetCompletableQuest()
    {
        foreach (var quest in quests)
        {
            // 진행 중이고 목표를 모두 완료한 퀘스트
            if (quest.state == QuestData.QuestState.InProgress && quest.AreAllObjectivesComplete())
            {
                return quest;
            }
        }
        return null;
    }

    /// <summary>
    /// 수락 가능한 퀘스트 목록 가져오기
    /// </summary>
    List<QuestData> GetAvailableQuests()
    {
        List<QuestData> available = new List<QuestData>();

        foreach (var quest in quests)
        {
            if (QuestManager.Instance.CanAcceptQuest(quest))
            {
                available.Add(quest);
            }
        }

        return available;
    }

    /// <summary>
    /// 퀘스트 완료 대화 표시
    /// </summary>
    void ShowQuestCompleteDialogue(QuestData quest)
    {
        // 퀘스트 완료 대화가 있으면 표시
        if (questCompleteDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(questCompleteDialogue);
        }
        else if (quest.lines.Count > 0)
        {
            // 퀘스트 자체에 대화가 있으면 표시
            // 임시로 일반 대화 표시
            if (dialogue != null)
            {
                DialogueManager.Instance.StartDialogue(dialogue);
            }
        }

        // 보상 지급
        QuestManager.Instance.ClaimReward(quest);
        Debug.Log($"퀘스트 완료: {quest.questTitle}");
    }

    /// <summary>
    /// 퀘스트 선택지 표시 (메이플스토리 스타일)
    /// </summary>
    void ShowQuestChoices(List<QuestData> availableQuests)
    {
        if (DialogueManager.Instance.choiceUI == null)
        {
            Debug.LogWarning("DialogueChoiceUI가 설정되지 않았습니다!");
            return;
        }

        List<string> choices = new List<string>
        {
            "퀘스트",
            "기타"
        };

        DialogueManager.Instance.choiceUI.ShowChoices(
            $"{npcName}와(과) 대화",
            choices,
            (choiceIndex) => OnChoiceSelected(choiceIndex, availableQuests)
        );
    }

    /// <summary>
    /// 선택지 클릭 시
    /// </summary>
    void OnChoiceSelected(int choiceIndex, List<QuestData> availableQuests)
    {
        switch (choiceIndex)
        {
            case 0: // 퀘스트
                ShowQuestList(availableQuests);
                break;
            case 1: // 기타 (일반 대화)
                if (dialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(dialogue);
                }
                break;
        }
    }

    /// <summary>
    /// 퀘스트 목록 표시
    /// </summary>
    void ShowQuestList(List<QuestData> availableQuests)
    {
        if (DialogueManager.Instance.questListUI == null)
        {
            Debug.LogWarning("QuestListUI가 설정되지 않았습니다!");
            return;
        }

        DialogueManager.Instance.questListUI.ShowQuestList(
            availableQuests,
            OnQuestSelected
        );
    }

    /// <summary>
    /// 퀘스트 선택 시
    /// </summary>
    void OnQuestSelected(QuestData quest)
    {
        // 퀘스트 수락
        QuestManager.Instance.AcceptQuest(quest);

        // 퀘스트 대화 표시 (있으면)
        if (quest.lines.Count > 0)
        {
            // QuestData의 대화를 임시 DialogueData로 변환하여 표시
            // 또는 퀘스트 수락 대화를 별도로 만들어서 표시
            Debug.Log($"퀘스트 수락: {quest.questTitle}");
        }
        else if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
    
    public void OnInteractionProgress(float progress)
    {
        // 대화는 즉시 시작되므로 사용 안 함
    }
    
    public void OnInteractionComplete(Pawn pawn)
    {
        // 대화는 즉시 시작되므로 사용 안 함
    }
    
    public void OnInteractionCancel(Pawn pawn)
    {
        // 대화는 취소 불가
    }
    
    public float GetInteractionDuration()
    {
        return interactionDuration; // 0 = 즉시
    }
    
    void OnDrawGizmosSelected()
    {
        // 상호작용 범위 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}