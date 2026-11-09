using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    private bool isRunning;

    private void Start()
    {
        StartRun();
    }

    public void StartRun()
    {
        isRunning = true;
        player.SetMovementEnabled(true);
        player.SetWeaponsEnabled(true);
    }

    private void Update()
    {
        if (isRunning && player.CurrentHealth <= 0)
        {
            EndRun();
        }
    }

    public void EndRun()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        player.SetMovementEnabled(false);
        player.SetWeaponsEnabled(false);

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);

        foreach (Enemy enemy in enemies)
        {
            enemy.enabled = false;
        }

        Projectile[] projectiles = FindObjectsByType<Projectile>(FindObjectsInactive.Exclude);

        foreach (Projectile projectile in projectiles)
        {
            projectile.enabled = false;
        }
    }
}
