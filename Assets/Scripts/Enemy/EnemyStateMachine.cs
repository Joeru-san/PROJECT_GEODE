public class EnemyStateMachine
{
    public EnemyBaseState currentEnemyState {get; set;}

    public void Initialize(EnemyBaseState startingState)
    {
        currentEnemyState = startingState;
        currentEnemyState.EnterState();
    }

    public void ChangeState(EnemyBaseState newState)
    {
        currentEnemyState.ExitState();
        currentEnemyState = newState;
        currentEnemyState.EnterState();
    }
}
