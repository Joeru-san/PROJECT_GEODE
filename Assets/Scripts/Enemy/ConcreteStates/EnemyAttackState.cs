using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log($"{enemy.transform.name} entered in attack state");
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log($"{enemy.transform.name} exit from attack state");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (!enemy.isInStrikingDistance)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
