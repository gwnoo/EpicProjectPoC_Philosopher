using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;

    private Hero hero;
    private float lastAttackTime;

    private float attackCooldown;
    private float damageMultiplier;
    private Color endColor;

    void Start()
    {
        hero = GetComponent<Hero>();

        // 클래스에 따라 쿨타임/데미지 설정
        ClassType type = hero.GetHeroClass();
        attackCooldown = HeroBalance.GetAttackCooldown(type);
        damageMultiplier = HeroBalance.GetDamageMultiplier(type);
        endColor = HeroBalance.GetEndColor(type);
    }

    void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            GameObject target = FindClosestEnemy();
            if (target != null)
            {
                FireAt(target.transform);
                lastAttackTime = Time.time;
            }
        }
    }

    void FireAt(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        GameObject proj = Instantiate(projectilePrefab, transform.position, rotation);
        Projectile p = proj.GetComponent<Projectile>();

        int rawPower = hero.GetPowerSum(); // 장비 총합
        p.damage = Mathf.RoundToInt(rawPower * damageMultiplier);
        p.endColor = endColor;
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                closest = enemy;
                minDist = dist;
            }
        }
        return closest;
    }
}
