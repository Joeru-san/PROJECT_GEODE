using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log($"{enemy.transform.name} entered in chase state");

        enemy.aggroTrigger.size = enemy.triggerSizes["chaseSize"];

        if(enemy.currentTarget != null) enemy.navMeshAgent.SetDestination(enemy.currentTarget.transform.position);
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log($"{enemy.transform.name} exit from chase state");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if(enemy.currentTarget != null) enemy.navMeshAgent.SetDestination(enemy.currentTarget.transform.position);

        if(enemy.isInStrikingDistance)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }else if (!enemy.isAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }
}
