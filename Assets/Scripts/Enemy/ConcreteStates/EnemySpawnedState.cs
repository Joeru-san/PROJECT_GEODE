using UnityEngine;

public class EnemySpawnedState : EnemyBaseState
{
    public EnemySpawnedState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }

    public override void EnterState()
    {
        base.EnterState();

        enemy.aggroTrigger.enabled = false;
        if(enemy.printDebug) Debug.Log($"{enemy.transform.name} entered in spawned state");
    }

    public override void ExitState()
    {
        base.ExitState();

        enemy.aggroTrigger.enabled = true;
        if(enemy.printDebug) Debug.Log($"{enemy.transform.name} exit from spawned state");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        
        if(enemy.currentTarget != null)
        {
            enemy.navMeshAgent.SetDestination(enemy.currentTarget.transform.position);
        }

        if (enemy.isInStrikingDistance)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
    }
}
