using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Player player;
    [Header("Enemy Stats")]
    public float attackDistance;
    public float chaseDistance;
    public int damage;
    public int health;
    //public float attackTimer = 1.3f;
    //public float attackCooldown = 2.66f;

    private bool isAttacking;
    private bool isDead;

    public NavMeshAgent agent;
    public Animator enemyAnim;

    private void Start()
    {
        // get the player component
        player = FindObjectOfType<Player>();
        // get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        // get the component animator who is in the enemy component under the EnemyAnim object
        enemyAnim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the enemy is currently dead, Stop the function
        if(isDead)
        {
            return;
        }

        // get the distance between the enemy and the player
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // if the enemy is currently outside the chaseRange
        if(distance > chaseDistance)
        {
            agent.isStopped = true;
            // play the anim for idle
            enemyAnim.SetBool("Running", false);
            return;
        }
        // if the enemy is within the chaseRange but outside the attackRange
        else if(distance < chaseDistance && distance > attackDistance)
        {
            // if so start moving the enemy towards the player
            agent.isStopped = false;
            // play the anim for running
            enemyAnim.SetBool("Running", true);
            agent.SetDestination(player.transform.position);
        }
        // if the enemy is within the attackRange
        else if(distance < attackDistance)
        {
            // if so start attacking the player
            agent.isStopped = true;
            // stop the anim for running
            enemyAnim.SetBool("Running", false);
            if(!isAttacking)
            {
                Attack();
            }
        }

    }
    void Attack()
    {
        isAttacking = true;
        // anim the attack
        enemyAnim.SetTrigger("Attack");
        // there will be a attachTimer delay between each attack
        Invoke(nameof(TryDamage), 1.3f);

        // there will be an attackCoolDown between each call;
        Invoke(nameof(DisableIsAttacking), 2.66f);
    }

    void TryDamage()
    {
        // Is the player in front of us?
        if(Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            // if so, damage the player
            player.TakeDamage(damage);
        }
    }

    void DisableIsAttacking()
    {
        // set isAttacking to false
        isAttacking = false;
    }

    public void TakeDamage( int damageToTake)
    {
        health -= damageToTake;
        Debug.Log("takeDamage enemy");

        // if health is 0, set isDead to true and stop the navMesh agent
        if(health <= 0)
        {
            isDead = true;
            agent.isStopped = true;
            // start the Die coroutine
            StartCoroutine(Die());
        }
    }

    // coroutine to wait for a few seconds before destroying the enemy
    IEnumerator Die()
    {
        // play the death anim
        enemyAnim.SetTrigger("Die");
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

}
