using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

enum CatType
{
    Callie, Dudong, Oreo
}

enum CatState
{
    Roaming,
    Chasing,
    Attacking
}

public class CatScript : MonoBehaviour
{
    [SerializeField] private float _AttackRange = 1f;

    private NavMeshAgent _Agent;
    private Transform _Target;
    [SerializeField] PlayerController _Player;
    private int _Health;
    private float _MoveSpeed;
    [SerializeField] CatType catType;
    private bool IsDead = false;

    // Cooldown fields
    private bool _isOnAttackCooldown = false;
    private float _attackCooldown = 1f;

    // Roaming fields
    private CatState _state = CatState.Roaming;
    private Vector3 _roamDestination;
    private float _roamRange = 3f;

    protected void Awake( )
    {
        _Agent = GetComponent<NavMeshAgent>( );

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _Target = player.transform;
    }

    private void Start( )
    {
        if (CatType.Callie == catType)
        {
            _MoveSpeed = 3f;
            _Health = 20;
            _roamRange = 6f;
        } else if (CatType.Dudong == catType)
        {
            _MoveSpeed = 1f;
            _Health = 8;
        } else if (CatType.Oreo == catType)
        {
            _MoveSpeed = 2f;
            _Health = 4;
        }

        _Agent.speed = _MoveSpeed;
        SetNewRoamDestination( );
    }

    private void Update( )
    {
        if (IsDead || _isOnAttackCooldown) return;

        if (Vector3.Distance(transform.position, _Target.position) < _roamRange)
        {
            float distance = Vector3.Distance(transform.position, _Target.position);

            if (distance > _AttackRange)
            {
                _state = CatState.Chasing;
            } else
            {
                _state = CatState.Attacking;
            }
        } else
        {
            _state = CatState.Roaming;
        }

        switch (_state)
        {
            case CatState.Roaming:
                Roam( );
                break;
            case CatState.Chasing:
                ChaseTarget( );
                break;
            case CatState.Attacking:
                AttackTarget( );
                break;
        }
    }

    private void Roam( )
    {
        _Agent.isStopped = false;
        if (Vector3.Distance(transform.position, _roamDestination) < 0.2f)
        {
            SetNewRoamDestination( );
        } else
        {
            _Agent.SetDestination(_roamDestination);
        }
    }

    private void SetNewRoamDestination( )
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _roamRange;
        randomDirection.y = 0;
        Vector3 roamPoint = transform.position + randomDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(roamPoint, out hit, _roamRange, NavMesh.AllAreas))
        {
            _roamDestination = hit.position;
        } else
        {
            _roamDestination = transform.position;
        }
    }

    private void ChaseTarget( )
    {
        _Agent.isStopped = false;
        if (_Target != null)
            _Agent.SetDestination(_Target.position);
    }

    private void AttackTarget( )
    {
        Debug.Log("Attack Target");
        _Agent.isStopped = true;
        if (_Player != null)
            _Player.TakeDamage(1);
        if (_Target != null)
            transform.LookAt(_Target);

        // Start cooldown
        StartCoroutine(AttackCooldown( ));
    }

    private IEnumerator AttackCooldown( )
    {
        _isOnAttackCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _isOnAttackCooldown = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsDead) return;

        GameObject projectile = collision.gameObject;
        if (projectile.CompareTag("Treats"))
        {
            TakeDamage(1);
            Destroy(projectile);
        }

        if (_Health <= 0)
        {
            IsDead = true;
            _Agent.isStopped = true;
            Destroy(gameObject, 1f);
        }
    }

    private void TakeDamage(int v)
    {
        Debug.Log($"{catType} took {v} damage, {_Health} remaining");
        _Health -= v;
    }
}
