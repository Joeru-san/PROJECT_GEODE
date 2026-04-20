using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    EnemyBaseState currentState;
    public EnemyWalkingState WalkingState = new EnemyWalkingState();
    public EnemyAttackingState AttackingState = new EnemyAttackingState();

    void Start()
    {
        currentState = WalkingState;
        WalkingState.EnterState(this);
    }

    void Update()
    {
        
    }

    public void SwitchState(EnemyBaseState newState)
    {
        currentState = newState;
        newState.EnterState(this);
    }
}
