using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ITriggerCheckeable
{
    [field:SerializeField] public float MaxHealth {get; set;}
    public float currentHealth {get; set;}

    public EnemyStateMachine stateMachine {get; set;}
    public EnemyAttackState attackState {get; set;}
    public EnemyIdleState idleState {get; set;}
    public EnemyChaseState chaseState {get; set;}
    public bool isAggroed {get; set;}
    public bool isInStrikingDistance {get; set;}

    void Awake()
    {
        stateMachine = new EnemyStateMachine();

        attackState = new EnemyAttackState(this, stateMachine);
        idleState = new EnemyIdleState(this, stateMachine);
        chaseState = new EnemyChaseState(this, stateMachine);
    }

    void Start()
    {
        currentHealth = MaxHealth;

        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        stateMachine.currentEnemyState.FrameUpdate(); 
    }

    void FixedUpdate()
    {
        stateMachine.currentEnemyState.PhysicsUpdate();
    }

    public void Die()
    {
        Destroy(this);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0f)
        {
            Die();
        }
    }

    public void SetAggroStatus(bool newAggroStatus)
    {
        isAggroed = newAggroStatus;   
    }

    public void SetStrikingDistanceBool(bool newStrikingDistanceBool)
    {
        isInStrikingDistance = newStrikingDistanceBool;
    }
}
