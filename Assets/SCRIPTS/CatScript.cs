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
    private float _roamRange = 7f;

    // Ranged attack fields (for Oreo)
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileOrigin;
    private float _rangedAttackRange = 5f;

    // Animation
    private Animator _animator;

    protected void Awake( )
    {
        _Agent = GetComponent<NavMeshAgent>( );

        // Find the Animator on the direct child (assumes only one child with Animator)
        _animator = GetComponentInChildren<Animator>( );

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _Target = player.transform;
    }

    private void Start( )
    {
        if (CatType.Callie == catType)
        {
            _MoveSpeed = 3f;
            _Health = 10 * 20;
        } else if (CatType.Dudong == catType)
        {
            _MoveSpeed = 1f;
            _Health = 4 * 20;
        } else if (CatType.Oreo == catType)
        {
            _MoveSpeed = 2f;
            _Health = 2 * 20;
            _AttackRange = _rangedAttackRange; // Oreo uses ranged attack range
        }

        _Agent.speed = _MoveSpeed;
        SetNewRoamDestination( );
    }

    private void Update( )
    {
        if (IsDead || _isOnAttackCooldown) return;

        //// Set move animation (speed parameter)
        //if (_animator != null)
        //    _animator.SetFloat("Speed", _Agent.velocity.magnitude);

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
        if (Vector3.Distance(transform.position, _roamDestination) < 1f)
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
        _Agent.isStopped = true;
        if (_Target != null)
            transform.LookAt(_Target);

        // Trigger attack animation
        if (_animator != null)
            _animator.SetTrigger("Attack");

        if (catType == CatType.Oreo)
        {
            // Ranged attack
            Debug.Log("Oreo shoots projectile!");
            ShootProjectile( );
        } else if (catType == CatType.Callie && _Health <= 10)
        {
            _AttackRange = _rangedAttackRange; // Oreo uses ranged attack range
            _attackCooldown = 0.75f; // faster attack speed
            Debug.Log("Callie shoots projectile!");
            ShootProjectile( );
        } else
        {
            // Melee attack
            Debug.Log("Attack Target");
            if (_Player != null)
                _Player.TakeDamage(20);
        }

        // Start cooldown
        StartCoroutine(AttackCooldown( ));
    }

    private void ShootProjectile( )
    {
        if (projectilePrefab != null && projectileOrigin != null)
        {
            // instantiate projectile
            Instantiate(projectilePrefab, projectileOrigin.position, projectileOrigin.rotation);
        }
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
            TakeDamage(20);
            Destroy(projectile);

            // Trigger hit animation
            if (_animator != null)
                _animator.SetTrigger("Hit");
        }

        if (_Health <= 0)
        {
            IsDead = true;
            _Agent.isStopped = true;
            gameObject.GetComponent<Collider>( ).enabled = false;

            // Trigger death animation
            if (_animator != null)
                _animator.SetBool("IsDead", IsDead);

            Destroy(gameObject, 1f);
        }
    }

    private void TakeDamage(int v)
    {
        Debug.Log($"{catType} took {v} damage, {_Health} remaining");
        _Health -= v;

        // Trigger hit animation
        if (_animator != null)
            _animator.SetTrigger("Hit");
    }
}
