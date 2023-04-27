using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    
    public float moveSpeed;
    public float jumpForce;
    public int curHp;
    public int maxHp;
    private int damage = 5;
    private float attackRange =2.5f;
    
    private bool isAttacking;

    public Rigidbody playerRb;
    public Animator playerAnim;

    private void Start()
    {
        // get the player component
        playerRb = GetComponent<Rigidbody>();
        // get the component animator who is in the player component under the PlayerAnim object
        playerAnim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        if(Input.GetMouseButton(0) && !isAttacking)
        {
            Attack();
        }
        if(!isAttacking)
        {
            UpdateAnimator();
        }
    }

    void UpdateAnimator()
    {
        // set the anim to false
        playerAnim.SetBool("MovingForwards", false);
        playerAnim.SetBool("MovingBackwards", false);
        playerAnim.SetBool("MovingLeft", false);
        playerAnim.SetBool("MovingRight", false);

        // Get the local direction we're going
        Vector3 localVel = transform.InverseTransformDirection(playerRb.velocity);

        // if the player is moving forwards
        if(localVel.z > 0.1f)
        {
            // set the anim to true
            playerAnim.SetBool("MovingForwards", true);
        }
        // if the player is moving backwards
        else if(localVel.z < -0.1f)
        {
            // set the anim to true
            playerAnim.SetBool("MovingBackwards", true);
        }
        // if the player is moving left
        else if(localVel.x < -0.1f)
        {
            // set the anim to true
            playerAnim.SetBool("MovingLeft", true);
        }
        // if the player is moving right
        else if(localVel.x > 0.1f)
        {
            // set the anim to true
            playerAnim.SetBool("MovingRight", true);
        }

    }

    void Move()
    {
        // Detecting the inputs from horizontal movement
        float horizontalInput = Input.GetAxis("Horizontal");
        // Detecting the inputs from vertical movement
        float verticalInput = Input.GetAxis("Vertical");

        // get  a relative direction based on where our player is facing
        Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
        // apply movespeed and gravity
        dir *= moveSpeed;
        dir.y = playerRb.velocity.y;

        // apply the velocity to the player
        playerRb.velocity = dir;
    }

    void Jump()
    {
        // if the player presses the space bar and the player is on the ground
        if(Input.GetKeyDown(KeyCode.Space) && CanJump())
        {
            // add jumpForce to the player's rigidbody
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // play the animation of jump
            playerAnim.SetTrigger("Jump");
        }
    }

    bool CanJump()
    {
        // create a ray from the player position towards the ground
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        // if the ray casted for the given distance (0.1f) has hit something,
        if(Physics.Raycast(ray, out hit, 0.1f))
        {
            // return true
            return true;
        }
        // otherwise return false
        return false;

    }

    public void TakeDamage(int damageToTake)
    {
        curHp -= damageToTake;

        // update the health bar
        HealthBarUI.instance.UpdateFill(curHp, maxHp);

        if(curHp < 0)
        {
            // restart the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Attack()
    {
        isAttacking = true;
        playerAnim.SetTrigger("Attack");
        // stop all the other anim
        playerAnim.SetBool("MovingForwards", false);
        playerAnim.SetBool("MovingBackwards", false);
        playerAnim.SetBool("MovingLeft", false);
        playerAnim.SetBool("MovingRight", false);

        Invoke(nameof(TryDamage), 0.5f);
        Invoke(nameof(DisableIsAttaking), 1.5f);
    }

    void TryDamage()
    {
        // create a ray one meter in front of the player
        Ray ray = new Ray(transform.position + transform.forward, transform.forward);

        // cast a sphere and look for all colliders inside it, which are on the "enemy" layer (8)
        RaycastHit[] hits = Physics.SphereCastAll(ray, 1f, attackRange, 1 << 8);

        foreach(RaycastHit hit in hits)
        {
            hit.collider.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }

    void DisableIsAttaking()
    {
        isAttacking = false;
    }
}
