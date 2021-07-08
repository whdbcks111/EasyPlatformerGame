using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    private Rigidbody2D rigidbody;
    private SpriteRenderer renderer;
    private Animator animator;
    private bool canJump = false;
    public float jumpForce;
    public float maxSpeed;
    public float moveForce;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.position.y < -20)
        {
            SceneManager.LoadScene(0);
        }
        if(Input.GetAxis("Horizontal") != 0)
            renderer.flipX = Input.GetAxis("Horizontal") < 0;
        
        if (Input.GetButtonUp("Horizontal"))
        {
            rigidbody.velocity = new Vector2(
                rigidbody.velocity.normalized.x * 0.4f, rigidbody.velocity.y);
        }
        
        rigidbody.velocity = new Vector2(
            Mathf.Lerp(rigidbody.velocity.x, 0, 0.1f), rigidbody.velocity.y);

        if (Input.GetButton("Jump") && canJump)
        {
            Jump();
        }

        animator.SetBool("isRunning", Mathf.Abs(rigidbody.velocity.x) > 0.8);
    }

    public void Jump()
    {
        animator.SetBool("isJumping", true);
        canJump = false;
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Enemy")
        {
            OnDamage(other.gameObject.transform.position);
        }
    }

    void OnDamage(Vector2 otherPos)
    {
        animator.SetBool("isJumping", true);
        canJump = false;
        float force = otherPos.x - transform.position.x > 0 ? -1 : 1;
        force *= jumpForce * 0.7f;
        rigidbody.AddForce(new Vector2(force, jumpForce * 0.8f), ForceMode2D.Impulse);
        renderer.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        gameObject.layer = LayerMask.NameToLayer("DamagedPlayer");
        Invoke("ExitDamaged", 2);
    }

    void ExitDamaged()
    {
        renderer.color = new Color(1f, 1f, 1f, 1f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Horizontal"))
        {
            float h = Input.GetAxis("Horizontal");
            rigidbody.AddForce(Vector2.right * (h * moveForce), ForceMode2D.Impulse);
            if (rigidbody.velocity.x > maxSpeed)
                rigidbody.velocity = new Vector2(maxSpeed, rigidbody.velocity.y);
            if (rigidbody.velocity.x < -maxSpeed)
                rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
        }

        if (rigidbody.velocity.y == 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigidbody.position, Vector2.down,
                2, LayerMask.GetMask("Floor", "Enemy"));
            if (rayHit.collider != null && rayHit.distance < 1.5f)
            {
                canJump = true;
                animator.SetBool("isJumping", false);
            }
        }
    }
}
