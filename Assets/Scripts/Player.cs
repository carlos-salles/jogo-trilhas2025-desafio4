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
    float jumpBufferingTime;

    float jumpSpeed;
    float gravityAccel;
    float distanceToGround;
    Rigidbody2D rb;

    Vector3 lastPosition;
    float moveInput;
    float jumpInputTimer;
    bool jumpPressed;



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
        jumpPressed = Input.GetButton("Jump");

        if (Input.GetButtonDown("Jump"))
        {
            jumpInputTimer = jumpBufferingTime;
        }
        else
        {
            jumpInputTimer = Mathf.Max(0f, jumpInputTimer - Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        float movementX = moveInput * moveSpeed;
        rb.velocity = new Vector2(movementX, rb.velocity.y);
        
        if (!jumpPressed && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y*0.5f);
        }

        if (IsGrounded() && jumpInputTimer > 0f)
        {
            Debug.Log("JUMP");
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            jumpInputTimer = 0f;
        }
        else
        {
            rb.AddForce(Vector2.down * gravityAccel);
        }

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
