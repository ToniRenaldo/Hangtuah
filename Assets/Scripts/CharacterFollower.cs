using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterFollower : MonoBehaviour
{
    NavMeshAgent agent;
    Transform target;
    public float distance;
    public float currentDistance;
    CharacterController controller;
    Animator animator;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<ThirdPersonController>().transform;
        controller= GetComponent<CharacterController>();
        foreach(Transform t in transform)
        {
            if(t.gameObject.activeSelf == true)
            {
                if(t.GetComponent<Animator>() != null)
                animator = t.gameObject.GetComponent<Animator>();
            }
        }
    }

    private void FixedUpdate()
    {
        currentDistance = Vector3.Distance(transform.position, target.position);
        if (Vector3.Distance(transform.position, target.position) > distance) 
        {
            agent.SetDestination(target.position);
            animator.SetFloat("Walking", 1);

        }
        else
        {
            agent.SetDestination(transform.position);
            animator.SetFloat("Walking", 0);

        }


    }
}
