using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 아군 유닛을 관리하는 매니저
/// 상점에서 구매 시 자동 등록
/// 엘리트 소환 시 모든 아군 타겟 변경
/// </summary>
public class AllyManager : MonoBehaviour
{
    public static AllyManager Instance;
    
    // 모든 아군 리스트
    public List<BaseAlly> allAllies = new List<BaseAlly>();
    
    void Awake()
    {
        // 싱글톤 설정
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
    
    /// <summary>
    /// 아군 등록 (각 아군이 Start에서 자동 호출)
    /// </summary>
    public void RegisterAlly(BaseAlly ally)
    {
        if (!allAllies.Contains(ally))
        {
            allAllies.Add(ally);
            Debug.Log($"{ally.gameObject.name} 등록됨. 현재 아군 수: {allAllies.Count}");
        }
    }
    
    /// <summary>
    /// 아군 제거 (죽거나 삭제될 때 호출)
    /// </summary>
    public void UnregisterAlly(BaseAlly ally)
    {
        if (allAllies.Contains(ally))
        {
            allAllies.Remove(ally);
            Debug.Log($"{ally.gameObject.name} 제거됨. 현재 아군 수: {allAllies.Count}");
        }
    }
    
    /// <summary>
    /// 모든 아군의 타겟을 특정 적으로 변경 (엘리트 소환 시 사용!)
    /// </summary>
    public void SetAllTargets(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("타겟이 null입니다!");
            return;
        }
        Debug.LogWarning("타겟을 바꿉니단");
        
        int count = 0;
        foreach (BaseAlly ally in allAllies)
        {
            if (ally != null)
            {
                ally.SetTarget(target);
                count++;
            }
        }
        
        Debug.Log($"총 {count}명의 아군이 {target.name}을(를) 타겟팅합니다!");
    }
    
    /// <summary>
    /// 현재 아군 수 반환
    /// </summary>
    public int GetAllyCount()
    {
        return allAllies.Count;
    }
    
    /// <summary>
    /// 특정 타입의 아군 수 반환
    /// </summary>
    public int GetAllyCount<T>() where T : BaseAlly
    {
        int count = 0;
        foreach (BaseAlly ally in allAllies)
        {
            if (ally is T)
                count++;
        }
        return count;
    }
    
    /// <summary>
    /// 모든 아군 리스트 반환 (읽기 전용)
    /// </summary>
    public List<BaseAlly> GetAllAllies()
    {
        return new List<BaseAlly>(allAllies);
    }
    
    /// <summary>
    /// null 체크 및 정리 (옵션)
    /// </summary>
    public void CleanupNullAllies()
    {
        allAllies.RemoveAll(ally => ally == null);
    }
}