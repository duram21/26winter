using UnityEngine;

/// <summary>
/// 대화 가능한 NPC
/// </summary>
public class DialogueNPC : MonoBehaviour, IInteractable
{
    [Header("NPC 정보")]
    public string npcName = "NPC";
    
    [Header("대화 데이터")]
    public DialogueData dialogue;
    
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
        // 대화 시작
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            Debug.Log($"{npcName}와 대화 시작");
        }
        else
        {
            Debug.LogError("DialogueManager가 없습니다!");
        }
        
        // 프롬프트 숨김
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
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