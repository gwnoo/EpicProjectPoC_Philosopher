using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private float spawnInterval = 3f;

    private Transform[] _spawnPoints;

    private int baseHealth = 40;
    private float baseSpeed = 5f;

    private int currentHealth;
    private float currentSpeed;

    void Start()
    {
        int count = transform.childCount;
        _spawnPoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            _spawnPoints[i] = transform.GetChild(i);
        }

        currentHealth = baseHealth;
        currentSpeed = baseSpeed;

        StartCoroutine(SpawnLoop());

        TurnManager tm = FindAnyObjectByType<TurnManager>();
        if (tm != null)
        {
            tm.OnTurnStart += OnTurnStart;
        }
    }

    void OnTurnStart()
    {
        currentHealth += 3;
        currentSpeed = Mathf.Min(currentSpeed + 0.3f, 10f);
        spawnInterval = Mathf.Max(spawnInterval - 0.2f, 0.5f);
        Debug.Log($"몬스터 강화됨: 체력 {currentHealth}, 속도 {currentSpeed}, 스폰 간격 {spawnInterval}");
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMonster()
    {
        if (_spawnPoints.Length == 0 || monsterPrefab == null) return;

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        GameObject monster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);

        var stats = monster.GetComponent<MonsterStats>();
        var logic = monster.GetComponent<Monster>();

        if (stats != null)
            stats.Initialize(currentHealth);

        if (logic != null)
            logic.moveSpeed = currentSpeed;
    }
}
