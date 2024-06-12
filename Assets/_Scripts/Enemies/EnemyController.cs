using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Knockback,
        Dead
    }

    private State currentState;

    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration;

    [SerializeField]
    private Transform
        groundCheck,
        WallCheck;

    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private Vector2 knockbackSpeed;

    private float 
        currentHealth,
        knockbackStartTime;

    private int 
        facingDirection,
        damageDirection;

    private Vector2 movement;

    private bool
        groundDetected,
        wallDectected;

    private GameObject enemy1;
    private Rigidbody2D enemy1Rb;
    private Animator enemy1Anim;

    private void Start()
    {
        enemy1 = transform.Find("enemy1").gameObject;
        enemy1Rb = enemy1.GetComponent<Rigidbody2D>();
        enemy1Anim = GetComponent<Animator>();

        facingDirection = 1;
    }

    private void Update()
    {
        switch(currentState)
        {
            case State.Moving:
                UpdateMovingState(); 
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState(); 
                break;
        }
    }
    //--Moving State
    private void EnterMovingState()
    {
        
    }
    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDectected = Physics2D.Raycast(WallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if (!groundDetected || wallDectected)
        {
            //Flip
            Flip();
        } 
        else
        {
            //Move
            movement.Set(movementSpeed * facingDirection, enemy1Rb.velocity.y);
            enemy1Rb.velocity = movement;
        }
    }
    private void ExitMovingState()
    {

    }

    //--Knockback State
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        enemy1Rb.velocity = movement;
        enemy1Anim.SetBool("Knockback", true);
    }
    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }
    private void ExitKnockbackState()
    {
        enemy1Anim.SetBool("Knockback", false);

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

    //--Other Functions

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        if (attackDetails[1] > enemy1.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        //Hit particle

        if (currentHealth > 0.0f)
        {
            SwitchState(State.Knockback);
        } 
        else if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        enemy1.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(WallCheck.position, new Vector2(WallCheck.position.x + wallCheckDistance, WallCheck.position.y));
    }
}
