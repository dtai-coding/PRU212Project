using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Collider2D arrowCollider;
    public Rigidbody2D rb;
    public int bounceCount;
    private bool hasHit;

    void OnEnable()
    {
        StartCoroutine(DisableAfterTime(ArrowPool.Instance.arrowLifeTime));
        rb.gravityScale = 1;
        hasHit = false;
    }

    IEnumerator DisableAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        ArrowPool.Instance.ReturnToPool(this);
    }

    void Update()
    {
        ArrowAngle();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided game object's layer is "Enemy"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Try to get the EnemyController component from the parent of enemy1
            EnemyController enemy = collision.transform.parent.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // Assuming each arrow deals 1 damage
                Debug.Log("Arrow collided with enemy");
            }
            ArrowPool.Instance.ReturnToPool(this);
        }
        else
        {
            bounceCount--;
            if (bounceCount <= 0)
            {
                arrowCollider.enabled = false;
                hasHit = true;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
            }
        }
    }

    public void ArrowAngle()
    {
        if (!hasHit)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

    }
}
