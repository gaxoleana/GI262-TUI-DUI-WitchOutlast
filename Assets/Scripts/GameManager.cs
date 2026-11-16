using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private EnemySpawner enemySpawner;
    private bool isRunning;
    private float elapsedTime;
    private int killCount;

    public float ElapsedTime
    {
        get
        {
            return elapsedTime;
        }
    }

    public int KillCount
    {
        get
        {
            return killCount;
        }
    }

    private void OnEnable()
    {
        Enemy.Killed += RegisterKill;
    }

    private void OnDisable()
    {
        Enemy.Killed -= RegisterKill;
    }

    private void Start()
    {
        StartRun();
    }

    public void StartRun()
    {
        elapsedTime = 0f;
        killCount = 0;
        player.ResetProgression();
        isRunning = true;
        player.SetMovementEnabled(true);
        player.SetWeaponsEnabled(true);
        enemySpawner.StartSpawning();
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        if (player.CurrentHealth <= 0)
        {
            EndRun();
            return;
        }

        elapsedTime += Time.deltaTime;
    }

    private void RegisterKill()
    {
        if (isRunning)
        {
            killCount++;
        }
    }

    public void EndRun()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        enemySpawner.StopSpawning();
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
