using UnityEngine;

/// <summary>
/// 상호작용 가능한 오브젝트 인터페이스
/// 나무, 바위, NPC 등 모든 상호작용 가능한 오브젝트가 구현
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 상호작용 가능한지 확인
    /// </summary>
    /// <param name="pawn">상호작용 시도하는 Pawn</param>
    /// <returns>상호작용 가능 여부</returns>
    bool CanInteract(Pawn pawn);
    
    /// <summary>
    /// 상호작용 시작
    /// </summary>
    /// <param name="pawn">상호작용하는 Pawn</param>
    void OnInteractionStart(Pawn pawn);
    
    /// <summary>
    /// 상호작용 진행 중 (0~1)
    /// </summary>
    /// <param name="progress">진행도 (0 = 시작, 1 = 완료)</param>
    void OnInteractionProgress(float progress);
    
    /// <summary>
    /// 상호작용 완료
    /// </summary>
    /// <param name="pawn">상호작용한 Pawn</param>
    void OnInteractionComplete(Pawn pawn);
    
    /// <summary>
    /// 상호작용 취소 (범위 벗어남, 키 뗌 등)
    /// </summary>
    /// <param name="pawn">상호작용하던 Pawn</param>
    void OnInteractionCancel(Pawn pawn);
}