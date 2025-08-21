using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragRoom : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Vector3 offset;
    private SnapToVertex snapScript;
    private bool moveHorizontal;
    private List<GameObject> connectedRooms = new List<GameObject>();
    private HashSet<GameObject> visitedRooms = new HashSet<GameObject>();
    private bool shouldDrag; // 드래그 여부 플래그
    private float centerThreshold = 0.2f; // 중앙 클릭 판단 기준 (월드 단위)

    void Start()
    {
        snapScript = FindObjectOfType<SnapToVertex>();
        if (snapScript == null)
        {
            Debug.LogError("SnapToVertex를 찾을 수 없습니다.");
        }
        if (!GetComponent<Collider2D>())
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                collider.size = sprite.size;
            }
            else
            {
                collider.size = new Vector2(1f, 1f);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭에만 반응
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3 center = transform.position;
        Vector2 clickOffset = new Vector2(
            Mathf.Abs(mousePosition.x - center.x),
            Mathf.Abs(mousePosition.y - center.y)
        );
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Vector2 size = sprite != null ? sprite.bounds.size : Vector2.one;

        // 중앙 클릭인지 확인 (크기의 20% 이내)
        if (clickOffset.x < size.x * centerThreshold && clickOffset.y < size.y * centerThreshold)
        {
            transform.Rotate(0f, 0f, 90f); // 90도 회전
            Debug.Log("방 회전: " + gameObject.name);

            // 연결된 방도 회전
            foreach (GameObject room in connectedRooms)
            {
                room.transform.RotateAround(transform.position, Vector3.forward, 90f);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 좌클릭에만 반응
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        offset = transform.position - mousePosition;

        Vector3 center = transform.position;
        Vector2 clickOffset = new Vector2(
            Mathf.Abs(mousePosition.x - center.x),
            Mathf.Abs(mousePosition.y - center.y)
        );
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Vector2 size = sprite != null ? sprite.bounds.size : Vector2.one;

        // 중앙 클릭이면 드래그 무시
        shouldDrag = !(clickOffset.x < size.x * centerThreshold && clickOffset.y < size.y * centerThreshold);
        if (!shouldDrag)
        {
            Debug.Log("중앙 드래그 무시: 회전만 적용");
            return;
        }

        moveHorizontal = Mathf.Abs(mousePosition.x - center.x) > Mathf.Abs(mousePosition.y - center.y);
        Debug.Log(moveHorizontal ? "좌우 이동" : "상하 이동");

        FindConnectedRooms();
        snapScript?.SetDraggingObject(this.gameObject, connectedRooms);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!shouldDrag) return; // 중앙 드래그는 무시

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3 newPosition = mousePosition + offset;
        newPosition.z = transform.position.z;

        if (moveHorizontal) newPosition.y = transform.position.y;
        else newPosition.x = transform.position.x;

        Vector3 delta = newPosition - transform.position;
        transform.position = newPosition;
        foreach (GameObject room in connectedRooms)
        {
            room.transform.position += delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (shouldDrag)
        {
            snapScript?.SnapOnRelease();
            connectedRooms.Clear();
            visitedRooms.Clear();
        }
        shouldDrag = false; // 드래그 상태 초기화
    }

    private void FindConnectedRooms()
    {
        connectedRooms.Clear();
        visitedRooms.Clear();
        visitedRooms.Add(this.gameObject);
        FindConnectedRoomsRecursively(this.gameObject);
    }

    private void FindConnectedRoomsRecursively(GameObject currentRoom)
    {
        GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Room");
        Vector2[] myVertices = GetVertices(currentRoom.transform);

        foreach (GameObject room in allRooms)
        {
            if (room == currentRoom || visitedRooms.Contains(room)) continue;

            Vector2[] roomVertices = GetVertices(room.transform);
            int connectionCount = 0;
            Vector2 firstVertex = Vector2.zero, secondVertex = Vector2.zero;

            for (int i = 0; i < myVertices.Length && connectionCount < 2; i++)
            {
                for (int j = 0; j < roomVertices.Length && connectionCount < 2; j++)
                {
                    if (Vector2.Distance(myVertices[i], roomVertices[j]) < 0.1f)
                    {
                        if (connectionCount == 0) firstVertex = myVertices[i];
                        else secondVertex = myVertices[i];
                        connectionCount++;
                    }
                }
            }

            if (connectionCount >= 2)
            {
                bool isHorizontal = Mathf.Abs(firstVertex.x - secondVertex.x) < Mathf.Abs(firstVertex.y - secondVertex.y);
                if (moveHorizontal == isHorizontal)
                {
                    connectedRooms.Add(room);
                    visitedRooms.Add(room);
                    Debug.Log((isHorizontal ? "가로" : "세로") + " 연결 방 추가: " + room.name);
                    FindConnectedRoomsRecursively(room);
                }
            }
        }
    }

    private Vector2[] GetVertices(Transform objTransform)
    {
        SpriteRenderer sprite = objTransform.GetComponent<SpriteRenderer>();
        if (!sprite)
        {
            Debug.LogWarning("SpriteRenderer가 없습니다. 기본 크기 사용: " + objTransform.name);
            Vector2 defaultSize = new Vector2(1f, 1f);
            Vector2 temp = objTransform.position;
            return new Vector2[]
            {
                temp + new Vector2(-defaultSize.x / 2, -defaultSize.y / 2),
                temp + new Vector2(defaultSize.x / 2, -defaultSize.y / 2),
                temp + new Vector2(defaultSize.x / 2, defaultSize.y / 2),
                temp + new Vector2(-defaultSize.x / 2, defaultSize.y / 2)
            };
        }
        Vector2 size = sprite.bounds.size;
        Vector2 pos = objTransform.position;
        return new Vector2[]
        {
            pos + new Vector2(-size.x / 2, -size.y / 2),
            pos + new Vector2(size.x / 2, -size.y / 2),
            pos + new Vector2(size.x / 2, size.y / 2),
            pos + new Vector2(-size.x / 2, size.y / 2)
        };
    }
}