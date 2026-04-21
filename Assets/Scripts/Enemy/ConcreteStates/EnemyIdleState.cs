using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : EnemyBaseState
{
    private float _wanderRadius = 3f;
    private float _wanderTimer = 5f;
    private float _minMoveDistance = 3f;
    private float _maxRoamDistance = 10f;
    private float _timer;
    private Vector3 _roamOrigin;

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log($"{enemy.transform.name} entered in idle state");
        _timer = _wanderTimer;
        _roamOrigin = enemy.transform.position;
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log($"{enemy.transform.name} exit from idle state");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (enemy.isAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
        else
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                Vector3 destination = SampleWanderDestination();
                if (destination != enemy.transform.position)
                {
                    Debug.Log("Wander timer countdown finished, moving to random position");
                    enemy.navMeshAgent.SetDestination(destination);
                }
                _timer = _wanderTimer;
            }
        }
    }

    Vector3 SampleWanderDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 candidate = enemy.transform.position + Random.insideUnitSphere * _wanderRadius;

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, _wanderRadius, NavMesh.AllAreas))
                continue;

            // Reject micro movements
            if (Vector3.Distance(hit.position, enemy.transform.position) < _minMoveDistance)
                continue;

            // Reject positions too far from origin
            if (Vector3.Distance(hit.position, _roamOrigin) > _maxRoamDistance)
                continue;

            return hit.position;
        }

        return enemy.transform.position;
    }
}