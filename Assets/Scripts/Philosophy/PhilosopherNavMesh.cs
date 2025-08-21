using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PhilosopherNavMesh : MonoBehaviour
{
    public float speed = 5f;
    private NavMeshAgent agent;
    public float doorRadiusReduction = 0.2f;
    private float originalRadius;
    private SpriteRenderer spriteRenderer;
    private GameObject currentRoom;
    PhManager phManager;

    void Start()
    {
        phManager = FindAnyObjectByType<PhManager>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            Debug.Log("NavMeshAgent 추가됨: " + gameObject.name);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            Debug.Log("SpriteRenderer 추가됨: " + gameObject.name);
        }

        agent.speed = speed;
        agent.angularSpeed = 0;
        agent.acceleration = 20f;
        agent.stoppingDistance = 0.5f;
        agent.autoBraking = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;

        originalRadius = 0.5f;
        agent.radius = originalRadius;
        agent.height = 2.0f;
    }

    void Update()
    {
        // 우클릭: 선택된 철학자 이동
        PhilosopherManager manager = FindObjectOfType<PhilosopherManager>();
        if (Input.GetMouseButtonDown(1) && manager != null && manager.GetSelectedPhilosopher() == this)
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = 0f;

            if (NavMesh.SamplePosition(clickPos, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
            {
                AdjustRadiusForDoor(hit.position);
                StopAllCoroutines();
                StartCoroutine(MoveCoroutine(new Vector3(hit.position.x, hit.position.y, -1)));
            }
            else
            {
                Debug.Log("유효하지 않은 위치: " + clickPos);
            }
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.velocity = Vector3.zero;
            agent.radius = originalRadius;
        }

        UpdateCurrentRoom();
    }

    void AdjustRadiusForDoor(Vector3 targetPosition)
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 5.0f)
        {
            agent.radius = doorRadiusReduction;
            Debug.Log("문 통과용 radius로 변경: " + agent.radius + " (" + gameObject.name + ")");
        }
        else
        {
            agent.radius = originalRadius;
        }
    }

    IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
        Debug.Log("Moving: " + gameObject.name);
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 diff = agent.nextPosition - transform.position;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.position = new Vector3(agent.nextPosition.x, agent.nextPosition.y, -1);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
        transform.position = targetPos;
        transform.rotation = Quaternion.identity;
        Debug.Log("이동 완료: " + gameObject.name);
    }

    void UpdateCurrentRoom()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        GameObject newRoom = null;

        foreach (GameObject room in rooms)
        {
            SpriteRenderer roomRenderer = room.GetComponent<SpriteRenderer>();
            if (roomRenderer != null)
            {
                Vector2 roomPos = room.transform.position;
                Vector2 roomSize = roomRenderer.bounds.size;
                Vector2 min = roomPos - roomSize / 2;
                Vector2 max = roomPos + roomSize / 2;

                Vector2 pos = transform.position;
                if (pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y)
                {
                    newRoom = room;
                    break;
                }
            }
        }

        //방 이동
        if (newRoom != currentRoom)
        {
            if (currentRoom != null)
            {
                transform.SetParent(null);
                phManager.PhilosopherExitsRoom(currentRoom);
                Debug.Log(gameObject.name + "가 " + currentRoom.name + "에서 나감");
            }
            currentRoom = newRoom;
            if (currentRoom != null)
            {
                transform.SetParent(currentRoom.transform);
                phManager.PhilosopherEntersRoom(currentRoom);
                Debug.Log(gameObject.name + "가 " + currentRoom.name + "에 들어감");
            }
        }
    }

    // PhilosopherManager에서 호출할 public 메서드
    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void StopMovement()
    {
        agent.isStopped = true;
        agent.ResetPath();
        StopAllCoroutines();
    }

    void OnMouseEnter()
    {
        Debug.Log("마우스 오버: " + gameObject.name);
    }
}