using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

namespace GlosColGames
{
    /// <summary>
    /// Attach to an enemy with a NavMeshAgent.
    /// This is the "brain": holds references + runs the current state.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Target")]
        public Transform target; // drag player here, OR tag your player "Player"

        [Header("Senses")]
        public float viewDistance = 12f;
        [Range(0f, 180f)] public float viewAngle = 90f;
        public LayerMask obstructionMask;

        [Header("Ranges")]
        public float attackRange = 2f;

        [Header("Movement")]
        public float patrolSpeed = 2.5f;
        public float chaseSpeed = 4.5f;

        [Header("Patrol")]
        public Transform[] patrolPoints;
        public float waypointTolerance = 0.6f;
        public float waitAtWaypoint = 1.0f;

        // Shared references for states
        [HideInInspector] public NavMeshAgent agent;

        // Blackboard data for states
        [HideInInspector] public Vector3 lastKnownTargetPos;
        [HideInInspector] public int patrolIndex;

        // State machine
        private EnemyState current;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
        }

        private void Start()
        {
            // Start simple: Patrol if points exist, otherwise Idle.
            if (patrolPoints != null && patrolPoints.Length > 0)
                SetState(new PatrolState());
            else
                SetState(new IdleState());
        }

        private void Update()
        {
            current?.Tick(this);
        }

        public void SetState(EnemyState next)
        {
            if (next == null) return;

            current?.Exit(this);
            current = next;
            current.Enter(this);

            // Debug for lessons:
            // Debug.Log($"{name} => {current.Name}");
        }

        // ---------- Helper methods students can read easily ----------

        public bool CanSeeTarget()
        {
            if (target == null) return false;

            Vector3 toTarget = target.position - transform.position;
            float dist = toTarget.magnitude;
            if (dist > viewDistance) return false;

            float angle = Vector3.Angle(transform.forward, toTarget);
            if (angle > viewAngle * 0.5f) return false;

            // Line of sight check (walls block vision)
            Vector3 eye = transform.position + Vector3.up * 1.6f;
            Vector3 aim = target.position + Vector3.up * 1.2f;

            if (Physics.Linecast(eye, aim, obstructionMask))
                return false;

            return true;
        }

        public float DistanceToTarget()
        {
            if (target == null) return Mathf.Infinity;
            return Vector3.Distance(transform.position, target.position);
        }

        public void FaceTarget(Vector3 worldPos, float turnSpeed = 10f)
        {
            Vector3 dir = worldPos - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f) return;

            Quaternion desired = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, Time.deltaTime * turnSpeed);
        }
    }

}