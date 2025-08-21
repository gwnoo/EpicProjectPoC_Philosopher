using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class FarmerMover : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Coroutine _moveCoroutine;
    private Vector3 _targetPosition;

    public bool IsMoving { get; private set; } = false;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        // NavMeshAgent 세팅 for 2D
        _agent.speed = 15f;
        _agent.angularSpeed = 0;
        _agent.acceleration = 20f;
        _agent.stoppingDistance = 0.1f;
        _agent.autoBraking = true;

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.updatePosition = false; // 수동으로 위치 업데이트

        _agent.radius = 0.3f; // 작물 간 간격 조절
        _agent.height = 0.1f; // 2D 환경이므로 낮게 설정
    }

    public void MoveTo(Vector3 destination)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _targetPosition = new Vector3(destination.x, destination.y, -1f);
        _agent.SetDestination(_targetPosition);
        _moveCoroutine = StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        IsMoving = true;

        while (Vector2.Distance(transform.position, _targetPosition) > 0.05f)
        {
            // NavMeshAgent가 계산한 위치로 직접 이동
            Vector3 next = _agent.nextPosition;
            next.z = -1f; // Z 고정
            transform.position = next;

            yield return null;
        }

        transform.position = _targetPosition;
        IsMoving = false;
    }

    public bool ReachedDestination()
    {
        return !IsMoving && Vector2.Distance(transform.position, _targetPosition) <= _agent.stoppingDistance;
    }

    public void StopMoving()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        _agent.ResetPath();
        IsMoving = false;
    }
}
