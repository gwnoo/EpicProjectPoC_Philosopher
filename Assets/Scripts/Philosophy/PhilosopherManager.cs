using UnityEngine;

public class PhilosopherManager : MonoBehaviour
{
    private PhilosopherNavMesh[] philosophers; // 모든 철학자 배열
    private PhilosopherNavMesh selectedPhilosopher = null; // 현재 선택된 철학자
    private static readonly Color defaultColor = Color.white;
    private static readonly Color selectedColor = Color.red;

    void Start()
    {
        // 씬에서 모든 철학자 찾기
        philosophers = FindObjectsOfType<PhilosopherNavMesh>();
        if (philosophers.Length == 0)
        {
            Debug.LogWarning("철학자가 없습니다.");
        }

        // 초기 설정: 모두 기본 색상
        foreach (var philosopher in philosophers)
        {
            philosopher.SetColor(defaultColor);
        }
    }

    void Update()
    {
        // 좌클릭: 철학자 선택/해제
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = 0f;

            RaycastHit2D[] hits = Physics2D.RaycastAll(clickPos, Vector2.zero);
            PhilosopherNavMesh clickedPhilosopher = null;

            foreach (RaycastHit2D hit in hits)
            {
                clickedPhilosopher = hit.collider?.GetComponent<PhilosopherNavMesh>();
                if (clickedPhilosopher != null)
                {
                    break; // 첫 번째 철학자 선택
                }
            }

            if (clickedPhilosopher != null)
            {
                SelectPhilosopher(clickedPhilosopher);
                Debug.Log("철학자 선택: " + clickedPhilosopher.gameObject.name);
            }
            else if (selectedPhilosopher != null)
            {
                DeselectPhilosopher();
                Debug.Log("철학자 선택 해제 및 이동 중지");
            }
        }
    }

    private void SelectPhilosopher(PhilosopherNavMesh philosopher)
    {
        if (selectedPhilosopher != null && selectedPhilosopher != philosopher)
        {
            selectedPhilosopher.SetColor(defaultColor);
            selectedPhilosopher.StopMovement();
        }

        selectedPhilosopher = philosopher;
        selectedPhilosopher.SetColor(selectedColor);
    }

    private void DeselectPhilosopher()
    {
        if (selectedPhilosopher != null)
        {
            selectedPhilosopher.SetColor(defaultColor);
            selectedPhilosopher.StopMovement();
            selectedPhilosopher = null;
        }
    }

    public PhilosopherNavMesh GetSelectedPhilosopher()
    {
        return selectedPhilosopher;
    }
}