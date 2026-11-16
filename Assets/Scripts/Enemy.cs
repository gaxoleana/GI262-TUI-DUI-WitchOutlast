using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    public static event System.Action Killed;

    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int baseDamage = 4;
    [SerializeField] private ExpOrb expOrbPrefab;

    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
    }

    private Rigidbody2D enemyBody;

    protected void Awake()
    {
        currentHealth = baseHealth;
        enemyBody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Transform playerTarget)
    {
        player = playerTarget;
    }

    protected void ChasePlayer()
    {
        if (player == null)
        {
            enemyBody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 enemyPosition = enemyBody.position;
        Vector2 playerPosition = player.position;
        float moveDistance = moveSpeed * Time.fixedDeltaTime;

        Vector2 nextPosition = Vector2.MoveTowards(enemyPosition, playerPosition, moveDistance);
        Vector2 movement = nextPosition - enemyPosition;

        enemyBody.linearVelocity = movement / Time.fixedDeltaTime;
    }

    protected void OnDisable()
    {
        enemyBody.linearVelocity = Vector2.zero;
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (!isActiveAndEnabled || currentHealth <= 0)
        {
            return;
        }

        Player touchedPlayer = other.GetComponent<Player>();

        if (touchedPlayer != null)
        {
            touchedPlayer.TakeDamage(baseDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth == 0)
        {
            enemyBody.linearVelocity = Vector2.zero;

            if (expOrbPrefab != null)
            {
                Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            }

            if (Killed != null)
            {
                Killed.Invoke();
            }

            Destroy(gameObject);
        }
    }
}
