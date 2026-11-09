using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private List<Weapon> weapons = new List<Weapon>();

    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
    }

    private InputAction moveAction;
    private Rigidbody2D playerBody;
    private Vector2 moveDirection;
    private bool movementEnabled;
    private float nextDamageTime;

    private void Awake()
    {
        currentHealth = baseHealth;
        playerBody = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        playerBody.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        Vector2 inputDirection = moveAction.ReadValue<Vector2>();
        moveDirection = Vector2.ClampMagnitude(inputDirection, 1f);
    }

    private void FixedUpdate()
    {
        if (movementEnabled)
        {
            Vector2 moveVelocity = moveDirection * moveSpeed;
            playerBody.linearVelocity = moveVelocity;
        }
        else
        {
            playerBody.linearVelocity = Vector2.zero;
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            playerBody.linearVelocity = Vector2.zero;
        }
    }

    public void SetWeaponsEnabled(bool weaponsEnabled)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon != null)
            {
                weapon.enabled = weaponsEnabled;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0 || Time.time < nextDamageTime)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        nextDamageTime = Time.time + invulnerabilityDuration;

        if (currentHealth == 0)
        {
            SetMovementEnabled(false);
            SetWeaponsEnabled(false);
        }
    }
}
