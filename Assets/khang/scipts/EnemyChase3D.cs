using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI3D : MonoBehaviour
{
    public Transform player;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    // Bán kính chọn điểm tuần tra ngẫu nhiên quanh vị trí hiện tại
    public float patrolRadius = 10f;

    // Vùng quét bằng mắt - giữ nguyên logic
    // scanAngle = 360 nghĩa là quét toàn bộ quanh enemy (không cần quay mặt về hướng player).
    public float scanDistance = 15f;
    public float scanAngle = 360f;

    private enum State
    {
        Patrol,
        Chase
    }

    private State currentState = State.Patrol;

    private NavMeshAgent agent;

    // Script riêng phụ trách nghe tiếng bước chân - có thể không gắn (để null) nếu chưa cần tính năng này
    private EnemyHearing hearing;

    private float patrolTimer;
    private const float patrolInterval = 3f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        hearing = GetComponent<EnemyHearing>();

        SetNewPatrolDestination();
        patrolTimer = patrolInterval;
    }

    void Update()
    {
        if (player == null)
            return;

        bool seenPlayer = CheckScan();
        bool heardPlayer = hearing != null && hearing.HeardPlayer;

        bool playerDetected = seenPlayer || heardPlayer;

        Debug.Log($"[AI] state={currentState} seen={seenPlayer} heard={heardPlayer} hearingRefNull={hearing == null} agentOnNavMesh={agent.isOnNavMesh}");

        if (currentState == State.Patrol)
        {
            if (playerDetected)
            {
                Debug.Log(seenPlayer
                    ? "ĐÃ QUÉT ĐƯỢC PLAYER!"
                    : "NGHE THẤY TIẾNG BƯỚC CHÂN!");

                currentState = State.Chase;
                agent.speed = chaseSpeed;

                if (hearing != null)
                    hearing.ResetMeter();
            }
            else
            {
                Patrol();
            }
        }
        else if (currentState == State.Chase)
        {
            if (playerDetected)
            {
                // NavMeshAgent tự tính đường ngắn nhất tới player mỗi frame
                ChasePlayer();
            }
            else
            {
                Debug.Log("MẤT DẤU PLAYER - QUAY VỀ TUẦN TRA");

                currentState = State.Patrol;
                agent.speed = patrolSpeed;

                SetNewPatrolDestination();
                patrolTimer = patrolInterval;
            }
        }
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        bool reachedDestination =
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance;

        if (patrolTimer <= 0f || reachedDestination)
        {
            SetNewPatrolDestination();
            patrolTimer = patrolInterval;
        }
    }

    void SetNewPatrolDestination()
    {
        Vector3 randomOffset =
            UnityEngine.Random.insideUnitSphere * patrolRadius;

        randomOffset.y = 0f;

        Vector3 candidatePosition =
            transform.position + randomOffset;

        if (NavMesh.SamplePosition(
                candidatePosition,
                out NavMeshHit hit,
                patrolRadius,
                NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void ChasePlayer()
    {
        // agent.SetDestination dùng thuật toán A* trên NavMesh
        // để tự tính quãng đường ngắn nhất tới player, tránh vật cản
        agent.SetDestination(player.position);
    }

    // Giữ nguyên logic quét bằng mắt (khoảng cách + góc + linecast) như bản gốc,
    // chỉ đổi từ việc tự set state sang trả về bool.
    bool CheckScan()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance > scanDistance)
            return false;

        float angle =
            Vector3.Angle(
                transform.forward,
                direction.normalized
            );

        if (angle > scanAngle / 2f)
            return false;

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
                return true;
            }

            return false;
        }
        else
        {
            return true;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 origin =
            transform.position + Vector3.up;

        Gizmos.color = Color.yellow;

        int segments = 36;

        // Giới hạn 360 để tránh vẽ chồng lặp khi scanAngle > 360
        float clampedAngle = Mathf.Min(scanAngle, 360f);

        float halfAngle = clampedAngle / 2f;

        Vector3 previousPoint =
            origin +
            Quaternion.Euler(
                0f,
                -halfAngle,
                0f
            ) *
            transform.forward *
            scanDistance;

        for (int i = 1; i <= segments; i++)
        {
            float angle =
                -halfAngle +
                (clampedAngle / segments) * i;

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

        // Nếu là 360° (vòng tròn kín) thì không cần vẽ 2 đường viền trái/phải
        if (clampedAngle < 360f)
        {
            Vector3 left =
                Quaternion.Euler(
                    0f,
                    -halfAngle,
                    0f
                ) *
                transform.forward;

            Vector3 right =
                Quaternion.Euler(
                    0f,
                    halfAngle,
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
        }

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