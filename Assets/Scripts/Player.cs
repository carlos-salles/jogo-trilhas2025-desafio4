using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    [SerializeField, Min(1f)]
    float maxFallSpeedFactor;
    [SerializeField]
    Sprite jumpSpriteUp;
    [SerializeField]
    Sprite jumpSpriteDown;
    [SerializeField]
    Sprite attackSprite;

    float jumpSpeed;
    float maxFallSpeed;
    float gravityAccel;
    float distanceToGround;
    float colliderWidth;
    Rigidbody2D rb;

    Vector3 lastPosition;
    float moveInput;
    float jumpInputTimer;
    bool jumpPressed;

    Animator animator;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    SlingShot slingShot;
    Transform arms;
    Transform shootPoint;
    Transform coolDownBar;
    Vector3 scale;

    private void Awake()
    {
        jumpSpeed = 2 * peakHeight / peakTime;
        maxFallSpeed = maxFallSpeedFactor * jumpSpeed;
        gravityAccel = jumpSpeed / peakTime;

        distanceToGround = GetComponent<Collider2D>().bounds.extents.y;
        colliderWidth = GetComponent<Collider2D>().bounds.size.x;

        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        scale = transform.localScale;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        arms = transform.Find("Arms");
        shootPoint = transform.Find("ShootPoint");
        coolDownBar = transform.Find("CoolDownBar");
    }

    void Update()
    {
        if (slingShot.shooting)
        {
            moveInput = 0f;
            jumpPressed = false;
            return;
        }

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

        if (moveInput != 0)
        {
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput);
            transform.localScale = scale;
        }

        if (slingShot.coolDownRecharge)
        {
            coolDownBar.gameObject.SetActive(true);
            if (scale.x < 0)
            {
                coolDownBar.localScale = -Vector3.one;
            }
            else
            {
                coolDownBar.localScale = Vector3.one;
            }
        }
        else
        {
            coolDownBar.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        bool grounded = IsGrounded();

        if (slingShot.shooting)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            jumpInputTimer = 0f;
            animator.SetBool("running", false);
            rb.AddForce(Vector2.down * gravityAccel);
            HandleVisuals(grounded);
            return;
        }

        float movementX = moveInput * moveSpeed;
        rb.velocity = new Vector2(movementX, rb.velocity.y);

        if (grounded)
        {
            animator.enabled = true;

            if (movementX != 0)
            {
                animator.SetBool("running", true);
            }
            else
            {
                animator.SetBool("running", false);
            }
        }
        else
        {
            animator.enabled = false;

            if (rb.velocity.y >= 0)
            {
                spriteRenderer.sprite = jumpSpriteUp;
            }
            else
            {
                spriteRenderer.sprite = jumpSpriteDown;
            }
        }

        if (!jumpPressed && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (grounded && jumpInputTimer > 0f)
        {
            Debug.Log("JUMP");
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            jumpInputTimer = 0f;
        }
        else
        {
            if (rb.velocity.y <= -maxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
            }
            else
            {
                rb.AddForce(Vector2.down * gravityAccel);
            }
        }
        Debug.Log($"current y-speed {rb.velocity.y}; terminal y-speed {maxFallSpeed}");
        HandleVisuals(grounded);

        void HandleVisuals(bool grounded)
        {
            if (slingShot.shooting)
            {
                animator.enabled = false;
                spriteRenderer.sprite = attackSprite;
                arms.gameObject.SetActive(true);

                float angle = Mathf.Atan2(slingShot.launchDirection.y, slingShot.launchDirection.x) * Mathf.Rad2Deg;

                if (angle > 90f || angle < -90f)
                {
                    scale.x = -Mathf.Abs(scale.x);
                }
                else
                {
                    scale.x = Mathf.Abs(scale.x);
                }

                transform.localScale = scale;

                arms.localScale = Vector3.one;
                shootPoint.localScale = Vector3.one;

                if (scale.x < 0)
                {
                    arms.localRotation = Quaternion.Euler(0f, 0f, 180f - angle);
                    shootPoint.localRotation = Quaternion.Euler(0f, 0f, 180f - angle);
                }
                else
                {
                    arms.localRotation = Quaternion.Euler(0f, 0f, angle);
                    shootPoint.localRotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
            else
            {
                arms.gameObject.SetActive(false);

                if (grounded)
                {
                    animator.enabled = true;
                }
                else
                {
                    animator.enabled = false;
                    spriteRenderer.sprite = rb.velocity.y >= 0 ? jumpSpriteUp : jumpSpriteDown;
                }
            }
        }

    }

    bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.down * (distanceToGround + 0.1f);
        Vector2 size = new Vector2(colliderWidth, 0.1f);
        LayerMask mask = LayerMask.GetMask("Default");
        var castHit = Physics2D.BoxCast(origin, size, 0f, Vector2.zero, Mathf.Infinity, mask);
        return castHit.collider is not null;
    }
}