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
    [SerializeField]
    Sprite jumpSpriteUp;
    [SerializeField]
    Sprite jumpSpriteDown;
    [SerializeField]
    Sprite attackSprite;

    float jumpSpeed;
    float gravityAccel;
    float distanceToGround;
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
    Vector3 scale;

    private void Awake()
    {
        jumpSpeed = 2 * peakHeight / peakTime;
        gravityAccel = jumpSpeed / peakTime;

        distanceToGround = GetComponent<Collider2D>().bounds.extents.y;

        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        scale = transform.localScale;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        arms = transform.Find("Arms");
        shootPoint = transform.Find("ShootPoint");
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

        if (IsGrounded())
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

        HandleVisuals(grounded);

        void HandleVisuals(bool grounded)
        {
            if (slingShot.shooting)
            {
                animator.enabled = false;
                spriteRenderer.sprite = attackSprite;
                arms.gameObject.SetActive(true);

                float angle = Mathf.Atan2(slingShot.launchDirection.y, slingShot.launchDirection.x) * Mathf.Rad2Deg;

                // Flip do player
                if (angle > 90f || angle < -90f)
                {
                    scale.x = -Mathf.Abs(scale.x); // virar para esquerda
                }
                else
                {
                    scale.x = Mathf.Abs(scale.x); // virar para direita
                }

                transform.localScale = scale;

                // Corrigir rotação do braço e do ponto de tiro
                arms.localScale = Vector3.one; // garante escala padrão
                shootPoint.localScale = Vector3.one;

                // Se o personagem estiver flipado, ajusta a rotação espelhando verticalmente
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
        Vector2 origin = transform.position;
        float distance = distanceToGround + 0.1f;
        LayerMask mask = LayerMask.GetMask("Default");
        RaycastHit2D raycastHit = Physics2D.Raycast(origin, Vector2.down, distance, mask);
        bool grounded = raycastHit.collider is not null;
        Debug.Log(grounded);
        return grounded;
    }
}
