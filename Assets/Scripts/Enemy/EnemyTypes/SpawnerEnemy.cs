using UnityEngine;

public class SpawnerEnemy : Enemy
{
    public EnemySpawnedState spawnedState {get; set;}

    protected override void Awake()
    {
        base.Awake();        

        spawnedState = new EnemySpawnedState(this, stateMachine);
    }

    void OnEnable()
    {
        currentHealth = MaxHealth;
        stateMachine.Initialize(spawnedState);
    }
}
