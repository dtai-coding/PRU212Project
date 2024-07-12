using Assets._Scripts.Enemies;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
	private enum State
	{
		Idle,
		Stun,
		Dead,
		Charging,
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
		facingDirection = 1,
		damageDirection;

	private float
		charge,
		lastShootTime;

	private Vector2 movement;

	private bool
		isCharging,
		isStunned,
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
		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		playerTransform = playerObject.transform;
	}

	private void Update()
	{

		DetectPlayer();

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
			case State.Charging:
				UpdateChargingState();
				break;
			case State.Shooting:
				UpdateShootingState();
				break;
			default:
				break;
		}
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
		isStunned = true;
		isCharging = false;
		charge = 0;
		enemy2Anim.SetBool("Shoot", false);
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
		isStunned = false;
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

	//--Charging State
	private void EnterChargingState()
	{
		Debug.Log("Enter charge State");

		enemy2Anim.SetBool("Shoot", true);
		isCharging = true;
		//charge = 0; // Initialize charge


	}

	private void UpdateChargingState()
	{
		enemy2Anim.SetFloat("Charge", charge);
		if (isCharging)
		{
			if (charge < 3)
			{
				charge += Time.deltaTime;
			}
			else
			{
				if (playerDetected)
				{
					SwitchState(State.Shooting);
				}
			}
		}
	}
	private void ExitChargingState()
	{
		
		Debug.Log("Exit charge State");
	}


	//--Shooting State
	private void EnterShootingState()
	{
		Debug.Log("Enter Shooting State");
		isCharging = false;
		charge = 0;

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
		enemy2Anim.SetBool("Shoot", false);

		SwitchState(State.Idle); // Switch to Idle state after shooting
		Debug.Log("Set to Idle");
	}

	private void UpdateShootingState()
	{
	}

	private void ExitShootingState()
	{
		Debug.Log("Exit Shooting State");

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
			case State.Charging:
				ExitChargingState();
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
			case State.Charging:
				EnterChargingState();
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
		if (playerDetected)
		{
			Vector2 playerDirection = playerTransform.position - Alive.transform.position;
			int newFacingDirection = playerDirection.x > 0 ? 1 : -1;
			if (newFacingDirection != facingDirection)
			{
				Flip();
			}
			if (!isCharging && Time.time >= lastShootTime + shootCooldown && !isStunned)
			{
				SwitchState(State.Charging);
			}
		}

	}

	private void Flip()
	{
		facingDirection *= -1;
		Alive.transform.Rotate(0.0f, 180.0f, 0.0f);
	}

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

	private void OnDrawGizmos()
	{
		//Gizmos.color = Color.clear;
	}
}
