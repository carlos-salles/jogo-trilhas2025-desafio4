using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Snake : MonoBehaviour
{
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float contactDamage;
    [SerializeField]
    float _facingDirection;
    float FacingDirection
    {
        get => _facingDirection;
        set
        {
            _facingDirection = Math.Sign(value);

            Vector3 scale = transform.localScale;
            scale.x = _facingDirection;
            transform.localScale = scale;
        }
    }


    [SerializeField]
    float viewRaycastDistance;
    [SerializeField]
    float groundRaycastOffsetX;
    float groundRaycastDistance;
    Vector3 GroundRaycastPosition
    {
        get => transform.position + FacingVector * groundRaycastOffsetX;
    }

    State currentState;
    Vector3 FacingVector
    {
        get => Vector3.Normalize(Vector3.right * FacingDirection);
    }

    Rigidbody2D rb;
    GameObject playerInstance;

    enum State { IDLE, CHASE, ATTACK }

    void Awake()
    {
        currentState = State.IDLE;
        rb = GetComponent<Rigidbody2D>();
        groundRaycastDistance = GetComponent<BoxCollider2D>().bounds.size.y;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<Health>().takeDamage(contactDamage);
        }
    }

    void FixedUpdate()
    {
        RaycastHit2D viewHit = ViewRaycast();
        bool isHittingGround = IsHittingGround();

        float playerDistance = Mathf.Infinity;
        bool isHittingWall = false;
        bool isHittingPlayer = false;
        if (viewHit)
        {
            GameObject gameObject = viewHit.collider.attachedRigidbody.gameObject;
            if (gameObject.CompareTag("Player"))
            {
                playerInstance = gameObject;
                isHittingPlayer = true;
            }
            isHittingWall = !isHittingPlayer;
        }
        if (playerInstance != null)
        {
            playerDistance = Vector2.Distance(transform.position, playerInstance.transform.position);
        }
        /*
        Debug.Log($"state: {currentState.ToString()}");
        Debug.Log($"ground: {isHittingGround}");
        Debug.Log($"wall: {isHittingWall}");
        Debug.Log($"player: {isHittingPlayer}");
        */

        if (currentState == State.IDLE)
        {
            if (isHittingPlayer)
            {
                currentState = State.CHASE;
            }
            else if (!isHittingGround || (isHittingWall && viewHit.distance < groundRaycastOffsetX))
            {
                FacingDirection = -FacingDirection;
            }
            else
            {
                rb.velocity = FacingVector * walkSpeed;
            }
        }
        else if (currentState == State.CHASE)
        {
            if (playerDistance > viewRaycastDistance)
            {
                currentState = State.IDLE;
            }
            else if (playerDistance < viewRaycastDistance / 2)
            {
                currentState = State.ATTACK;
            }
            else
            {
                FacingDirection = playerInstance.transform.position.x - transform.position.x;
                rb.velocity = FacingVector * runSpeed;
            }
        }
        else if (currentState == State.ATTACK)
        {
            Vector2 jumpDirection = new Vector2(FacingDirection, 1);
            //rb.AddForce(jumpDirection.normalized * 10f, ForceMode2D.Impulse);
            currentState = State.CHASE;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, FacingVector * viewRaycastDistance);
        Gizmos.DrawRay(GroundRaycastPosition, Vector3.down * groundRaycastDistance);
    }

    RaycastHit2D ViewRaycast()
    {
        Debug.DrawRay(transform.position, FacingVector * viewRaycastDistance, Color.red);
        var hit = Physics2D.Raycast(transform.position, FacingVector, viewRaycastDistance);
        return hit;
    }

    bool IsHittingGround() {
        LayerMask mask = LayerMask.GetMask("Default");
        var hit = Physics2D.Raycast(GroundRaycastPosition, Vector2.down, groundRaycastDistance, mask);
        return hit;
    }
}
