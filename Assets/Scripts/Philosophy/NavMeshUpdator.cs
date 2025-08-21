using UnityEngine;
using NavMeshPlus.Components;

public class NavMeshUpdater : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private bool isDragging;

    void Start()
    {
        navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface를 찾을 수 없습니다.");
            return;
        }

        // 게임 시작 시 NavMesh 강제 갱신
        navMeshSurface.BuildNavMesh();
        Debug.Log("게임 시작 시 NavMesh 빌드 완료");

        // NavMeshModifier 추가
        if (!GetComponent<NavMeshModifier>())
        {
            NavMeshModifier modifier = gameObject.AddComponent<NavMeshModifier>();
            modifier.overrideArea = true;
            modifier.area = 1; // "Not Walkable"
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
        isDragging = false;
    }

    void Update()
    {
        bool positionChanged = transform.position != lastPosition;
        bool rotationChanged = transform.rotation != lastRotation;

        if (positionChanged || rotationChanged)
        {
            isDragging = true;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
        else if (isDragging && !positionChanged && !rotationChanged)
        {
            if (navMeshSurface != null)
            {
                navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
                Debug.Log("NavMesh 갱신 완료: " + gameObject.name +
                          " (위치: " + transform.position + ", 회전: " + transform.rotation.eulerAngles + ")");
            }
            isDragging = false;
        }
    }
}
