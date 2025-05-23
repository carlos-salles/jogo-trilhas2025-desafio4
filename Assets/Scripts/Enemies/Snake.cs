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

    float currentVelocity;
    bool attacked;

    Rigidbody2D rb;
    Timer cooldownTimer;
    Health health;
    GameObject playerInstance;

    enum State { IDLE, CHASE, ATTACK }

    void Awake()
    {
        currentState = State.IDLE;
        currentVelocity = walkSpeed;
        rb = GetComponent<Rigidbody2D>();
        cooldownTimer = GetComponentInChildren<Timer>();
        groundRaycastDistance = GetComponent<BoxCollider2D>().bounds.size.y;
        health = GetComponent<Health>();
    }
    // Start is called before the first frame update
    void Start()
    {
        health?.onDeath.AddListener(() => Destroy(this.gameObject));
        GameObject scoreObject = GameObject.FindWithTag("Score");
        if (scoreObject != null) {
            Score score = scoreObject.GetComponent<Score>();
            health.onDeath.AddListener(() => score.AddPoints(10));
        }
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
        //Debug.Log($"snake -> {IsGrounded()}");
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
            else if (IsGrounded() && (!isHittingGround || (isHittingWall && viewHit.distance < groundRaycastOffsetX)))
            {
                FacingDirection = -FacingDirection;
            }
            else
            {
                currentVelocity = Mathf.SmoothStep(currentVelocity, FacingDirection * walkSpeed, 0.2f);
                rb.velocity = new Vector2(currentVelocity, rb.velocity.y);
            }
        }
        else if (currentState == State.CHASE)
        {
            if (playerDistance > viewRaycastDistance)
            {
                currentState = State.IDLE;
            }
            else if (playerDistance < viewRaycastDistance * 0.75 && cooldownTimer.IsStopped)
            {
                currentState = State.ATTACK;
            }
            else
            {
                float deltaX = playerInstance.transform.position.x - transform.position.x;
                if (Mathf.Abs(deltaX) > 1f)
                FacingDirection = playerInstance.transform.position.x - transform.position.x;
                currentVelocity = Mathf.SmoothStep(currentVelocity, FacingDirection * runSpeed, 0.25f);
                rb.velocity = new Vector2(currentVelocity, rb.velocity.y);
            }
        }
        else if (currentState == State.ATTACK)
        {
            if (attacked && IsGrounded())
            {
                attacked = false;
                currentState = State.CHASE;
            }
            else {
                Vector2 jumpDirection = new Vector2(FacingDirection * 10, 1);
                rb.AddForce(jumpDirection, ForceMode2D.Impulse);
                attacked = true;
                cooldownTimer.StartTimer();
            }
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

    bool IsHittingGround()
    {
        LayerMask mask = LayerMask.GetMask("Default");
        var hit = Physics2D.Raycast(GroundRaycastPosition, Vector2.down, groundRaycastDistance, mask);
        return hit;
    }
    
    bool IsGrounded()
    {
        var hits = new List<ContactPoint2D>();
        var mask = LayerMask.GetMask("Default");
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(mask);

        if (rb.GetContacts(hits) == 0)
        {
            //Debug.Log("No contacts");
            return false;
        }


        return hits.Exists(p => Vector2.Angle(p.normal, Vector2.up) < 10f);
    }
}
