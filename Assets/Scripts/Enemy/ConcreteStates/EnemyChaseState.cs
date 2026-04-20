using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log($"{enemy.transform.name} entered in chase state");
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log($"{enemy.transform.name} exit from chase state");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if(enemy.isInStrikingDistance)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }else if (!enemy.isAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }
}
