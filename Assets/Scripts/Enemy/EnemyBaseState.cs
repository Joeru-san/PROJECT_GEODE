public class EnemyBaseState
{
    protected Enemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    public EnemyBaseState(Enemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }

    public virtual void EnterState() {}
    public virtual void ExitState() {}
    public virtual void FrameUpdate() {}
    public virtual void PhysicsUpdate() {}
}
