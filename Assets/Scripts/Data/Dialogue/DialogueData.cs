using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 대사 한 줄 데이터
/// </summary>
[System.Serializable]
public class DialogueLine
{
    [Header("대사")]
    [TextArea(2, 5)]
    public string text = "안녕하세요!";
    
    [Header("음성 (옵션)")]
    public AudioClip voiceClip;
    
    [Header("지연 시간 (옵션)")]
    public float delayAfter = 0f; // 이 대사 후 대기 시간
}

/// <summary>
/// 대화 데이터 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("캐릭터")]
    public string characterName = "NPC";
    public Sprite characterSprite; // 캐릭터 이미지 (옵션)

    [Header("대화 정보")]
    public string dialogueName = "새 대화";
    
    [Header("대사 목록")]
    public List<DialogueLine> lines = new List<DialogueLine>();
    
    [Header("다음 대화 (옵션)")]
    [Tooltip("이 대화가 끝나면 자동으로 시작할 대화")]
    public DialogueData nextDialogue;
    
    /// <summary>
    /// 대사 개수
    /// </summary>
    public int LineCount => lines.Count;
    
    /// <summary>
    /// 특정 인덱스의 대사 가져오기
    /// </summary>
    public DialogueLine GetLine(int index)
    {
        if (index >= 0 && index < lines.Count)
        {
            return lines[index];
        }
        return null;
    }
}