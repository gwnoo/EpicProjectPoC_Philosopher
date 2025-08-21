using UnityEngine;
using System.Collections;

public class ShrinkOnEnter : MonoBehaviour
{
    public Vector3 shrinkSize = new Vector3(1f, 1f, 1f);
    public Vector3 originalSize = new Vector3(100f, 100f, 1f);
    private float moveUpOffset = 30f;
    public float shrinkDuration = 0.2f;
    public float growDuration = 0.2f;

    public float shrunkMoveSpeed = 20f;
    public float normalMoveSpeed = 500f;

    private bool _isShrunk = false;
    private Coroutine _resizeCoroutine;
    private PlayerMove _playerMove;

    public float exitPushDistance = 100f; // 나갈 때 밀리는 거리



    void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
    }

    private Vector3 _lastTinyNationPos;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isShrunk && other.CompareTag("TinyNation"))
        {
            _lastTinyNationPos = other.transform.position; // 충돌한 소인국 위치 저장

            if (_resizeCoroutine != null)
                StopCoroutine(_resizeCoroutine);
            _resizeCoroutine = StartCoroutine(ShrinkAndMove(other.transform));
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        if (_isShrunk && other.CompareTag("TinyNation"))
        {
            if (_resizeCoroutine != null)
                StopCoroutine(_resizeCoroutine);
            _resizeCoroutine = StartCoroutine(GrowAndZoomOut());
        }
    }

    private IEnumerator ShrinkAndMove(Transform target)
    {
        _isShrunk = true;

        float time = 0f;
        Vector3 startSize = transform.localScale;

        while (time < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(startSize, shrinkSize, time / shrinkDuration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = shrinkSize;

        // 진입 방향에 따라 이동 위치 계산
        Vector3 directionIn = (transform.position - target.position).normalized;
        Vector3 enterOffset = directionIn * moveUpOffset;
        Vector3 enterTargetPos = new Vector3(target.position.x + enterOffset.x, target.position.y + enterOffset.y, transform.position.z);

        transform.position = enterTargetPos;

        // 카메라 줌인 & 이동속도 줄이기
        Camera.main.GetComponent<FollowAndZoomCamera>()?.SetZoomedIn(true);
        _playerMove?.SetMoveSpeed(shrunkMoveSpeed);
    }


    private IEnumerator GrowAndZoomOut()
    {
        _isShrunk = false;

        //나가는 방향 벡터 계산 (캐릭터 중심 - 소인국 중심)
        Vector3 direction = (transform.position - _lastTinyNationPos).normalized;
        Vector3 pushTargetPos = transform.position + direction * exitPushDistance;
        pushTargetPos.z = 0f; // z축 고정

        //밀어낸 위치로 즉시 이동
        transform.position = pushTargetPos;

        //확대 애니메이션
        float time = 0f;
        Vector3 startSize = transform.localScale;

        while (time < growDuration)
        {
            transform.localScale = Vector3.Lerp(startSize, originalSize, time / growDuration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalSize;

        //카메라 & 이동속도 복원
        Camera.main.GetComponent<FollowAndZoomCamera>()?.SetZoomedIn(false);
        _playerMove?.SetMoveSpeed(normalMoveSpeed);
    }


}
