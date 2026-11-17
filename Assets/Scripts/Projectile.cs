using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lifeTime = 6f;

    private Rigidbody2D projectileBody;
    private int damage;
    private bool isFinished;
    private int remainingPierces;
    private readonly HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    private void Awake()
    {
        projectileBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 direction, int projectileDamage, int pierceCount = 0)
    {
        damage = projectileDamage;
        remainingPierces = Mathf.Max(0, pierceCount);
        isFinished = false;
        hitEnemies.Clear();
        Vector2 moveDirection = direction.normalized;
        projectileBody.linearVelocity = moveDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled || isFinished)
        {
            return;
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            if (enemy.CurrentHealth <= 0 || hitEnemies.Contains(enemy))
            {
                return;
            }

            hitEnemies.Add(enemy);
            enemy.TakeDamage(damage);

            if (remainingPierces > 0)
            {
                remainingPierces--;
                return;
            }

            Finish();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Finish();
        }
    }

    private void Finish()
    {
        isFinished = true;
        projectileBody.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        projectileBody.linearVelocity = Vector2.zero;
    }
}
