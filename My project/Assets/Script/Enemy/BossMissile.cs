using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent bossNav;

    void Awake()
    {
        bossNav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        bossNav.SetDestination(target.position);
    }
}
