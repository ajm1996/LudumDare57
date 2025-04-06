using System;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float direction;
    [SerializeField] private bool isPatrolling= true;
    [SerializeField] private float idlePatrolTime = 1f;
    [SerializeField] private UnityEngine.Vector3 patrolDestination = UnityEngine.Vector3.zero;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform player;
    [SerializeField] private float sightDistance = 100f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        patrolDestination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPatrolling)
        {
            // Walk to set destination
            if(Math.Abs(transform.position.x - patrolDestination.x) > 1)
            {
                // Calculate target speed
                float targetSpeed =  Math.Sign(direction) * 25;

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

            Ray ray = new Ray(transform.position, player.transform.position - transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit, sightDistance))
            {
                Debug.Log(hit.collider.transform.position);
                if (hit.collider.gameObject == player)
                {
                    Debug.Log("Player Detected!");
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (player.transform.position - transform.position).normalized * sightDistance);
    }
}
