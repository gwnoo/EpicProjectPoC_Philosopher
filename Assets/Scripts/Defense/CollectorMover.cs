using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CollectorMover : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Coroutine _moveRoutine;
    private Vector3 _target;

    public bool IsMoving { get; private set; }

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = 30f;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.updatePosition = false;
    }

    public void MoveTo(Vector3 position)
    {
        _target = new Vector3(position.x, position.y, -1f);
        _agent.SetDestination(_target);

        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        IsMoving = true;
        while (Vector2.Distance(transform.position, _target) > 0.1f)
        {
            transform.position = _agent.nextPosition;
            yield return null;
        }

        transform.position = _target;
        IsMoving = false;
    }

    public bool ReachedDestination() => !IsMoving;
}
