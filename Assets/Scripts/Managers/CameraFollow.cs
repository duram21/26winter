using UnityEngine;

/// <summary>
/// Pixel Perfect Camera Follow - 매끄러운 버전
/// Time.smoothDeltaTime 사용으로 더 부드러운 이동
/// </summary>
public class CameraFollow_Smooth : MonoBehaviour
{
    [Header("타겟")]
    public Transform target;
    
    [Header("오프셋")]
    public Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("부드러운 추적")]
    public bool smoothFollow = true;
    [Range(1f, 20f)]
    public float smoothSpeed = 8f; // 5~8 권장
    
    [Header("Pixel Perfect 설정")]
    [Tooltip("스프라이트의 Pixels Per Unit과 동일하게 설정")]
    public int pixelsPerUnit = 16;
    [Tooltip("픽셀 단위로 위치 스냅")]
    public bool pixelSnapping = true;
    
    [Header("고급 설정")]
    [Tooltip("더 부드러운 이동을 위해 smoothDeltaTime 사용")]
    public bool useSmoothDeltaTime = true;
    [Tooltip("매우 작은 이동 무시 (떨림 방지)")]
    public float deadZone = 0.001f;
    
    [Header("범위 제한")]
    public bool limitCameraBounds = false;
    public Vector2 minBounds = new Vector2(-50, -50);
    public Vector2 maxBounds = new Vector2(50, 50);
    
    private float unitsPerPixel;
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        // 타겟 자동 찾기
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("[Camera] 타겟 설정: " + player.name);
            }
        }
        
        // Pixels Per Unit 계산
        unitsPerPixel = 1f / pixelsPerUnit;
        
        // 시작 위치를 타겟으로
        if (target != null)
        {
            Vector3 startPos = target.position + offset;
            if (pixelSnapping)
            {
                startPos = SnapToPixel(startPos);
            }
            transform.position = startPos;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // 1. 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;
        
        // 2. 부드러운 이동
        Vector3 newPosition;
        if (smoothFollow)
        {
            // smoothDeltaTime 사용으로 더 부드러운 이동
            float deltaTime = useSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime;
            
            // SmoothDamp 방식 (더 부드러움)
            newPosition = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                1f / smoothSpeed,
                Mathf.Infinity,
                deltaTime
            );
            
            // 또는 Lerp 방식
            // newPosition = Vector3.Lerp(
            //     transform.position, 
            //     desiredPosition, 
            //     smoothSpeed * deltaTime
            // );
        }
        else
        {
            newPosition = desiredPosition;
        }
        
        // 3. Dead Zone (매우 작은 이동 무시)
        float moveDistance = Vector3.Distance(transform.position, newPosition);
        if (moveDistance < deadZone)
        {
            return; // 너무 작은 이동은 무시
        }
        
        // 4. Pixel Snapping (픽셀 단위로 정렬)
        if (pixelSnapping)
        {
            newPosition = SnapToPixel(newPosition);
        }
        
        // 5. 범위 제한
        if (limitCameraBounds)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
        }
        
        transform.position = newPosition;
    }
    
    /// <summary>
    /// 위치를 픽셀 단위로 스냅
    /// </summary>
    Vector3 SnapToPixel(Vector3 position)
    {
        position.x = Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel;
        position.y = Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel;
        // Z는 그대로 (카메라 거리)
        
        return position;
    }
    
    /// <summary>
    /// 타겟 변경
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        velocity = Vector3.zero; // velocity 리셋
    }
    
    /// <summary>
    /// 즉시 타겟 위치로 이동 (컷씬 등)
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null) return;
        
        Vector3 targetPos = target.position + offset;
        if (pixelSnapping)
        {
            targetPos = SnapToPixel(targetPos);
        }
        
        transform.position = targetPos;
        velocity = Vector3.zero;
    }
    
    void OnDrawGizmosSelected()
    {
        // 범위 제한 표시
        if (limitCameraBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
        
        // 타겟 표시
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}