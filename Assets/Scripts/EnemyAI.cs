using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public List<Transform> patrolPoints;
    public PlayerController player;
    public float viewAngle;
    public float damage = 30;
    public float attackDistance = 1;
    public Animator animator;


    private NavMeshAgent _navMeshAgent;
    private bool _isPlayerNoticed;
    private PlayerHealth _playerHealth;


    private void Start()
    {
        InitComponentLinks();
        PickNewPatrolPoint();
    }


    private void Update()
    {
        NoticePlayerUpdate();
        ChaseUpdate();
        PatrolUpdate();
        AttackUpdate();
    }

    private void AttackUpdate()
    {
        if(_isPlayerNoticed)
        {
            if(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                animator.SetTrigger("attack");
            }
        }
    }

    private void ChaseUpdate()
    {
        if (_isPlayerNoticed)
        {
            _navMeshAgent.destination = player.transform.position;
        }
    }

    public void AttackDamage()
    {
        if (!_isPlayerNoticed) return;
        if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance + attackDistance) return;

        _playerHealth.DealDamage(damage);
    }

    private void NoticePlayerUpdate()
    {
        _isPlayerNoticed = false;
        if (_playerHealth.value <= 0) return;

        var direction = player.transform.position - transform.position;

        if (Vector3.Angle(transform.forward, direction) < viewAngle)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, direction, out hit))
            {
                if (hit.collider.gameObject == player.gameObject)
                {
                    _isPlayerNoticed = true;
                }
            }
        }
    }

    private void PickNewPatrolPoint()
    {
        _navMeshAgent.destination = patrolPoints[Random.Range(0, patrolPoints.Count)].position;
    }

    private void PatrolUpdate()
    {
        if(!_isPlayerNoticed)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                PickNewPatrolPoint();
            }
        }
    }

    private void InitComponentLinks()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerHealth = player.GetComponent<PlayerHealth>();
    }
}

