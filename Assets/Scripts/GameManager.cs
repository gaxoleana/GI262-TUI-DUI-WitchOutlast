using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event System.Action RunStarted;
    public event System.Action RunEnded;

    [SerializeField] private Player player;
    [SerializeField] private EnemySpawner enemySpawner;
    private bool isRunning;
    private float elapsedTime;
    private int killCount;
    private bool isPaused;
    private float resumeTimeScale = 1f;

    public bool IsRunning
    {
        get { return isRunning; }
    }

    public bool IsPaused
    {
        get { return isPaused; }
    }

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

        if (isPaused)
        {
            Time.timeScale = resumeTimeScale;
            isPaused = false;
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        player.SetMovementEnabled(false);
        player.SetWeaponsEnabled(false);
        enemySpawner.StopSpawning();
    }

    public void StartRun()
    {
        if (isRunning || player.CurrentHealth <= 0)
        {
            return;
        }

        Time.timeScale = 1f;
        isPaused = false;
        elapsedTime = 0f;
        killCount = 0;
        player.ResetProgression();
        player.ResetWeapons();
        isRunning = true;
        player.SetMovementEnabled(true);
        player.SetWeaponsEnabled(true);
        enemySpawner.StartSpawning();

        if (RunStarted != null)
        {
            RunStarted.Invoke();
        }
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

        if (isPaused)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
    }

    public bool PauseRun()
    {
        if (!isRunning || isPaused || player.CurrentHealth <= 0)
        {
            return false;
        }

        resumeTimeScale = Time.timeScale;
        isPaused = true;
        Time.timeScale = 0f;
        player.SetMovementEnabled(false);
        return true;
    }

    public void ResumeRun()
    {
        if (!isRunning || !isPaused)
        {
            return;
        }

        isPaused = false;
        Time.timeScale = resumeTimeScale;
        player.SetMovementEnabled(true);
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

        if (isPaused)
        {
            Time.timeScale = resumeTimeScale;
            isPaused = false;
        }

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

        if (RunEnded != null)
        {
            RunEnded.Invoke();
        }
    }
}
