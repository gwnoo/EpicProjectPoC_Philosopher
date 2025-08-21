using UnityEngine;

public class FollowAndZoomCamera : MonoBehaviour
{
    public Transform target;                 // 따라갈 대상 (플레이어)
    public float followSpeed = 5f;           // 따라가는 속도
    public float defaultSize = 500f;         // 기본 카메라 크기
    public float zoomedInSize = 20f;         // 축소된 카메라 크기
    public float zoomSpeed = 3f;             // 줌 전환 속도

    private Camera _cam;
    private bool _isZoomedIn = false;

    void Start()
    {
        target = FindAnyObjectByType<PlayerMove>()?.transform;
        _cam = GetComponent<Camera>();
        _cam.orthographicSize = defaultSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // 픽셀 퍼펙트를 위한 정수 위치 스냅
        smoothPos.x = Mathf.Round(smoothPos.x * 100f) / 100f;
        smoothPos.y = Mathf.Round(smoothPos.y * 100f) / 100f;

        transform.position = smoothPos;

        float targetSize = _isZoomedIn ? zoomedInSize : defaultSize;
        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }


    // 외부에서 호출 (ShrinkOnEnter 등에서)
    public void SetZoomedIn(bool isZoomedIn)
    {
        _isZoomedIn = isZoomedIn;
    }
}
