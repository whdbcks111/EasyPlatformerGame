using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator animator;
    private SpriteRenderer renderer;
    private int moveAxis = 0;
    
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        ChangeMove();
    }

    private void Update()
    {
        animator.SetBool("isRunning", Mathf.Abs(rigid.velocity.x) > 0.8);
    }

    private void FixedUpdate()
    {
        RaycastHit2D rayHitX = Physics2D.Raycast(rigid.position + new Vector2(moveAxis, 0), 
            Vector2.down, 3, LayerMask.GetMask("Floor"));
        if (rayHitX.collider == null)
        {
            ChangeMoveAxis(-moveAxis);
        }
        rigid.velocity = new Vector2(moveAxis, rigid.velocity.y + 0.05f);
    }

    void ChangeMoveAxis(int axis)
    {
        moveAxis = axis;
        renderer.flipX = moveAxis > 0;
    }

    void ChangeMove()
    {
        int random = Random.Range(-3, 4);
        ChangeMoveAxis(random == 0 ? 0 : (random < 0 ? -1 : 1));
        Invoke("ChangeMove", 5);
    }
}
