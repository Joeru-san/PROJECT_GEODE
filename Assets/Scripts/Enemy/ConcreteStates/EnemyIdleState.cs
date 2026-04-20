using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log($"{enemy.transform.name} entered in idle state");
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log($"{enemy.transform.name} exit from idle state");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        
        if(enemy.isAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
