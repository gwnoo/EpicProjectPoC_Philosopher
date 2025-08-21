using UnityEngine;
using System.Collections.Generic;

public class PhManager : MonoBehaviour
{
    // 방 정보 관리
    [System.Serializable]
    public class RoomInfo
    {
        public GameObject room;
        public int philosophyIndex;
        public bool isPhilosophyCreated;
        public int philosopherCount;
        public List<string> specialPhilosophersInRoom;
    }

    // 특수 철학자 정보
    [System.Serializable]
    public class SpecialPhilosopher
    {
        public string id;
        public string name;
    }

    [SerializeField] private List<RoomInfo> rooms = new List<RoomInfo>();
    [SerializeField] private List<SpecialPhilosopher> specialPhilosophers = new List<SpecialPhilosopher>();
    private Dictionary<GameObject, RoomInfo> roomLookup;
    public Dictionary<string, SpecialPhilosopher> specialPhilosopherLookup;

    void Start()
    {
        roomLookup = new Dictionary<GameObject, RoomInfo>();
        foreach (var roomInfo in rooms)
        {
            if (roomInfo.room != null)
            {
                roomLookup[roomInfo.room] = roomInfo;
                roomInfo.isPhilosophyCreated = false;
                roomInfo.philosopherCount = 0;
                roomInfo.specialPhilosophersInRoom = new List<string>();
            }
            else
            {
                Debug.LogWarning("RoomInfo에 방이 지정되지 않았습니다.");
            }
        }

        specialPhilosopherLookup = new Dictionary<string, SpecialPhilosopher>();
        foreach (var sp in specialPhilosophers)
        {
            if (!string.IsNullOrEmpty(sp.id))
            {
                specialPhilosopherLookup[sp.id] = sp;
                Debug.Log($"특수 철학자 초기화: {sp.name} (ID: {sp.id})");
            }
            else
            {
                Debug.LogWarning("특수 철학자의 ID가 설정되지 않았습니다.");
            }
        }

        if (specialPhilosophers.Count != 8)
        {
            Debug.LogWarning($"특수 철학자는 8명이어야 하지만, 현재 {specialPhilosophers.Count}명입니다.");
        }
    }

    public void PhilosopherEntersRoom(GameObject room, string specialPhilosopherId = null)
    {
        if (roomLookup.TryGetValue(room, out RoomInfo roomInfo))
        {
            if (string.IsNullOrEmpty(specialPhilosopherId))
            {
                roomInfo.philosopherCount++;
                Debug.Log($"일반 철학자가 {room.name}에 입장. 현재 일반 철학자 수: {roomInfo.philosopherCount}");
            }
            else if (specialPhilosopherLookup.ContainsKey(specialPhilosopherId))
            {
                if (!roomInfo.specialPhilosophersInRoom.Contains(specialPhilosopherId))
                {
                    roomInfo.specialPhilosophersInRoom.Add(specialPhilosopherId);
                    Debug.Log($"특수 철학자 {specialPhilosopherLookup[specialPhilosopherId].name} (ID: {specialPhilosopherId})가 {room.name}에 입장. 방 내 특수 철학자: {roomInfo.specialPhilosophersInRoom.Count}");
                }
                else
                {
                    Debug.LogWarning($"특수 철학자 {specialPhilosopherId}는 이미 {room.name}에 있습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"알 수 없는 특수 철학자 ID: {specialPhilosopherId}");
            }
        }
        else
        {
            Debug.LogWarning($"방 {room.name}에 대한 정보가 없습니다.");
        }
    }

    public void PhilosopherExitsRoom(GameObject room, string specialPhilosopherId = null)
    {
        if (roomLookup.TryGetValue(room, out RoomInfo roomInfo))
        {
            if (string.IsNullOrEmpty(specialPhilosopherId))
            {
                if (roomInfo.philosopherCount > 0)
                {
                    roomInfo.philosopherCount--;
                    Debug.Log($"일반 철학자가 {room.name}에서 퇴장. 현재 일반 철학자 수: {roomInfo.philosopherCount}");
                }
                else
                {
                    Debug.LogWarning($"{room.name}에 일반 철학자가 없습니다.");
                }
            }
            else if (specialPhilosopherLookup.ContainsKey(specialPhilosopherId))
            {
                if (roomInfo.specialPhilosophersInRoom.Contains(specialPhilosopherId))
                {
                    roomInfo.specialPhilosophersInRoom.Remove(specialPhilosopherId);
                    Debug.Log($"특수 철학자 {specialPhilosopherLookup[specialPhilosopherId].name} (ID: {specialPhilosopherId})가 {room.name}에서 퇴장. 방 내 특수 철학자: {roomInfo.specialPhilosophersInRoom.Count}");
                }
                else
                {
                    Debug.LogWarning($"특수 철학자 {specialPhilosopherId}는 {room.name}에 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"알 수 없는 특수 철학자 ID: {specialPhilosopherId}");
            }
        }
        else
        {
            Debug.LogWarning($"방 {room.name}에 대한 정보가 없습니다.");
        }
    }

    public void AddRoom(GameObject room, int philosophyIndex = 0)
    {
        RoomInfo newRoomInfo = new RoomInfo
        {
            room = room,
            philosophyIndex = philosophyIndex,
            isPhilosophyCreated = false,
            philosopherCount = 0,
            specialPhilosophersInRoom = new List<string>()
        };
        rooms.Add(newRoomInfo);
        roomLookup[room] = newRoomInfo;
        Debug.Log($"새 방 추가: {room.name} (인덱스: {philosophyIndex})");
    }

    public List<string> GetSpecialPhilosophersInRoom(GameObject room)
    {
        if (roomLookup.TryGetValue(room, out RoomInfo roomInfo))
        {
            return new List<string>(roomInfo.specialPhilosophersInRoom);
        }
        Debug.LogWarning($"방 {room.name}에 대한 정보가 없습니다.");
        return new List<string>();
    }

    public int GetPhilosopherCount(GameObject room)
    {
        if (roomLookup.TryGetValue(room, out RoomInfo roomInfo))
        {
            return roomInfo.philosopherCount;
        }
        Debug.LogWarning($"방 {room.name}에 대한 정보가 없습니다.");
        return 0;
    }

    public bool IsPhilosophyCreated(GameObject room)
    {
        return roomLookup.TryGetValue(room, out RoomInfo roomInfo) && roomInfo.isPhilosophyCreated;
    }

    // 방의 철학 생성 상태를 설정 (MakePh에서 호출)
    public void SetPhilosophyCreated(GameObject room, bool created)
    {
        if (roomLookup.TryGetValue(room, out RoomInfo roomInfo))
        {
            roomInfo.isPhilosophyCreated = created;
        }
    }

    public void LogRoomStatus()
    {
        foreach (var roomInfo in rooms)
        {
            string specialPhilosophers = roomInfo.specialPhilosophersInRoom.Count > 0
                ? string.Join(", ", roomInfo.specialPhilosophersInRoom.ConvertAll(id => specialPhilosopherLookup[id].name))
                : "없음";
            Debug.Log($"방: {roomInfo.room.name}, 인덱스: {roomInfo.philosophyIndex}, 생성: {roomInfo.isPhilosophyCreated}, 일반 철학자 수: {roomInfo.philosopherCount}, 특수 철학자: {specialPhilosophers}");
        }
    }

    public List<RoomInfo> GetRooms()
    {
        return new List<RoomInfo>(rooms); // rooms 리스트의 복사본 반환
    }

    public void SetPhilosopherCount(GameObject room, int count)
    {
        if (roomLookup.TryGetValue(room, out RoomInfo roomInfo))
        {
            roomInfo.philosopherCount = count;
            Debug.Log($"방 {room.name}의 일반 철학자 수가 {count}로 설정되었습니다.");
        }
        else
        {
            Debug.LogWarning($"방 {room.name}에 대한 정보가 없습니다.");
        }
    }
}