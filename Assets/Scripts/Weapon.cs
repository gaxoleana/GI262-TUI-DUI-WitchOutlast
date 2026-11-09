using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected int baseDamage = 25;
    [SerializeField, Min(0.01f)] private float attackSpeed = 1f;

    protected Player player;
    private float attackTimer;

    protected void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    protected void OnEnable()
    {
        attackTimer = 0f;
    }

    protected void Update()
    {
        if (player == null || player.CurrentHealth <= 0 || attackSpeed <= 0f)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = 1f / attackSpeed;
        }
    }

    public abstract void Attack();
}
