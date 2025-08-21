using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    private int _currentHealth;
    private EquipmentDropper _dropper;

    public void Initialize(int maxHealth)
    {
        _currentHealth = maxHealth;
    }

    private void Awake()
    {
        _dropper = GetComponent<EquipmentDropper>();
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        Debug.Log($"{gameObject.name} 데미지 {amount} → 남은 체력: {_currentHealth}");

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        _dropper?.DropEquipment();
        Destroy(gameObject);
    }
}
