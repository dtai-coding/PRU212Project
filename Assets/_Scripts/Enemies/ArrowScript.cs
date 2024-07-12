using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

namespace Assets._Scripts.Enemies
{
	public class ArrowScript : MonoBehaviour
	{
		public Rigidbody2D rb;
		public Collider2D arrowCollider;

		void OnCollisionEnter2D(Collision2D collision)
		{
			// Check if the arrow collides with the player

			if (collision.gameObject.layer == LayerMask.NameToLayer("player"))
			{
				Debug.Log("Arrow collision with player");
				Player player = collision.transform.GetComponent<Player>();
				if (player != null)
				{
					player.Die();
					Debug.Log("Arrow collied with Player");
					Destroy(gameObject);
				}
			}
			else if (collision.gameObject.layer == LayerMask.NameToLayer("Arrow"))
			{
				InstanceDestroyArrow();
			}
			DestroyArrow();
		}
		private void DestroyArrow()
		{
			arrowCollider.enabled = false;
			rb.velocity = Vector2.zero;
			rb.gravityScale = 0;
			StartCoroutine(CountdownAndDestroy());
		}

		IEnumerator CountdownAndDestroy()
		{
			// Wait for 3 seconds
			yield return new WaitForSeconds(3f);

			// Destroy the GameObject
			Destroy(gameObject);
		}
		public void InstanceDestroyArrow()
		{
			Destroy(gameObject);
		}
	}

}
