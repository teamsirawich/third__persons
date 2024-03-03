using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform[] targetPoint;
    public int currentPoint;

    public NavMeshAgent agent;

    public Animator Animator;

    public float waitAtPoint = 1f;
    private float waitCounter;

    public enum AIState
    {
        isDead, isSeekTargetPoint, isSeekPlayer, isAttack
    }
    public AIState state;

    // Start is called before the first frame update
    void Start()
    {
        waitCounter = waitAtPoint;

    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        if (!PlayerController.instance.isDead)
        {

            if (distanceToPlayer >= 1.5f && distanceToPlayer <= 8f)
            {
                state = AIState.isSeekPlayer;
            }
            else if (distanceToPlayer > 8)
            {
                state = AIState.isSeekTargetPoint;
            }
            else
            {
                state = AIState.isAttack;
            }
        }
        else
        {
            state = AIState.isSeekTargetPoint;
            Animator.SetBool("attack", false);
            Animator.SetBool("run", true);
        }


        switch (state)
        {
            case AIState.isDead:

                break;

            case AIState.isSeekPlayer:

                agent.SetDestination(PlayerController.instance.transform.position);
                Animator.SetBool("run", true);
                Animator.SetBool("attack", false);
                break;

            case AIState.isSeekTargetPoint:

                agent.SetDestination(targetPoint[currentPoint].position);
                agent.stoppingDistance = 0f;
               
                if (agent.remainingDistance <= .2f)
                {
                    if (waitCounter > 0)
                    {
                        waitCounter -= Time.deltaTime;
                        Animator.SetBool("run", false);
                    }
                    else
                    {
                        currentPoint++;
                        waitCounter = waitAtPoint;
                        Animator.SetBool("run", true);
                    }


                    if (currentPoint >= targetPoint.Length)
                    {
                        currentPoint = 0;
                    }
                    agent.SetDestination(targetPoint[currentPoint].position);
 
                }

            break;
            
                    case AIState.isAttack:
                    RotateTowardsPlayer();
                    agent.stoppingDistance = 1.5f;
                    Animator.SetBool("attack", true);
                    Animator.SetBool("run", false);
                    break;

        }

    }
    void RotateTowardsPlayer()
    {
        Vector3 direction = (PlayerController.instance.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0 , direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,Time.deltaTime * 5f);
    }
}
