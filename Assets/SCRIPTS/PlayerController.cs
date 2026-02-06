using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Input Action Maps and Actions
    [SerializeField] InputActionAsset InputAction;
    private InputActionMap _playerActionMap;
    private InputAction _qAction, _attackAction, _moveAction, _sprintAction;  // BUTTON ACTIONS

    NavMeshAgent navMeshAgent;

    // Projectile Prefab
    [SerializeField] GameObject projectile;
    [SerializeField]  Transform projectileOrigin;
    float CooldownTime = 0.5f;
    float lastFireTime = 0f;
    bool canFire = true;

    // Angel Model
    GameObject angelModel;

    // Combat vars
    int MaxHealth = 100;
    int CurrentHealth;
    float MaxEnergy = 100f;
    float CurrentEnergy;
    bool IsDead = false;
    bool IsSprinting = false;

    private void Awake( )
    {
        navMeshAgent = GetComponent<NavMeshAgent>( );
        angelModel = transform.Find("AngelModel").gameObject;
        CurrentHealth = MaxHealth;
        CurrentEnergy = MaxEnergy;
    }

    private void Update( )
    {
        // Attach camera to player with offset
        Camera.main.transform.position = transform.position + new Vector3(0, 4.5f, -2.97f);

        if (_sprintAction.ReadValue<float>( ) > 0)
        {
            IsSprinting = true;
            navMeshAgent.speed = 5f;
        } else
        {
            IsSprinting = false;
            navMeshAgent.speed = 2f;
        }


        //Vector2 mouseScreenPos = Mouse.current.position.ReadValue( );

        //Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        //    MoveAgentToPoint(hit.point);
        //    Debug.Log($"Move to {hit.point}");
        //}

        if (!canFire)
        {
           if (lastFireTime < CooldownTime)
           {
               lastFireTime += Time.deltaTime;
           } else
           {
               canFire = true;
               lastFireTime = 0f;
           }
        }


        // Sprinting drains energy
        if (IsSprinting)
        {
            CurrentEnergy -= 20f * Time.deltaTime;
            if (CurrentEnergy <= 0)
            {
                CurrentEnergy = 0;
                IsSprinting = false;
                navMeshAgent.speed = 2f;
            }
        } else
        {
            // Regenerate energy when not sprinting
            CurrentEnergy += 10f * Time.deltaTime;
            if (CurrentEnergy > MaxEnergy)
                CurrentEnergy = MaxEnergy;
        }
    }

    private void OnEnable( )
    {
        // Setup Input Actions
        _playerActionMap = InputAction.FindActionMap("Player");
        _qAction = _playerActionMap.FindAction("Q");
        _attackAction = _playerActionMap.FindAction("PrimaryAttack");
        _moveAction = _playerActionMap.FindAction("SecondaryAttack");
        _sprintAction = _playerActionMap.FindAction("Sprint");
        _playerActionMap.Enable( );

        // Bind Button Actions
        //_qAction.performed += _qAction_performed;
        _attackAction.performed += _attackAction_performed;
        _moveAction.performed += _moveAction_performed;
    }

    private void _attackAction_performed(InputAction.CallbackContext obj)
    {
        if (!canFire)
            return;

        canFire = false;

        // rotate model to mouse pos
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue( );
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 lookPos = hit.point;
            Vector3 direction = lookPos - angelModel.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                angelModel.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        // instantiate projectile
        Instantiate(projectile, projectileOrigin.position, projectileOrigin.rotation);

        // reset model rotation
        StartCoroutine(ResetRotation( ));
    }

    IEnumerator ResetRotation( )
    {
        yield return new WaitForSeconds(0.4f);
        angelModel.transform.LookAt(transform.position + transform.forward);
    }

    private void _moveAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Move action performed");
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue( );

        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            MoveAgentToPoint(hit.point);
            Debug.Log($"Move to {hit.point}");
        }
    }

    private void MoveAgentToPoint(Vector3 mouseWorldPos)
    {
        navMeshAgent.SetDestination(mouseWorldPos);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Player takes {damage} damage.");
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player has died.");
        }
    }

    private void OnDisable( )
    {
        _playerActionMap.Disable( );
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (IsDead) return;

        GameObject projectile = collision.gameObject;
        if (projectile.CompareTag("Hairball"))
        {
            TakeDamage(20);
            Destroy(projectile);
        }

        //if (_Health <= 0)
        //{
        //    IsDead = true;
        //    _Agent.isStopped = true;
        //    Destroy(gameObject, 1f);
        //}
    }
}
