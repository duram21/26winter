using UnityEngine;

/// <summary>
/// Pawn 캐릭터 - 지속 상호작용 시스템
/// 나무 등의 오브젝트와 지속적으로 상호작용 가능
/// </summary>
public class Pawn : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;


    [Header("Animator Override Controllers")]
    [Tooltip("ItemType enum 순서대로 배열에 할당 (None, Axe, Meat, Gold, Knife, Wood, Hammer, Pickaxe)")]
    public AnimatorOverrideController[] itemControllers;
    
    [Header("상호작용 설정")]
    public float interactionDuration = 0f;
    public bool autoDetectAnimationLength = true;
    
    [Header("지속 상호작용 설정")]
    public float interactionRange = 2f; // 상호작용 범위
    public LayerMask interactableLayer; // 상호작용 가능한 레이어
    
    [Header("현재 상태")]
    public ItemType currentItem = ItemType.None;
    
    // 컴포넌트
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // 입력
    private Vector2 moveInput;
    private bool isMoving = false;
    
    // 상호작용
    private bool isInteracting = false;
    private float interactTimer = 0f;
    
    // 지속 상호작용 대상
    private IInteractable currentInteractTarget; // 현재 상호작용 중인 대상
    private GameObject nearbyInteractable; // 근처의 상호작용 가능한 오브젝트    
    public enum ItemType
    {
        None = 0,
        Axe = 1,
        Meat = 2,
        Gold = 3,
        Knife = 4,
        Wood = 5,
        Hammer = 6,
        Pickaxe = 7
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        ValidateItemControllers();
        ChangeItem(currentItem);
    }
    
    void ValidateItemControllers()
    {
        int requiredCount = System.Enum.GetValues(typeof(ItemType)).Length;
        
        if (itemControllers == null || itemControllers.Length != requiredCount)
        {
            Debug.LogWarning($"itemControllers 배열 크기가 맞지 않습니다! 필요: {requiredCount}, 현재: {(itemControllers?.Length ?? 0)}");
        }
    }
    
    void Update()
    {
        if(IsInDialogue())
        {
            return;
        }

        // 지속 상호작용 중일 때
        if (isInteracting && currentInteractTarget != null)
        {
            // 범위 체크
            if (!IsInInteractionRange(currentInteractTarget))
            {
                // 범위 벗어남 - 상호작용 취소
                CancelInteraction();
                Debug.Log("범위를 벗어나 상호작용이 취소되었습니다!");
                return;
            }
            
            // 상호작용 타이머 업데이트
            interactTimer += Time.deltaTime;
            
            // 상호작용 진행도 업데이트
            float progress = interactTimer / interactionDuration;
            currentInteractTarget.OnInteractionProgress(progress);
            
            // 상호작용 완료
            if (interactTimer >= interactionDuration)
            {
                CompleteInteraction();
            }
            
            return; // 상호작용 중에는 다른 입력 무시
        }
        
        // 근처 상호작용 가능한 오브젝트 찾기
        FindNearbyInteractable();
        
        // 입력 받기
        HandleInput();
        
        // 애니메이션 업데이트
        UpdateAnimation();
        
        // 아이템 변경 테스트
        HandleItemSwitch();
        
        // 상호작용 입력
        HandleInteraction();
    }
    
    void FixedUpdate()
    {
        if(IsInDialogue())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
    
        if (isInteracting)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        Move();
    }
    
    /// <summary>
    /// 근처의 상호작용 가능한 오브젝트 찾기
    /// </summary>
    void FindNearbyInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
        
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        
        foreach (Collider2D col in colliders)
        {
            // IInteractable 인터페이스가 있는지 확인
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract(this))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = col.gameObject;
                }
            }
        }
        
        nearbyInteractable = closest;
    }
    
    /// <summary>
    /// 상호작용 범위 내에 있는지 확인
    /// </summary>
    bool IsInInteractionRange(IInteractable target)
    {
        if (target == null) return false;
        
        MonoBehaviour targetMono = target as MonoBehaviour;
        if (targetMono == null) return false;
        
        float distance = Vector2.Distance(transform.position, targetMono.transform.position);
        return distance <= interactionRange;
    }
    
    void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        moveInput = new Vector2(horizontal, vertical).normalized;
        isMoving = moveInput.magnitude > 0;
    }
    
    void Move()
    {
        rb.linearVelocity = moveInput * moveSpeed;
        
        if (moveInput.x > 0)
            spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;
    }
    
    void UpdateAnimation()
    {
        if (animator == null) return;
        
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isInteracting", isInteracting);
    }
    
    public void ChangeItem(ItemType newItem)
    {
        currentItem = newItem;
        
        int index = (int)newItem;
        
        if (itemControllers == null || index < 0 || index >= itemControllers.Length)
        {
            Debug.LogWarning($"잘못된 아이템 인덱스: {index}");
            return;
        }   
        
        if (itemControllers[index] == null)
        {
            Debug.LogWarning($"{newItem}에 대한 Override Controller가 설정되지 않았습니다! (인덱스: {index})");
            return;
        }
        
        animator.runtimeAnimatorController = itemControllers[index];
        Debug.Log($"아이템 변경: {newItem} (인덱스: {index})");
        
        // 상호 작용 시간을 위해 추가. (나무 베는건 오래, 양 죽이는건 짧게..)
        if(newItem == ItemType.Knife)
        {
            interactionDuration = 0.3f;
        }
        if(newItem == ItemType.Axe)
        {
            interactionDuration = 1.0f;
        }
    }
    
    void HandleInteraction()
    {
        // 스페이스바로 상호작용 시작/유지
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isInteracting && nearbyInteractable != null)
            {
                TryStartInteraction();
            }
        }
        // 스페이스바를 떼면 상호작용 취소
       /* else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isInteracting)
            {
                CancelInteraction();
                Debug.Log("상호작용이 취소되었습니다!");
            }
        } */
    }
    
    void TryStartInteraction()
    {
        if (nearbyInteractable == null) return;
        
        IInteractable interactable = nearbyInteractable.GetComponent<IInteractable>();
        if (interactable == null) return;
        
        if (!interactable.CanInteract(this))
        {
            Debug.Log("상호작용할 수 없습니다!");
            return;
        }
        
        StartInteraction(interactable);
    }
    
    void StartInteraction(IInteractable target)
    {
        if (isInteracting) return;
        
        isInteracting = true;
        interactTimer = 0f;
        currentInteractTarget = target;
        
        rb.linearVelocity = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetBool("isInteracting", true);
        }
        
        if (autoDetectAnimationLength)
        {
            interactionDuration = GetInteractAnimationLength();
        }
        
        // 상호작용 시작 알림
        currentInteractTarget.OnInteractionStart(this);
        
        Debug.Log($"{currentItem}로 상호작용 시작! (지속시간: {interactionDuration}초)");
    }
    
    void CompleteInteraction()
    {
        if (!isInteracting || currentInteractTarget == null) return;
        
        // 상호작용 완료 알림
        currentInteractTarget.OnInteractionComplete(this);
        
        Debug.Log("상호작용 완료!");
        
        EndInteraction();
    }
    
    /// <summary>
    /// 상호작용 취소
    /// </summary>
    public void CancelInteraction()
    {
        if (!isInteracting) return;
        
        // 취소 알림
        if (currentInteractTarget != null)
        {
            currentInteractTarget.OnInteractionCancel(this);
        }
        
        EndInteraction();
    }
    
    void EndInteraction()
    {
        isInteracting = false;
        interactTimer = 0f;
        currentInteractTarget = null;
        
        if (animator != null)
        {
            animator.SetBool("isInteracting", false);
        }
    }
    
    float GetInteractAnimationLength()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return 1f;
        
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains("Interact") || clip.name.Contains("interact"))
            {
                return clip.length;
            }
        }
        
        return 1f;
    }


    bool IsInDialogue()
    {
        // DialogueManager가 있고 대화 활성화 중이면 true
        if (DialogueManager.Instance != null)
        {
            return DialogueManager.Instance.IsDialogueActive();
        }
        return false;
    }
    
    /// <summary>
    /// 애니메이션 이벤트: 타격 시점
    /// </summary>
    public void OnInteractHit()
    {
        // 지속 상호작용에서는 타격 이벤트 대신 진행도로 처리
        Debug.Log($"{currentItem} 상호작용 중...");
    }
    
    void HandleItemSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) ChangeItem(ItemType.None);
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeItem(ItemType.Axe);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeItem(ItemType.Meat);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeItem(ItemType.Gold);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeItem(ItemType.Knife);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeItem(ItemType.Wood);
        if (Input.GetKeyDown(KeyCode.Alpha6)) ChangeItem(ItemType.Hammer);
        if (Input.GetKeyDown(KeyCode.Alpha7)) ChangeItem(ItemType.Pickaxe);
    }
    
    public ItemType GetCurrentItem() => currentItem;
    public bool IsInteracting() => isInteracting;
    public bool IsMoving() => isMoving;
    public float GetInteractionProgress() => isInteracting ? (interactTimer / interactionDuration) : 0f;
    
    void OnDrawGizmosSelected()
    {
        // 상호작용 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        // 근처 상호작용 가능한 오브젝트 표시
        if (nearbyInteractable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, nearbyInteractable.transform.position);
        }
    }
}