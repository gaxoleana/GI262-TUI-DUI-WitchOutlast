using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    /*
    moveSpeed ความเร็วเดิน unit/s
    baseHealth พลังชีวิตเริ่มต้น
    currentHealth พลังชีวิตปัจจุบัน
    invulnerabilityDuration ระยะเวลาที่ไม่สามารถรับความเสียหายได้หลังจากถูกโจมตี
    weapons รายการอาวุธที่ผู้เล่นถืออยู่
    */

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private List<Weapon> weapons = new List<Weapon>();
    [SerializeField, Min(1)] private int baseExpRequired = 100;
    [SerializeField, Min(0)] private int expIncreasePerLevel = 50;

    private int level = 1;
    private int currentExp;
    private int expRequired;

    /*
    CurrentHealth เป็น property ที่ใช้ในการเข้าถึงค่าพลังชีวิตปัจจุบันของผู้เล่น
    */

    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
    }

    public int MaxHealth
    {
        get
        {
            return baseHealth;
        }
    }

    public int Level
    {
        get
        {
            return level;
        }
    }

    public int CurrentExp
    {
        get
        {
            return currentExp;
        }
    }

    public int ExpRequired
    {
        get
        {
            return expRequired;
        }
    }

    /*
    moveAction เป็น InputAction ที่ใช้รับค่าการเดินของผู้เล่น
    playerBody เป็นตัวแปรที่เก็บ Rigidbody2D ของผู้เล่น
    moveDirection เป็นตัวแปรที่เก็บทิศทางการเคลื่อนที่
    movementEnabled เป็นตัวแปรที่ใช้ในการเปิด/ปิดการเคลื่อนที่
    nextDamageTime เป็นตัวแปรที่เก็บเวลาที่ผู้เล่นสามารถรับความเสียหายได้อีกครั้งหลังจากถูกโจมตี
    */

    private InputAction moveAction;
    private Rigidbody2D playerBody;
    private Vector2 moveDirection;
    private bool movementEnabled;
    private float nextDamageTime;

    /*
    Awake() ถูกเรียกครั้งเดียวต่อ instance เพื่อเตรียมค่าเริ่มต้นเมื่อ GameObject อยู่ในสถานะ active
    ถ้าสร้าง GameObject ขณะ inactive จะรอจนถูกเปิดใช้งานครั้งแรก
    OnEnable() ถูกเรียกเมื่อ Component นี้เปิดใช้งานบน GameObject ที่ active ใน Hierarchy
    OnDisable() ถูกเรียกเมื่อ Component นี้หรือ GameObject ถูกปิดใช้งาน
    Update() เป็นฟังก์ชันที่ถูกเรียกทุกเฟรมเพื่ออัปเดตค่าต่าง ๆ
    FixedUpdate() ถูกเรียกตามช่วงเวลาฟิสิกส์คงที่ อาจเกิด 0 ครั้งหรือหลายครั้งต่อเฟรมภาพ
    SetMovementEnabled(bool enabled) เป็นฟังก์ชันที่ใช้ในการเปิด/ปิดการเคลื่อนที่ของผู้เล่น
    SetWeaponsEnabled(bool weaponsEnabled) เป็นฟังก์ชันที่ใช้ในการเปิด/ปิดการใช้งานอาวุธของผู้เล่น
    TakeDamage(int damage) เป็นฟังก์ชันที่ใช้ในการลดค่าพลังชีวิตของผู้เล่นเมื่อถูกโจมตี
    */

    private void Awake()
    {
        /*
        currentHealth ถูกตั้งค่าเป็น baseHealth เพื่อให้ผู้เล่นเริ่มต้นด้วยพลังชีวิตเต็ม
        playerBody ถูกกำหนดเป็น Rigidbody2D ของผู้เล่นเพื่อใช้ในการเคลื่อนที่
        moveAction ถูกกำหนดเป็น InputAction สำหรับการเคลื่อนที่ของผู้เล่น
        */

        currentHealth = baseHealth;
        ResetProgression();
        playerBody = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        //เปิดรับ Input การเดินผ่าน moveAction โดยการเดินจริงยังขึ้นอยู่กับ movementEnabled

        moveAction.Enable();
    }

    private void OnDisable()
    {
        //ปิดใช้งาน moveAction และหยุดการเคลื่อนที่ของผู้เล่น
        //set ค่า linearVelocity ของ playerBody เป็น Vector2.zero เพื่อหยุดการเคลื่อนที่

        moveAction.Disable();
        playerBody.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        //อ่าน Input การเดินและจำกัดความยาวของเวกเตอร์ไม่ให้เกิน 1 เพื่อกันเดินทแยงเร็วกว่าเดินตรง
        //ค่าที่มีความยาวต่ำกว่า 1 จะยังคงเดิม

        Vector2 inputDirection = moveAction.ReadValue<Vector2>();
        moveDirection = Vector2.ClampMagnitude(inputDirection, 1f);
    }

    private void FixedUpdate()
    {
        /*
        ถ้า movementEnabled เป็น true ให้คำนวณความเร็วในการเคลื่อนที่ของผู้เล่นโดยใช้ moveDirection และ moveSpeed
        และกำหนดค่า linearVelocity ของ playerBody เป็น moveVelocity
        ถ้า movementEnabled เป็น false ให้กำหนดค่า linearVelocity ของ playerBody เป็น Vector2.zero เพื่อหยุดการเคลื่อนที่
        */

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
        /*
        ถ้า enabled เป็น true ให้เปิดการเคลื่อนที่ของผู้เล่น
        ถ้า enabled เป็น false ให้ปิดการเคลื่อนที่ของผู้เล่นและหยุดการเคลื่อนที่โดยกำหนดค่า linearVelocity ของ playerBody เป็น Vector2.zero
        */

        movementEnabled = enabled;

        if (!enabled)
        {
            playerBody.linearVelocity = Vector2.zero;
        }
    }

    public void SetWeaponsEnabled(bool weaponsEnabled)
    {
        /*
        ถ้า weaponsEnabled เป็น true ให้เปิดการใช้งานอาวุธของผู้เล่น
        ถ้า weaponsEnabled เป็น false ให้ปิดการใช้งานอาวุธของผู้เล่น
        */

        foreach (Weapon weapon in weapons)
        {
            if (weapon != null)
            {
                weapon.enabled = weaponsEnabled;
            }
        }
    }

    public void ResetProgression()
    {
        level = 1;
        currentExp = 0;
        expRequired = Mathf.Max(1, baseExpRequired);
    }

    public void GainExp(int amount)
    {
        if (amount <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentExp += amount;

        while (currentExp >= expRequired)
        {
            currentExp -= expRequired;
            level++;
            expRequired += Mathf.Max(0, expIncreasePerLevel);
        }
    }

    public void TakeDamage(int damage)
    {
        /*
        ถ้า damage น้อยกว่าหรือเท่ากับ 0 หรือ currentHealth น้อยกว่าหรือเท่ากับ 0 หรือเวลาปัจจุบันน้อยกว่า nextDamageTime ให้ return ออกจากฟังก์ชัน
        ลดค่า currentHealth ลงด้วย damage โดยไม่ให้ต่ำกว่า 0 ไม่ได้จำกัดค่าสูงสุดไว้ที่ baseHealth
        กำหนดค่า nextDamageTime เป็นเวลาปัจจุบันบวกกับ invulnerabilityDuration
        ถ้า currentHealth เท่ากับ 0 ให้ปิดการเคลื่อนที่และการใช้งานอาวุธของผู้เล่น
        */

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
