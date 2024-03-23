using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI2 : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    public PlayerController player;
    public Animator animator;
    public float damage = 30;
    public float attackDistance = 1;
    private PlayerHealth _playerHealth;
    public Transform patrolPoint;

    void Start()
    {
        InitComponentLinks();
    }

    // Update is called once per frame
    private void Update()
    {
        _navMeshAgent.destination = player.transform.position;
        if (_playerHealth.value > 1)
        {
            AttackUpdate();
        }
        NoticePlayerUpdate();
    }

    private void NoticePlayerUpdate()
    {
        if (_playerHealth.value <= 0)
        {
            animator.SetTrigger("idle");
        }
    }

    private void InitComponentLinks()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void AttackUpdate()
    {
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            animator.SetTrigger("attack");
        }
    }

    public void AttackDamage()
    {
        if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance) return;

            _playerHealth.DealDamage(damage);
    }
}
