using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float movementInputDirection;
    private float jumpTimer = 0f;
    private float turnTimer = 0f;
    private float wallJumpTimer = 0f;
    private float slowTimer = 0f;
    private float timeSlowCooldownTimer = 0f;
    private float chargeTime = 0f;

    private int facingDirection = 1;
    private int lastWallJumpDirection;
    private int currentBounceIndex = 0;


    private bool isFaceRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canNormalJump = true;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isDashing;
    private bool isCharging = false;
    private bool isTimeSlow = false;


    public float Speed = 10.0f;
    public float JumpForce = 16.0f;
    public float groundCheckRadius = 0.35f;
    public float wallCheckDistance = 0.5f;
    public float wallSlideSpeed = 2f;
    public float airMoveForce = 50f;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeight = 0.5f;
    public float wallJumpForce = 20f;
    public float jumpTimerSet = 0.15f;
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    public float slowTimerSet = 1f;
    public float timeSlowCoolDown = 5f;
    public float dashDistance = 25f;
    public float dashDuration = 0.2f;
    public float chargePower = 30f;
    public float chargeTimeSet = 2f;

    public int[] bounceAmounts = new int[] { 1, 2, 3 };
    public int bounceCount;

    public static Player Instance { get; private set; }

    public Vector2 wallHopDirection = new Vector2(1f, 0.5f);
    public Vector2 wallJumpDirection = new Vector2(1f, 2f);

    public GameObject arrowPrefab;
    public GameObject pointer;
    private Rigidbody2D rb;
    private Animator anim;
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform firePoint;

    public LayerMask whatIsGround;

    void Awake()
    {
        Debug.Log("Player Awake. Initial bounce count: " + bounceCount);
        Instance = this;
        bounceCount = bounceAmounts[currentBounceIndex];
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        pointer = GameObject.Find("Pointer");
        pointer.SetActive(false);
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        CheckInput();
        CheckMovementDirection();
        UpdateAnim();
        CheckIfCanJump();
        CheckIfWallSlide();
        CheckJump();
        PointerDirection();
        CheckDash();
        CheckCharge();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        ApplyMovement();
        CheckSurrounding();
    }

    private void CheckIfWallSlide()
    {
        if (!isGrounded && isTouchingWall)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckSurrounding()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            canNormalJump = true;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }
    }
    private void CheckMovementDirection()
    {
        if (!isCharging)
        {
            if (isFaceRight && movementInputDirection < 0)
            {
                Flip();
            }
            else if (!isFaceRight && movementInputDirection > 0)
            {
                Flip();
            }
        }
        else
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePosition.x < transform.position.x && isFaceRight)
            {
                Flip();
            }
            else if (mousePosition.x > transform.position.x && !isFaceRight)
            {
                Flip();
            }
        }

        const float velocityThreshold = 0.01f;
        if (Mathf.Abs(rb.velocity.x) < velocityThreshold)
        {
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }
    }

    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            { WallHop(); }
            else if (isGrounded)
            { NormalJump(); }
        }

        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }


    public void CheckDash()
    {
        timeSlowCooldownTimer -= Time.deltaTime;
        if (timeSlowCooldownTimer <= 0)
        {
            timeSlowCooldownTimer = 0;
        }
    }

    public void CheckCharge()
    {
        if (isCharging)
        {
            canMove = false;
            if (isGrounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // Stop moving if grounded
            }
            if (isTimeSlow)
            {
                chargeTime = chargeTimeSet;
            }
            else
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Min(chargeTime, chargeTimeSet);
            }

            // Allow slowTimer to decrease even when charging
            if (isTimeSlow && slowTimer > 0)
            {
                slowTimer += Time.deltaTime;
                if (slowTimer >= slowTimerSet)
                {
                    isTimeSlow = false;
                    Time.timeScale = 1f;
                    pointer.SetActive(false);
                    FireArrow();
                    slowTimer = 0f;
                    timeSlowCooldownTimer = timeSlowCoolDown;
                }
            }
        }
    }


    private void UpdateAnim()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isCharging", isCharging);
        anim.SetFloat("chargeTime", chargeTime);
    }

    private void CheckInput()
    {

        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && isTouchingWall)
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;
            }
        }

        if (!canMove)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }
        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeight);
        }

        if (Input.GetKey(KeyCode.LeftShift) && timeSlowCooldownTimer == 0 && !isCharging)
        {
            if (slowTimer < slowTimerSet)
            {
                isTimeSlow = true;
                slowTimer += Time.deltaTime;
                TimeSlow();
            }
            else
            {
                isTimeSlow = false;
                if (isCharging)
                {
                    Time.timeScale = 1f;
                    pointer.SetActive(false);
                    FireArrow();
                    slowTimer = 0f;
                    timeSlowCooldownTimer = timeSlowCoolDown;
                }
                else
                {
                    Time.timeScale = 1f;
                    pointer.SetActive(false); 
                    StartCoroutine(Dash());
                    slowTimer = 0f;
                    timeSlowCooldownTimer = timeSlowCoolDown;
                }

            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && slowTimer > 0)
        {
            isTimeSlow = false;
            if (isCharging)
            {
                Time.timeScale = 1f;
                pointer.SetActive(false);
                FireArrow();
                slowTimer = 0f;
                timeSlowCooldownTimer = timeSlowCoolDown;
            }
            else
            {
                Time.timeScale = 1f;
                pointer.SetActive(false);
                StartCoroutine(Dash());
                slowTimer = 0f;
                timeSlowCooldownTimer = timeSlowCoolDown;
            }
        }

        if (Input.GetMouseButtonDown(0) && !isWallSliding && !isDashing)
        {
            isCharging = true;
            chargeTime = 0f;
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        { 
            FireArrow();
            if (isTimeSlow)
            {
                isTimeSlow = false;
                Time.timeScale = 1f;
                pointer.SetActive(false);
                FireArrow();
                slowTimer = 0f;
                timeSlowCooldownTimer = timeSlowCoolDown;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Cycle through the bounce amounts
            currentBounceIndex = (currentBounceIndex + 1) % bounceAmounts.Length;
            bounceCount = bounceAmounts[currentBounceIndex];
            Debug.Log("Right mouse button clicked. Current bounce count: " + bounceCount);
        }
    }
    public void FireArrow()
    {
        canMove = true;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized;
        Quaternion correctedRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 45);
        Arrow arrow = ArrowPool.Instance.GetArrow();
        if (arrow != null)
        {
            arrow.transform.position = firePoint.position;
            arrow.transform.rotation = correctedRotation;
            arrow.rb.velocity = direction * chargeTime * chargePower;
            arrow.bounceCount = bounceCount;
            Debug.Log("Arrow fired. Arrow bounce count: " + arrow.bounceCount);
        }
        isCharging = false;
        if (isTimeSlow)
        {
            isTimeSlow = false;
            Time.timeScale = 1f;
            pointer.SetActive(false);
            slowTimer = 0f;
            timeSlowCooldownTimer = timeSlowCoolDown;
        }
    }

    public void TimeSlow()
    {
        Time.timeScale = 0.5f;
        pointer.SetActive(true);
    }


    public IEnumerator Dash()
    {
        if (isCharging)
        {
            yield break; // Don't allow dashing if charging an arrow
        }

        if (timeSlowCooldownTimer == 0)
        {
            Time.timeScale = 1f;
            pointer.SetActive(false);
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            Vector2 direction = Quaternion.Euler(0, 0, 45) * pointer.transform.right;
            rb.velocity = direction * dashDistance;
            yield return new WaitForSeconds(dashDuration);
            rb.velocity = Vector2.zero;
            rb.gravityScale = originalGravity;
        }
        isDashing = false;
    }

    public void PointerDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)pointer.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pointer.transform.rotation = Quaternion.Euler(0f, 0f, angle - 45f);
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }

    private void WallHop()
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;

        }
    }

    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);

        }
        else if (canMove)
        {
            rb.velocity = new Vector2(Speed * movementInputDirection, rb.velocity.y);
        }

        if (isWallSliding == true)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFaceRight = !isFaceRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.clear;

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}