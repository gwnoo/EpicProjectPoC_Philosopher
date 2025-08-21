using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    private PhManager phManager; // PhManager 참조
    private MakePh makePh; // MakePh 참조
    private float turnDuration = 10f; // 턴 지속 시간 (초 단위)
    private float turnTimer; // 현재 턴의 남은 시간
    private int currentTurn = 0; // 현재 턴 번호

    public Action OnTurnStart; // 턴 종료 이벤트

    void Start()
    {
        // 참조 확인 및 초기화
        if (phManager == null)
        {
            phManager = FindAnyObjectByType<PhManager>();
            if (phManager == null)
            {
                Debug.LogError("PhManager를 찾을 수 없습니다. 인스펙터에서 참조를 설정하세요.");
                return;
            }
        }

        if (makePh == null)
        {
            makePh = FindAnyObjectByType<MakePh>();
            if (makePh == null)
            {
                Debug.LogError("MakePh를 찾을 수 없습니다. 인스펙터에서 참조를 설정하세요.");
                return;
            }
        }

        // 타이머 초기화
        turnTimer = turnDuration;
    }

    void Update()
    {
        // 턴 타이머 감소
        turnTimer -= Time.deltaTime;
        if (turnTimer <= 0)
        {
            EndTurn();
        }
    }

    // 턴 종료 시 호출
    private void EndTurn()
    {
        currentTurn++;
        Debug.Log($"턴 {currentTurn} 종료");

        // PhManager에서 최신 방 목록 가져오기
        var roomInfos = phManager.GetRooms();
        if (roomInfos.Count == 0)
        {
            Debug.LogWarning("현재 관리할 방이 없습니다.");
        }
        else
        {
            // 각 방에서 철학 생성 시도
            foreach (var roomInfo in roomInfos)
            {
                if (roomInfo.room != null)
                {
                    makePh.CreatePhilosophyInRoom(roomInfo.room);
                }
                else
                {
                    Debug.LogWarning("RoomInfo에 방이 지정되지 않았습니다.");
                }
            }
        }

        // 타이머 리셋
        turnTimer = turnDuration;
        OnTurnStart?.Invoke(); // 턴 시작 이벤트 호출   
        Debug.Log($"턴 {currentTurn + 1} 시작");
    }

    // 현재 턴 번호 반환
    public int GetCurrentTurn()
    {
        return currentTurn;
    }

    // 남은 턴 시간 반환
    public float GetRemainingTurnTime()
    {
        return turnTimer;
    }

    // 수동으로 턴 종료 트리거 (테스트용)
    public void ForceEndTurn()
    {
        turnTimer = 0;
    }
}