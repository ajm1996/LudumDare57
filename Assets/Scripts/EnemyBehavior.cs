using System;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float direction;
    [SerializeField] private ActivityState activeState = ActivityState.Patrolling;
    [SerializeField] private UnityEngine.Vector3 patrolDestination = UnityEngine.Vector3.zero;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform player;
    [SerializeField] private float sightDistance = 100f;
    [SerializeField] private Transform MikuBeam;

    [SerializeField] private float stepTime = 2f;
    // Time Trackers
    [SerializeField] private float idlePatrolTime = 1f;
    [SerializeField] private float attackChargeTime = 3f;
    [SerializeField] private float attackTime = 1f;

    // Sprites
    [SerializeField] private Sprite Normal;
    [SerializeField] private Sprite Attacking;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        patrolDestination = transform.position;
        //find the player object
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeState == ActivityState.Patrolling)
        {
            // Walk to set destination
            if(Math.Abs(transform.position.x - patrolDestination.x) > 1)
            {
                MoveToPosition();
            }
            // Chillout between destination walk to's
            else if (idlePatrolTime > 0f)
            {
                idlePatrolTime -= Time.deltaTime;
            }
            // Chillout time over, find new spot
            else
            {
                direction = UnityEngine.Random.Range(-100, 100);
                patrolDestination = new UnityEngine.Vector3(transform.position.x + direction, transform.position.y);
                idlePatrolTime = UnityEngine.Random.Range(2, 5);
            }

            RaycastHit2D visionHit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, sightDistance, 1 << 6);
            //Double if statement to avoid null exception errors
            if (visionHit.collider != null)
            {
                if (visionHit.collider.gameObject == player.gameObject)
                {
                    activeState = ActivityState.Chasing;
                }
            }
        }
        else if (activeState == ActivityState.Chasing)
        {
            //Just run it down at the player for now
            patrolDestination = player.transform.position;
            MoveToPosition();

            // Now we need to decide once we're close enough to fire meh laser
            RaycastHit2D attackHit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, sightDistance / 2, 1 << 6);
            //Double if statement to avoid null exception errors
            if (attackHit.collider != null)
            {
                if (attackHit.collider.gameObject == player.gameObject)
                {
                    GetComponent<SpriteRenderer>().sprite = Attacking;
                    activeState = ActivityState.Attacking;
                }
            }

            //Check if we're too far away to bother chasing
            if (UnityEngine.Vector3.Distance(player.transform.position, transform.position) > 200f)
            {
                Debug.Log("Huh, must've been the wind");
                activeState = ActivityState.Patrolling;
            }
        }
        else if (activeState == ActivityState.Attacking)
        {
            if (attackChargeTime > 0)
            {
                attackChargeTime -= Time.deltaTime;
            }
            else
            {
                // Now that the attack has been charge, shoot at where the player was when the charge started
                UnityEngine.Vector3 direction = (patrolDestination - transform.position).normalized;
                UnityEngine.Vector3 laserPoint = direction * 60f;
                Instantiate(MikuBeam, transform.position + laserPoint, UnityEngine.Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
                attackChargeTime = 3f;
                attackTime = 2f;
                activeState = ActivityState.Coolingdown;
                GetComponent<SpriteRenderer>().sprite = Normal;
            }
        }
        else if (activeState == ActivityState.Coolingdown)
        {
            if (attackTime > 0)
            {
                attackTime -= Time.deltaTime;
            }
            else
            {
                activeState = ActivityState.Chasing;
            }
        }
    }

    private enum ActivityState
    {
        Patrolling,
        Chasing,
        Attacking,
        Coolingdown
    }

    void MoveToPosition()
    {
        // Walk to set destination
        
        // Calculate target speed
        float targetSpeed =  Math.Sign(patrolDestination.x - transform.position.x) * 25;
        // Calculate the difference between current and desired velocity
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        // Apply acceleration in the direction we want to move
        float movement = speedDiff * 5f; // 5f is an acceleration factor
        // Apply the force
        rb.AddForce(movement * UnityEngine.Vector2.right);
        // Clamp the velocity to prevent excessive speed
        float maxSpeed = moveSpeed;
        rb.linearVelocity = new UnityEngine.Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed),
            rb.linearVelocity.y
        );
        if (rb.totalForce.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;            
        }

        if (stepTime > 0)
        {
            stepTime -= Time.deltaTime;
        }
        else
        {
            audioSource.Play();
            stepTime = 2f;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (player.transform.position - transform.position).normalized * sightDistance);
    }
}
