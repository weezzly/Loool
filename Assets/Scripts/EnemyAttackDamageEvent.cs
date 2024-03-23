using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDamageEvent : MonoBehaviour
{
    public EnemyAI2 enemyAI2;
    public void AttackDamageEvent()
    {
        enemyAI2.AttackDamage();
    }
}
