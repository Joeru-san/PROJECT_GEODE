using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }

    bool _isAttacking = false;

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
        if (!_isAttacking)
        {
            enemy.StartCoroutine(AttackCoroutine());
        }

        if (!enemy.isInStrikingDistance)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        yield return new WaitForSeconds(enemy.attackCoolDown);

        if(enemy.currentTarget != null)
        {
            IDamageable damageable = enemy.currentTarget.GetComponentInParent<IDamageable>();
            if(damageable != null)
            {
                Debug.Log($"[EnemyAttackState] Enemy {enemy.name} hit {damageable} for {enemy.attackDamage} damage");
                damageable.TakeDamage(enemy.attackDamage);
            }else
            {
                Debug.Log($"[EnemyAttackState] Enemy {enemy.name} didn't get IDamageable component");
            }
        }

        _isAttacking = false;
    }
}
