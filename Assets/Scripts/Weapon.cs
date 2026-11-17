using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected int baseDamage = 25;
    [SerializeField, Min(0.01f)] private float attackSpeed = 1f;
    [SerializeField, Min(1)] private int maxLevel = 8;
    [SerializeField, Min(0)] private int damageIncreasePerLevel = 5;
    [SerializeField, Min(0f)] private float attackSpeedIncreasePerLevel = 0.15f;

    protected Player player;
    private float attackTimer;
    private int weaponLevel = 1;
    private int startingDamage;
    private float startingAttackSpeed;

    public int WeaponLevel
    {
        get { return weaponLevel; }
    }

    public int MaxLevel
    {
        get { return Mathf.Max(1, maxLevel); }
    }

    public int Damage
    {
        get { return baseDamage; }
    }

    public float AttackSpeed
    {
        get { return attackSpeed; }
    }

    public bool CanUpgrade
    {
        get { return weaponLevel < MaxLevel; }
    }

    public int NextDamage
    {
        get
        {
            if (!CanUpgrade)
            {
                return baseDamage;
            }

            return baseDamage + Mathf.Max(0, damageIncreasePerLevel);
        }
    }

    public float NextAttackSpeed
    {
        get
        {
            if (!CanUpgrade)
            {
                return attackSpeed;
            }

            return attackSpeed + Mathf.Max(0f, attackSpeedIncreasePerLevel);
        }
    }

    protected void Awake()
    {
        player = GetComponentInParent<Player>();
        startingDamage = baseDamage;
        startingAttackSpeed = attackSpeed;
        ResetUpgrades();
    }

    protected void OnEnable()
    {
        attackTimer = 0f;
    }

    protected void Update()
    {
        if (Time.timeScale <= 0f || player == null || player.CurrentHealth <= 0 || attackSpeed <= 0f)
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

    public virtual bool Upgrade()
    {
        if (!CanUpgrade)
        {
            return false;
        }

        baseDamage = NextDamage;
        attackSpeed = NextAttackSpeed;
        weaponLevel++;
        attackTimer = Mathf.Min(attackTimer, 1f / attackSpeed);
        return true;
    }

    public void ResetUpgrades()
    {
        weaponLevel = 1;
        baseDamage = startingDamage;
        attackSpeed = startingAttackSpeed;
        attackTimer = 0f;
    }

    public abstract void Attack();
}
