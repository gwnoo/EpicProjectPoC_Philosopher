using System.Collections.Generic;
using UnityEngine;

public class MakePh : MonoBehaviour
{
    [SerializeField] private Sprite[] philosophySprites; // 철학 이미지 배열
    [SerializeField] private PhManager phManager; // PhManager 참조
    private int philosophyIndex = 0; // 전체 철학 인덱스 (초기값 0)
    private HashSet<int> createdPhilosophyIndices = new HashSet<int>(); // 생성된 철학 인덱스 추적

    void Start()
    {
        if (philosophySprites == null || philosophySprites.Length == 0)
        {
            Debug.LogWarning("철학 이미지가 설정되지 않았습니다.");
        }

        if (phManager == null)
        {
            phManager = FindObjectOfType<PhManager>();
            if (phManager == null)
            {
                Debug.LogError("PhManager를 찾을 수 없습니다. 인스펙터에서 참조를 설정하세요.");
            }
        }
    }

    // 특정 방에서 철학 생성
    public void CreatePhilosophyInRoom(GameObject room)
    {
        if (phManager == null)
        {
            Debug.LogError("PhManager가 설정되지 않았습니다.");
            return;
        }

        if (phManager.IsPhilosophyCreated(room))
        {
            Debug.Log($"방 {room.name}의 철학은 이미 생성되었습니다.");
            return;
        }

        // 다음 철학 인덱스 계산 (첫 생성 시 1)
        int nextPhilosophyIndex = philosophyIndex + 1;

        if (nextPhilosophyIndex >= philosophySprites.Length)
        {
            Debug.LogWarning($"철학 인덱스 {nextPhilosophyIndex}가 philosophySprites 배열 범위를 초과했습니다. 더 이상 철학을 생성할 수 없습니다.");
            return;
        }

        if (createdPhilosophyIndices.Contains(nextPhilosophyIndex))
        {
            Debug.LogWarning($"철학 인덱스 {nextPhilosophyIndex}는 이미 생성되었습니다.");
            return;
        }

        // 특수 철학자 확인
        var specialPhilosophers = phManager.GetSpecialPhilosophersInRoom(room);
        bool specialPhilosopherConditionMet = CheckSpecialPhilosopherCondition(room, specialPhilosophers);

        if (specialPhilosopherConditionMet)
        {
            UpdateFloorSprite(room, philosophySprites[nextPhilosophyIndex]);
            phManager.SetPhilosophyCreated(room, true);
            createdPhilosophyIndices.Add(nextPhilosophyIndex); // 생성된 인덱스 기록
            philosophyIndex = nextPhilosophyIndex; // 전체 인덱스 증가
        }
        else
        {
            // 일반 철학자 수 계산 (특수 철학자도 일반 철학자로 포함)
            int totalPhilosophers = phManager.GetPhilosopherCount(room) + specialPhilosophers.Count;
            float creationChance = GetPhilosophyCreationChance(totalPhilosophers);

            if (creationChance > 0 && Random.value <= creationChance)
            {
                UpdateFloorSprite(room, philosophySprites[nextPhilosophyIndex]);
                phManager.SetPhilosophyCreated(room, true);
                createdPhilosophyIndices.Add(nextPhilosophyIndex); // 생성된 인덱스 기록
                philosophyIndex = nextPhilosophyIndex; // 전체 인덱스 증가
                if (phManager.GetPhilosopherCount(room) > 0)
                {
                    foreach (Transform child in room.transform)
                    {
                        PhilosopherNavMesh philosopher = child.GetComponent<PhilosopherNavMesh>();
                        if (philosopher != null)
                        {
                            Destroy(child.gameObject); // 철학자 오브젝트 파괴
                        }
                    }
                    phManager.SetPhilosopherCount(room, 0);
                }
            }
        }
    }

    // 특수 철학자의 조건 확인
    private bool CheckSpecialPhilosopherCondition(GameObject room, List<string> specialPhilosopherIds)
    {
        if (specialPhilosopherIds.Count == 0)
        {
            return false;
        }

        foreach (string id in specialPhilosopherIds)
        {
            if (phManager.specialPhilosopherLookup.TryGetValue(id, out var specialPhilosopher))
            {
                bool conditionMet = EvaluateSpecialPhilosopherCondition(id, room);
                if (conditionMet)
                {
                    Debug.Log($"특수 철학자 {specialPhilosopher.name} (ID: {id}) 조건 만족 in {room.name}");
                    return true; // 하나라도 조건을 만족하면 즉시 생성
                }
                else
                {
                    Debug.Log($"특수 철학자 {specialPhilosopher.name} (ID: {id}) 조건 불만족 in {room.name}");
                }
            }
        }
        return false; // 모든 특수 철학자가 조건을 만족하지 않음
    }

    // 각 특수 철학자의 고유 조건 평가 (ID 기반)
    private bool EvaluateSpecialPhilosopherCondition(string id, GameObject room)
    {
        int philosopherCount = phManager.GetPhilosopherCount(room); // 일반 철학자 수
        int specialCount = phManager.GetSpecialPhilosophersInRoom(room).Count; // 특수 철학자 수

        switch (id)
        {
            case "SP0": return philosopherCount == 0 && specialCount == 1; // 고독한 사색가
            case "SP1": return (philosopherCount + specialCount) >= 2; // 논쟁가
            case "SP2": return (philosopherCount + specialCount) >= 3; // 중재자
            case "SP3": return specialCount >= 2; // 집단주의자
            case "SP4": return (philosopherCount + specialCount) == 1; // 혁신가
            case "SP5": return philosopherCount >= 2; // 관찰자
            case "SP6": return (philosopherCount + specialCount) >= 4; // 혼돈의 사상가
            case "SP7": return (philosopherCount + specialCount) == 3; // 완벽주의자
            default:
                Debug.LogWarning($"알 수 없는 특수 철학자 ID: {id}");
                return false;
        }
    }

    // 철학 생성 확률 계산 (일반 철학자 용)
    private float GetPhilosophyCreationChance(int philosopherCount)
    {
        switch (philosopherCount)
        {
            case 1: Debug.Log("고독 상태: 철학 생성 확률 20%"); return 0.2f;
            case 2: Debug.Log("논쟁 상태: 철학 생성 확률 40%"); return 0.4f;
            case 3: Debug.Log("논쟁 상태: 철학 생성 확률 60%"); return 0.6f;
            case 4 or > 4: Debug.Log("철학 정체 상태: 철학 생성 불가"); return 0f;
            default: Debug.Log("철학자 없음: 철학 생성 불가"); return 0f;
        }
    }

    // 방의 바닥 스프라이트 업데이트
    private void UpdateFloorSprite(GameObject room, Sprite newSprite)
    {
        if (room.transform.childCount > 0)
        {
            Transform firstChild = room.transform.GetChild(0); // 첫 번째 자식 오브젝트 (바닥)
            SpriteRenderer spriteRenderer = firstChild.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newSprite;
                Debug.Log($"바닥 스프라이트 변경: {firstChild.gameObject.name} -> {newSprite.name}");
            }
            else
            {
                Debug.LogWarning($"{room.name}의 첫 번째 자식 {firstChild.gameObject.name}에 SpriteRenderer가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"{room.name}에 자식 오브젝트(바닥)가 없습니다.");
        }
    }

    // 현재 철학 인덱스 반환 (디버깅 또는 외부 접근용)
    public int GetCurrentPhilosophyIndex()
    {
        return philosophyIndex;
    }
}