
using UnityEngine;

public class EnemyAI3D : MonoBehaviour
{
    public Transform player;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float rotateSpeed = 5f;

    public float scanDistance = 15f;
    public float scanAngle = 180f;

    public float endDistance = 2f;

    private enum State
    {
        Patrol,
        Chase,
        End
    }

    private State currentState = State.Patrol;

    private Vector3 patrolDirection;
    private float patrolTimer;

    void Start()
    {
        patrolDirection = transform.forward;
        patrolDirection.y = 0f;

        if (patrolDirection.sqrMagnitude < 0.01f)
            patrolDirection = Vector3.forward;

        patrolDirection.Normalize();

        patrolTimer = 3f;
    }

    void Update()
    {
        if (player == null)
            return;

        if (currentState == State.Patrol)
        {
            Patrol();
            ScanPlayer();
        }
        else if (currentState == State.Chase)
        {
            ChasePlayer();
        }
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0f)
        {
            patrolDirection =
                Quaternion.Euler(
                    0f,
                    UnityEngine.Random.Range(-120f, 120f),
                    0f
                ) * transform.forward;

            patrolDirection.y = 0f;
            patrolDirection.Normalize();

            patrolTimer = 3f;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(patrolDirection);

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * 50f * Time.deltaTime
            );

        transform.position +=
            transform.forward *
            patrolSpeed *
            Time.deltaTime;
    }

    void ScanPlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance > scanDistance)
            return;

        float angle =
            Vector3.Angle(
                transform.forward,
                direction.normalized
            );

        if (angle > scanAngle / 2f)
            return;

        Vector3 origin =
            transform.position + Vector3.up;

        Vector3 target =
            player.position + Vector3.up;

        if (Physics.Linecast(
            origin,
            target,
            out RaycastHit hit
        ))
        {
            if (hit.transform == player ||
                hit.transform.root == player.root)
            {
                Debug.Log("ĐÃ QUÉT ĐƯỢC PLAYER!");

                currentState = State.Chase;
            }
        }
        else
        {
            currentState = State.Chase;

            Debug.Log("ĐÃ QUÉT ĐƯỢC PLAYER!");
        }
    }

    void ChasePlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance <= endDistance)
        {
            currentState = State.End;

            Debug.Log("END GAME!");

            return;
        }

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * 100f *
                    Time.deltaTime
                );
        }

        transform.position +=
            transform.forward *
            chaseSpeed *
            Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Vector3 origin =
            transform.position + Vector3.up;

        Gizmos.color = Color.yellow;

        int segments = 36;

        Vector3 previousPoint =
            origin +
            Quaternion.Euler(
                0f,
                -90f,
                0f
            ) *
            transform.forward *
            scanDistance;

        for (int i = 1; i <= segments; i++)
        {
            float angle =
                -90f +
                (180f / segments) * i;

            Vector3 direction =
                Quaternion.Euler(
                    0f,
                    angle,
                    0f
                ) *
                transform.forward;

            Vector3 point =
                origin +
                direction *
                scanDistance;

            Gizmos.DrawLine(
                previousPoint,
                point
            );

            previousPoint = point;
        }

        Vector3 left =
            Quaternion.Euler(
                0f,
                -90f,
                0f
            ) *
            transform.forward;

        Vector3 right =
            Quaternion.Euler(
                0f,
                90f,
                0f
            ) *
            transform.forward;

        Gizmos.DrawLine(
            origin,
            origin + left * scanDistance
        );

        Gizmos.DrawLine(
            origin,
            origin + right * scanDistance
        );

        if (player != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(
                origin,
                player.position
            );
        }
    }
}
