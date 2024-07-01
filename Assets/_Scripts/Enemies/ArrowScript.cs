using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Enemies
{
    public class ArrowScript : MonoBehaviour
    {
        private int damageDirection;
        public int damage = 10; // Adjust damage value as needed
        public Rigidbody2D rb;
        public void SetDamageDirection(int direction)
        {
            damageDirection = direction;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if the arrow collides with the player
            
                if (collision.gameObject.layer == LayerMask.NameToLayer("player"))
                {
                    Debug.Log("Arrow collision with player");
                    Player player = collision.transform.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamage();
                        Debug.Log("Arrow collied with Player");
                    Destroy(gameObject);
                }
                } else if (collision.gameObject.layer == LayerMask.NameToLayer("platform"))
                {
                    Destroy(gameObject);
                }
            Destroy(gameObject);
        }
    }

}
