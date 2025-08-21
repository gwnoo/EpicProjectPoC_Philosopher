using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorBehavior : MonoBehaviour
{
    private CollectorMover mover;
    private StorageBox storageBox;
    [SerializeField] private List<Hero> heroes;

    private Queue<EquipmentData> _collectedEquipments = new();
    private bool _isBusy = false;
    private Vector3 _originalPosition;

    private void Awake()
    {
        mover = GetComponent<CollectorMover>();
        storageBox = FindAnyObjectByType<StorageBox>();
        _originalPosition = transform.position;
    }

    private void Start()
    {
        TurnManager tm = FindAnyObjectByType<TurnManager>();
        if (tm != null)
        {
            tm.OnTurnStart += OnTurnStart;
        }
    }

    void OnTurnStart()
    {
        if (_isBusy) return;

        Equipment[] allEquipment = GameObject.FindObjectsByType<Equipment>(0);
        if (allEquipment.Length > 0)
            StartCoroutine(CollectAndProcess(allEquipment));
    }

    IEnumerator CollectAndProcess(Equipment[] items)
    {
        _isBusy = true;

        // 1. 모든 장비 줍기
        foreach (var equipment in items)
        {
            if (equipment == null) continue;

            mover.MoveTo(equipment.transform.position);
            yield return new WaitUntil(() => mover.ReachedDestination());
            yield return new WaitForSeconds(0.1f);

            EquipmentData data = equipment.data;
            Destroy(equipment.gameObject);
            Debug.Log($"장비 줍기 완료: {data.itemName}");
            _collectedEquipments.Enqueue(data);
        }

        // 2. 줍고 나서 하나씩 처리
        while (_collectedEquipments.Count > 0)
        {
            EquipmentData data = _collectedEquipments.Dequeue();

            // 장착 가능한 영웅 탐색
            Hero bestTarget = null;
            int lowestPower = int.MaxValue;

            foreach (var hero in heroes)
            {
                if (hero.CanAccept(data))
                {
                    int avgPower = hero.GetPowerSum();
                    if (avgPower < lowestPower)
                    {
                        lowestPower = avgPower;
                        bestTarget = hero;
                    }
                }
            }

            if (bestTarget != null)
            {
                mover.MoveTo(bestTarget.transform.position);
                yield return new WaitUntil(() => mover.ReachedDestination());
                yield return new WaitForSeconds(0.1f);
                bestTarget.Equip(data);
                Debug.Log($"장비 장착됨: {data.itemName} → {bestTarget.name}");
            }
            else
            {
                mover.MoveTo(storageBox.transform.position);
                yield return new WaitUntil(() => mover.ReachedDestination());
                yield return new WaitForSeconds(0.1f);
                storageBox.AddMaterial(data.slot, data.power);
                Debug.Log($"장비 분해됨: {data.itemName} → {data.slot} 재료 +{data.power}");
            }
        }

        // 3. 처리 완료 후 다시 필드에 장비가 있는지 확인
        Equipment[] newDroppedItems = GameObject.FindObjectsByType<Equipment>(0);
        if (newDroppedItems.Length > 0)
        {
            Debug.Log("새 장비 발견 → 즉시 다시 수거 시작");
            StartCoroutine(CollectAndProcess(newDroppedItems));
        }
        else
        {
            // 원래 자리로 복귀
            mover.MoveTo(_originalPosition);
            yield return new WaitUntil(() => mover.ReachedDestination());

            _isBusy = false;
        }
    }
}
