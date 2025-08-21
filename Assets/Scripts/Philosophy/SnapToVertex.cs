using UnityEngine;
using System.Collections.Generic;

public class SnapToVertex : MonoBehaviour
{
    private GameObject targetObject;
    public float snapRange = 5.0f; // 스냅 허용 범위 (월드 단위)
    private Vector2[] myVertices;
    private Vector2[] targetVertices;
    private GameObject draggingObject;
    private List<GameObject> connectedRooms;

    void Start()
    {
        UpdateVertices();
    }

    void UpdateVertices()
    {
        myVertices = GetVertices(transform);
        if (targetObject != null)
        {
            targetVertices = GetVertices(targetObject.transform);
            Debug.Log("내 꼭지점: " + string.Join(", ", myVertices));
            Debug.Log("타겟 꼭지점: " + string.Join(", ", targetVertices));
        }
    }

    Vector2[] GetVertices(Transform objTransform)
    {
        SpriteRenderer spriteRenderer = objTransform.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogWarning(objTransform.name + "에 SpriteRenderer 또는 Sprite가 없습니다.");
            return new Vector2[0];
        }

        // 로컬 크기에 스케일 적용
        Vector2 localSize = spriteRenderer.sprite.bounds.size; // 스프라이트의 로컬 단위 크기
        Vector2 scaledSize = new Vector2(localSize.x * objTransform.localScale.x, localSize.y * objTransform.localScale.y);
        Vector2 pos = objTransform.position;

        return new Vector2[]
        {
            pos + new Vector2(-scaledSize.x / 2, -scaledSize.y / 2),
            pos + new Vector2(scaledSize.x / 2, -scaledSize.y / 2),
            pos + new Vector2(scaledSize.x / 2, scaledSize.y / 2),
            pos + new Vector2(-scaledSize.x / 2, scaledSize.y / 2)
        };
    }

    void SnapToClosestVertex(GameObject roomToSnap)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("타겟 오브젝트가 설정되지 않음");
            return;
        }

        float distance = Vector2.Distance(roomToSnap.transform.position, targetObject.transform.position);
        Debug.Log("거리: " + distance + " (snapRange: " + snapRange + ")");
        if (distance > snapRange)
        {
            Debug.Log("거리 초과로 스냅하지 않음: " + roomToSnap.name);
            return;
        }

        Vector2[] snapVertices = GetVertices(roomToSnap.transform);
        UpdateVertices(); // 타겟 꼭지점 갱신

        float minDistance = float.MaxValue;
        Vector2 closestMyVertex = Vector2.zero;
        Vector2 closestTargetVertex = Vector2.zero;

        for (int i = 0; i < snapVertices.Length; i++)
        {
            for (int j = 0; j < targetVertices.Length; j++)
            {
                float dist = Vector2.Distance(snapVertices[i], targetVertices[j]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestMyVertex = snapVertices[i];
                    closestTargetVertex = targetVertices[j];
                }
            }
        }

        Debug.Log($"가장 가까운 꼭지점: {roomToSnap.name} 꼭지점 = {closestMyVertex}, 타겟 꼭지점 = {closestTargetVertex}, 거리 = {minDistance}");

        Vector2 offset = closestTargetVertex - closestMyVertex;
        Vector3 finalDelta = offset;

        roomToSnap.transform.position = (Vector2)roomToSnap.transform.position + offset;
        Debug.Log(roomToSnap.name + " 스냅 후 위치: " + roomToSnap.transform.position);

        // 드래그 오브젝트와 연결된 방 이동
        if (draggingObject != roomToSnap)
        {
            draggingObject.transform.position += finalDelta;
            Debug.Log(draggingObject.name + " 연결 이동 후 위치: " + draggingObject.transform.position);
        }

        foreach (GameObject room in connectedRooms)
        {
            if (room != roomToSnap && room != draggingObject)
            {
                room.transform.position += finalDelta;
                Debug.Log(room.name + " 연결 이동 후 위치: " + room.transform.position);
            }
        }
    }

    private void SetClosestRoomTarget()
    {
        GameObject[] roomObjects = GameObject.FindGameObjectsWithTag("Room");
        if (roomObjects.Length == 0)
        {
            Debug.LogWarning("Room 태그 오브젝트가 존재하지 않음");
            return;
        }

        float minDistance = float.MaxValue;
        GameObject closestRoom = null;

        Vector2 referencePosition = draggingObject.transform.position;
        Debug.Log("타겟 찾기 기준 위치: " + referencePosition);

        foreach (GameObject room in roomObjects)
        {
            if (room == this.gameObject || room == draggingObject || (connectedRooms != null && connectedRooms.Contains(room)))
            {
                Debug.Log("제외된 룸: " + room.name);
                continue;
            }

            float distance = Vector2.Distance(referencePosition, room.transform.position);
            Debug.Log($"룸: {room.name}, 거리: {distance}");

            if (distance < minDistance)
            {
                minDistance = distance;
                closestRoom = room;
            }
        }

        targetObject = closestRoom;
        if (targetObject != null)
        {
            Debug.Log("가장 가까운 Room 타겟 설정: " + targetObject.name + ", 거리: " + minDistance);
        }
        else
        {
            Debug.LogWarning("적합한 타겟 룸을 찾지 못함");
        }
    }

    public void SetDraggingObject(GameObject obj, List<GameObject> connected)
    {
        draggingObject = obj;
        connectedRooms = connected;
        Debug.Log("드래그 오브젝트 설정: " + (obj != null ? obj.name : "null") + ", 연결된 방 수: " + (connected != null ? connected.Count : 0));
    }

    public void SnapOnRelease()
    {
        Debug.Log("SnapOnRelease 호출됨");
        if (draggingObject != null)
        {
            SetClosestRoomTarget();
            if (targetObject != null)
            {
                SnapToClosestVertex(draggingObject);
                UpdateVertices();

                if (Vector2.Distance(draggingObject.transform.position, targetObject.transform.position) > snapRange)
                {
                    Debug.Log("드래그한 룸 스냅 실패, 연결된 룸 확인 시작");
                    foreach (GameObject room in connectedRooms)
                    {
                        float distance = Vector2.Distance(room.transform.position, targetObject.transform.position);
                        if (distance <= snapRange)
                        {
                            SnapToClosestVertex(room);
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("타겟을 찾을 수 없음");
            }
            draggingObject = null;
            connectedRooms = null;
        }
        else
        {
            Debug.LogWarning("SnapOnRelease 호출 시 드래그 오브젝트가 null임");
        }
    }
}