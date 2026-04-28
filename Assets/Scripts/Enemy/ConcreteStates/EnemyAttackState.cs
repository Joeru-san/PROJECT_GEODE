using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }

    public override void EnterState()
    {
        base.EnterState();

        enemy.aggroTrigger.enabled = false;
        Debug.Log($"{enemy.transform.name} entered in attack state");
    }

    public override void ExitState()
    {
        base.ExitState();

        enemy.aggroTrigger.enabled = true;
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
