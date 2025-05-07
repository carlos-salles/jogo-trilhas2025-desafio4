using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Min(0.1f)]
    float peakHeight;
    [SerializeField, Min(0.1f)]
    float peakTime;
    [SerializeField, Min(0f)]
    float moveSpeed;
    [SerializeField, Min(0.1f)]
    float jumpInputTime;

    float jumpSpeed;
    float gravityAccel;
    float distanceToGround;
    Rigidbody2D rb;

    float moveInput;
    float jumpInputCounter;
    bool jumpInput;


    private void Awake()
    {
        jumpSpeed = 2 * peakHeight / peakTime;
        gravityAccel = jumpSpeed / peakTime;

        distanceToGround = GetComponent<Collider2D>().bounds.extents.y;

        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {   
            
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpInputCounter = jumpInputTime;
        }
        else
        {
            jumpInputCounter -= Mathf.Max(0f, jumpInputCounter - Time.deltaTime);
        }
        jumpInput = jumpInputCounter > 0;
    }

    void FixedUpdate()
    {
        float movementX = moveInput * moveSpeed;

        if (IsGrounded() && jumpInput)
        {
            Debug.Log("JUMP");
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            jumpInputCounter = 0f;
        }
        else
        {
            rb.AddForce(Vector2.down * gravityAccel);
        }

        rb.velocity = new Vector2(movementX, rb.velocity.y);
    }

    bool IsGrounded()
    {
        Vector2 origin = transform.position;
        float distance = distanceToGround + 0.1f;
        LayerMask mask = LayerMask.GetMask("Default");
        RaycastHit2D raycastHit = Physics2D.Raycast(origin, Vector2.down, distance, mask);
        bool grounded = raycastHit.collider is not null;
        Debug.Log(grounded);
        return grounded;
    }
}
