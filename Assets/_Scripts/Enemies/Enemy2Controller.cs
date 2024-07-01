using Assets._Scripts.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    private enum State
    {
        Idle,
        Stun,
        Dead,
        Shooting
    }

    private State currentState;

    [SerializeField]
    private float
        maxHealth,
        knockbackDuration,
        currentHealth,
        knockbackStartTime,
        detectionRange,
        shootForce,
        shootCooldown;

    [SerializeField]
    private Transform
        playerCheck,
        firePoint;

    [SerializeField]
    private GameObject arrowPrefab;

    [SerializeField]
    private LayerMask whatIsPlayer;

    [SerializeField]
    private Vector2 knockbackSpeed;

    private int
        facingDirection,
        damageDirection;

    private float lastShootTime;

    private Vector2 movement;

    private bool
        playerDetected;

    private GameObject Alive;
    private Rigidbody2D enemy2Rb;
    private Animator enemy2Anim;
    private Transform playerTransform;

    private void Start()
    {
        Alive = transform.Find("Alive").gameObject;
        enemy2Rb = Alive.GetComponent<Rigidbody2D>();
        enemy2Anim = Alive.GetComponent<Animator>();
        currentHealth = maxHealth;
        facingDirection = -1;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Stun:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
            /*case State.Shooting:
                UpdateShootingState();
                break;*/
            default:
                DetectPlayer();
                break;
        }
        DetectPlayer();
    }
    //--Idle State
    private void EnterIdleState()
    {

    }
    private void UpdateIdleState()
    {

    }
    private void ExitIdleState()
    {

    }

    //--Knockback State
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        enemy2Rb.velocity = movement;
        enemy2Anim.SetBool("Stun", true);
        Debug.Log("Entry Knockback State");
    }
    private void UpdateKnockbackState()
    {

        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Idle);
        }

    }
    private void ExitKnockbackState()
    {
        enemy2Anim.SetBool("Stun", false);
        Debug.Log("Exited Knockback State");

    }

    //--Dead State
    private void EnterDeadState()
    {
        Destroy(gameObject);
    }
    private void UpdateDeadState()
    {

    }
    private void ExitDeadState()
    {

    }
    //--Shooting State
    private void EnterShootingState()
    {
        enemy2Anim.SetBool("Shoot", true);
        Debug.Log("Enter Shooting State");
            if (Time.time >= lastShootTime + shootCooldown)
        {
                Debug.Log(Time.time);
                Debug.Log(lastShootTime + shootCooldown);
            lastShootTime = Time.time;
            Vector2 direction = (playerTransform.position - firePoint.position).normalized;

            // Instantiate arrow
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

            // Rotate arrow to face shooting direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Apply force to arrow
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            rb.velocity = direction * shootForce;

            // Set the arrow's damage direction if necessary
            ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
            if (arrowScript != null)
            {
                arrowScript.SetDamageDirection(facingDirection);
            }
        SwitchState(State.Idle);
        Debug.Log("Set to Idle");
        }
    }

    private void UpdateShootingState()
    {
        // Ensure the animation completes before exiting the state
            // Calculate direction towards player
           
    }
    private void ExitShootingState()
    {
        enemy2Anim.SetBool("Shoot", false);
        Debug.Log("Exit Shooting State");
    }

    //--Other Functions

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            SwitchState(State.Dead);
        }
        else
        {
            SwitchState(State.Stun);
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        Alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }


    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Idle:
                ExitIdleState();
                break;
            case State.Stun:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
            case State.Shooting:
                ExitShootingState();
                break;
        }

        switch (state)
        {
            case State.Idle:
                EnterIdleState();
                break;
            case State.Stun:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
            case State.Shooting:
                EnterShootingState();
                break;
        }

        currentState = state;
    }

    private void DetectPlayer()
    {
        playerDetected = Physics2D.OverlapCircle(Alive.transform.position, detectionRange, whatIsPlayer);

        if (playerDetected && Time.time >= lastShootTime + shootCooldown)
        {
            Vector2 playerDirection = playerTransform.position - Alive.transform.position;
            int newFacingDirection = playerDirection.x > 0 ? 1 : -1;

            if (newFacingDirection == facingDirection)
            {
                facingDirection = newFacingDirection;

                // Flip the enemy only when facing direction changes
                Flip();

                Debug.Log("Facing direction changed. New direction: " + facingDirection);
                Debug.Log("NewFacing:" + newFacingDirection);
            }

            // Switch to the Shooting state
            SwitchState(State.Shooting);
        }
        else
        {
            // If player is not detected, continue moving
            SwitchState(State.Idle);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.clear;
    }
}
