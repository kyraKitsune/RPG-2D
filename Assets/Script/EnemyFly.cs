using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // Pour le pathfinding

public class EnemyFly : EnemyStats // Enemy sera enfantt du script EnemyStats
{

    public Transform attackPoint;
    public LayerMask playerLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        InitializeCharacter(OnAttack);
    }

    void OnAttack()
    {
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 1f));

            if (dist < attackRange)
            {
                anim.SetTrigger("attack");
                rb.velocity = Vector2.zero; // Pour pas que le joueur bouge pendant l'attaque
            }
        }

        //attack
        lastAttackTime = Time.time;
    }

    void Attack()
    {
        Collider2D player = Physics2D.OverlapCircle(attackPoint.transform.position, 0.5f, playerLayerMask);
        
        if(player != null && player.tag == "Player")
        {
            player.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
