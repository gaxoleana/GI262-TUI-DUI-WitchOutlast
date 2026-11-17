using UnityEngine;

public class Staff : Weapon
{
    [SerializeField] private Projectile projectilePrefab;

    public int PierceCount
    {
        get { return GetPierceCount(WeaponLevel); }
    }

    public int NextPierceCount
    {
        get { return GetPierceCount(Mathf.Min(WeaponLevel + 1, MaxLevel)); }
    }

    private int GetPierceCount(int level)
    {
        if (level >= 8)
        {
            return 2;
        }

        if (level >= 4)
        {
            return 1;
        }

        return 0;
    }

    public override void Attack()
    {
        if (player == null || player.CurrentHealth <= 0 || projectilePrefab == null)
        {
            return;
        }

        Enemy target = FindNearestEnemy();

        if (target == null)
        {
            return;
        }

        Vector2 shootDirection = target.transform.position - transform.position;

        if (shootDirection == Vector2.zero)
        {
            shootDirection = Vector2.right;
        }

        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Initialize(shootDirection, baseDamage, PierceCount);
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);
        Enemy nearestEnemy = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (Enemy enemy in enemies)
        {
            if (!enemy.isActiveAndEnabled || enemy.CurrentHealth <= 0)
            {
                continue;
            }

            float distanceToEnemy = Vector2.Distance(player.transform.position, enemy.transform.position);

            if (distanceToEnemy < nearestDistance)
            {
                nearestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
