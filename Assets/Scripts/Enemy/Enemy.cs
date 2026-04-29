using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamageable, ITriggerCheckeable
{

    public EnemyStateMachine stateMachine {get; set;}
    public EnemyAttackState attackState {get; set;}
    public EnemyIdleState idleState {get; set;}
    public EnemyChaseState chaseState {get; set;}

    [field: Header("IDamageable")]
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public float currentHealth { get; set; }
    [field: SerializeField] public float attackCoolDown { get; set; }
    [field: SerializeField] public float attackDamage { get; set; }

    [Header("Triggers")]
    public GameObject currentTarget;
    public bool isAggroed {get; set;}
    public BoxCollider aggroTrigger;
    
    public bool isInStrikingDistance {get; set;}

    public BoxCollider strikingTrigger;

    [HideInInspector] public NavMeshAgent navMeshAgent;

    public bool printDebug = false;

    public Dictionary<string, Vector3> triggerSizes = new Dictionary<string, Vector3>() 
    {
        {"basicSize", new Vector3(10f, 1f, 10f)},
        {"scoutSize", new Vector3(12f, 1f, 12f)},
        {"chaseSize", new Vector3(14f, 1f, 14f)},
    };

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        attackState = new EnemyAttackState(this, stateMachine);
        idleState = new EnemyIdleState(this, stateMachine);
        chaseState = new EnemyChaseState(this, stateMachine);

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
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
        if(ObjectPooler.inst.poolDictionary.ContainsKey(GetType().Name))
        {
            ObjectPooler.inst.ReAddToPool(GetType().Name, this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }
        Debug.Log($"{name} is destroyed");
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
