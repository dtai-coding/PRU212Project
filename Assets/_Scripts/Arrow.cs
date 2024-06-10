using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    public int bounceCount;
    private bool hasHit;

    void OnEnable()
    {
        StartCoroutine(DisableAfterTime(ArrowPool.Instance.arrowLifeTime));
        rb.gravityScale = 1;
        hasHit = false;
        Debug.Log("Arrow enabled. Arrow bounce count: " + bounceCount);
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
        bounceCount--;
        Debug.Log("Arrow collided. Arrow bounce count after collision: " + bounceCount);
        if (bounceCount <= 0)
        {
            hasHit = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
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
