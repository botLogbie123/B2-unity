using UnityEngine;
using UnityEngine.AI;

namespace GlosColGames
{
    /// <summary>
    /// Abstract base class: all states inherit from this.
    /// Students add new states by creating another class that inherits EnemyState.
    /// </summary>
    public abstract class EnemyState
    {
        public virtual string Name => GetType().Name;

        public virtual void Enter(EnemyAI ai) { }
        public virtual void Tick(EnemyAI ai) { }
        public virtual void Exit(EnemyAI ai) { }

        // Helper: move somewhere safely
        protected void MoveTo(EnemyAI ai, Vector3 pos, float speed, float stoppingDistance)
        {
            ai.agent.isStopped = false;
            ai.agent.speed = speed;
            ai.agent.stoppingDistance = stoppingDistance;
            ai.agent.SetDestination(pos);
        }
    }

    // ------------------------------------------------------------
    //  STATE 1: IDLE (do nothing until player seen, or patrol exists)
    // ------------------------------------------------------------
    public class IdleState : EnemyState
    {
        public override void Enter(EnemyAI ai)
        {
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
        }

        public override void Tick(EnemyAI ai)
        {
            // If patrol points added, start patrolling
            if (ai.patrolPoints != null && ai.patrolPoints.Length > 0)
            {
                ai.SetState(new PatrolState());
                return;
            }

            // If player seen, chase
            if (ai.CanSeeTarget())
            {
                ai.lastKnownTargetPos = ai.target.position;
                ai.SetState(new ChaseState());
            }
        }
    }

    // ------------------------------------------------------------
    //  STATE 2: PATROL (walk between points)
    // ------------------------------------------------------------
    public class PatrolState : EnemyState
    {
        private float waitTimer;

        public override void Enter(EnemyAI ai)
        {
            waitTimer = 0f;

            if (ai.patrolPoints == null || ai.patrolPoints.Length == 0)
            {
                ai.SetState(new IdleState());
                return;
            }

            GoToCurrentPoint(ai);
        }

        public override void Tick(EnemyAI ai)
        {
            if (ai.CanSeeTarget())
            {
                ai.lastKnownTargetPos = ai.target.position;
                ai.SetState(new ChaseState());
                return;
            }

            if (ai.patrolPoints == null || ai.patrolPoints.Length == 0)
            {
                ai.SetState(new IdleState());
                return;
            }

            if (ai.agent.pathPending) return;

            if (ai.agent.remainingDistance <= ai.waypointTolerance)
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= ai.waitAtWaypoint)
                {
                    waitTimer = 0f;
                    ai.patrolIndex = (ai.patrolIndex + 1) % ai.patrolPoints.Length;
                    GoToCurrentPoint(ai);
                }
            }
        }

        private void GoToCurrentPoint(EnemyAI ai)
        {
            var point = ai.patrolPoints[ai.patrolIndex];
            if (point == null) return;

            MoveTo(ai, point.position, ai.patrolSpeed, 0f);
        }
    }

    // ------------------------------------------------------------
    //  STATE 3: CHASE (follow player; if close enough -> Attack)
    // ------------------------------------------------------------
    public class ChaseState : EnemyState
    {
        public override void Enter(EnemyAI ai)
        {
            // no special setup needed
        }

        public override void Tick(EnemyAI ai)
        {
            if (ai.target == null)
            {
                ai.SetState(new PatrolState());
                return;
            }

            // Update last known position if we can see target
            if (ai.CanSeeTarget())
                ai.lastKnownTargetPos = ai.target.position;

            // If in range, attack
            if (ai.DistanceToTarget() <= ai.attackRange)
            {
                ai.SetState(new AttackState());
                return;
            }

            // Otherwise chase last known position (simple!)
            MoveTo(ai, ai.lastKnownTargetPos, ai.chaseSpeed, ai.attackRange * 0.9f);

            // If we reached last known and can't see target, go back to patrol
            if (!ai.CanSeeTarget() && !ai.agent.pathPending && ai.agent.remainingDistance <= 0.8f)
            {
                ai.SetState(new PatrolState());
            }
        }
    }

    // ------------------------------------------------------------
    //  STATE 4: ATTACK (stop, face target, cooldown print)
    // ------------------------------------------------------------
    public class AttackState : EnemyState
    {
        private float nextAttackTime;

        public override void Enter(EnemyAI ai)
        {
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
            nextAttackTime = Time.time; // can attack immediately
        }

        public override void Tick(EnemyAI ai)
        {
            if (ai.target == null)
            {
                ai.SetState(new PatrolState());
                return;
            }

            // If target moved away -> chase again
            if (ai.DistanceToTarget() > ai.attackRange * 1.2f)
            {
                ai.SetState(new ChaseState());
                return;
            }

            // Face the player
            ai.FaceTarget(ai.target.position, 12f);

            // "Attack" on cooldown (for lesson: print/log)
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + 1.0f; // 1 second cooldown

                RaycastHit hit;
                if (Physics.Raycast(ai.transform.position, ai.transform.forward, out hit, ai.attackRange))
                { 
                    if (hit.collider.GetComponent<PlayerHealth>() != null) 
                    {
                        PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
                        health.TakenDamage(5);
                    }
                }

                Debug.Log($"{ai.name} attacks!");

                // Extend for advanced students:
                // - raycast hit
                // - melee trigger collider
                // - reduce PlayerHealth
            }
        }

        public override void Exit(EnemyAI ai)
        {
            ai.agent.isStopped = false;
        }
    }

}