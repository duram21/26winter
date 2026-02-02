using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// 대화 시스템 관리자 (싱글톤)
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("UI 참조")]
    public GameObject dialoguePanel; // 대화 패널
    public TMP_Text nameText; // 캐릭터 이름
    public TMP_Text dialogueText; // 대사 텍스트
    public Image characterImage; // 캐릭터 이미지 (옵션)
    public Button nextButton; // 다음 버튼
    
    [Header("타이핑 효과")]
    public bool useTypingEffect = true;
    public float typingSpeed = 0.05f; // 글자당 시간
    
    [Header("오디오")]
    public AudioSource audioSource; // 음성 재생용

    [Header("선택지 UI")]
    public DialogueChoiceUI choiceUI; // 선택지 UI
    public QuestListUI questListUI; // 퀘스트 목록 UI

    [Header("상태")]
    [SerializeField] private bool isDialogueActive = false;
    [SerializeField] private bool isTyping = false;
    
    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    
    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 시작 시 대화 패널 숨김
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    void Start()
    {
        // 다음 버튼 클릭 이벤트
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
    }
    
    void Update()
    {
        // 스페이스바로 다음 진행
        if (isDialogueActive && Input.GetKeyUp(KeyCode.Space))
        {
            OnNextButtonClicked();
        }
    }
    
    /// <summary>
    /// 대화 시작
    /// </summary>
    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null || dialogue.LineCount == 0)
        {
            Debug.LogWarning("대화 데이터가 없습니다!");
            return;
        }
        
        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;
        
        // UI 표시
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

                // 캐릭터 이름
        if (nameText != null)
        {
            nameText.text = dialogue.characterName;
        }
        
        // 캐릭터 이미지 (있다면)
        if (characterImage != null && dialogue.characterSprite != null)
        {
            characterImage.sprite = dialogue.characterSprite;
            characterImage.enabled = true;
        }
        else if (characterImage != null)
        {
            characterImage.enabled = false;
        }
        
        // 플레이어 이동 멈춤 (옵션)
        StopPlayerMovement();
        
        // 첫 대사 표시
        ShowCurrentLine();
        
        Debug.Log($"대화 시작: {dialogue.dialogueName}");
    }
    
    /// <summary>
    /// 현재 대사 표시
    /// </summary>
    void ShowCurrentLine()
    {
        DialogueLine line = currentDialogue.GetLine(currentLineIndex);
        
        if (line == null)
        {
            EndDialogue();
            return;
        }
        
        // 대사 텍스트
        if (useTypingEffect)
        {
            // 타이핑 효과로 표시
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(line.text));
        }
        else
        {
            // 즉시 표시
            if (dialogueText != null)
            {
                dialogueText.text = line.text;
            }
        }
        
        // 음성 재생 (있다면)
        if (audioSource != null && line.voiceClip != null)
        {
            audioSource.clip = line.voiceClip;
            audioSource.Play();
        }
    }
    
    /// <summary>
    /// 타이핑 효과
    /// </summary>
    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }
    
    /// <summary>
    /// 다음 버튼 클릭
    /// </summary>
    void OnNextButtonClicked()
    {
        if (!isDialogueActive) return;
        
        // 타이핑 중이면 스킵
        if (isTyping)
        {
            SkipTyping();
            return;
        }
        
        // 다음 대사로
        currentLineIndex++;
        
        if (currentLineIndex < currentDialogue.LineCount)
        {
            // 다음 대사 표시
            ShowCurrentLine();
        }
        else
        {
            // 대화 종료
            EndDialogue();
        }
    }
    
    /// <summary>
    /// 타이핑 스킵
    /// </summary>
    void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        // 현재 대사 전체 표시
        DialogueLine line = currentDialogue.GetLine(currentLineIndex);
        if (line != null && dialogueText != null)
        {
            dialogueText.text = line.text;
        }
        
        isTyping = false;
    }
    
    /// <summary>
    /// 대화 종료
    /// </summary>
    void EndDialogue()
    {
        
        // UI 숨김
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        // 플레이어 이동 재개 (옵션)
        ResumePlayerMovement();
        
        isDialogueActive = false;
        Debug.Log("대화 종료");
        
        // 다음 대화가 있으면 시작 (옵션)
        if (currentDialogue.nextDialogue != null)
        {
            StartDialogue(currentDialogue.nextDialogue);
        }
    }
    
    /// <summary>
    /// 플레이어 이동 멈춤 (옵션)
    /// </summary>
    void StopPlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Pawn pawn = player.GetComponent<Pawn>();
            if (pawn != null)
            {
                // Pawn 이동 비활성화 (Pawn 스크립트에 추가 필요)
                // pawn.canMove = false;
            }
        }
    }
    
    /// <summary>
    /// 플레이어 이동 재개 (옵션)
    /// </summary>
    void ResumePlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Pawn pawn = player.GetComponent<Pawn>();
            if (pawn != null)
            {
                // Pawn 이동 활성화
                // pawn.canMove = true;
            }
        }
    }
    
    /// <summary>
    /// 대화 진행 중인지
    /// </summary>
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    /// <summary>
    /// 선택지 UI가 활성화되어 있는지
    /// </summary>
    public bool IsChoiceActive()
    {
        if (choiceUI != null && choiceUI.IsActive())
            return true;
        if (questListUI != null && questListUI.IsActive())
            return true;
        return false;
    }
}