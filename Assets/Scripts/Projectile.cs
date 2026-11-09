using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lifeTime = 6f;

    private Rigidbody2D projectileBody;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        projectileBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 direction, int projectileDamage)
    {
        damage = projectileDamage;
        Vector2 moveDirection = direction.normalized;
        projectileBody.linearVelocity = moveDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled || hasHit)
        {
            return;
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            if (enemy.CurrentHealth <= 0)
            {
                return;
            }

            hasHit = true;
            projectileBody.linearVelocity = Vector2.zero;
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            hasHit = true;
            projectileBody.linearVelocity = Vector2.zero;
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        projectileBody.linearVelocity = Vector2.zero;
    }
}
