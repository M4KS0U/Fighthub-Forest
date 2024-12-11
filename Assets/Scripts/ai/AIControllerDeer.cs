using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.AI;

public class AIControllerDeer : MonoBehaviour
{
    private DecisionNode rootNode;
    private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private GameObject allyPlayer;

    private Vector3? targetPatrol = null;


    void Start()
    {
        if (agent == null)
        {
            if (!TryGetComponent<NavMeshAgent>(out agent))
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                Debug.LogWarning("NavMeshAgent component added to " + gameObject.name + " because it was missing.");
            }
        }
        rootNode = new Decision
        {
            decisionCondition = IsEnemyInRange,
            trueNode = new Decision
            {
                decisionCondition = haveAally,
                trueNode = new ActionNode(Attack),
                falseNode = new ActionNode(makeAlly)
            },
            falseNode = new ActionNode(followAllyOrPatrol)
        };
    }

    private void followAllyOrPatrol()
    {
        if (allyPlayer != null)
        {
            agent.SetDestination(allyPlayer.transform.position);
            agent.isStopped = false;
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
        else
        {
            Patrol();
        }
    }

    private void makeAlly()
    {
        AIDetect[] detects = FindObjectsOfType<AIDetect>();
        GameObject closestAlly = null;
        foreach (AIDetect detect in detects)
        {
            if (closestAlly == null)
            {
                closestAlly = detect.gameObject;
            }
            else
            {
                if (Vector3.Distance(transform.position, detect.transform.position) < Vector3.Distance(transform.position, closestAlly.transform.position))
                {
                    closestAlly = detect.gameObject;
                }
            }
        }
        allyPlayer = closestAlly;

    }

    private bool haveAally()
    {
        if (allyPlayer == null)
        {
            return false;
        }
        return true;
    }

    private void Attack()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);
    }

    private bool IsMeleeInRange()
    {
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, closestEnemy.position) < 1f;
    }

    bool IsHealthLow()
    {
        return false;
    }

    void Retreat()
    {
        Debug.Log("Retreat");
    }

    void Update()
    {
        rootNode.Evaluate();
    }

    private bool IsEnemyInRange()
    {
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, closestEnemy.position) < 7f;
    }

    private void GoTo()
    {
        Transform closestEnemy = FindClosestEnemy();
        agent.SetDestination(closestEnemy.position);
        agent.isStopped = false;
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);

    }

    private void Patrol()
    {
        animator.SetBool("isWalking", false);
        if (targetPatrol == null || Vector3.Distance(transform.position, targetPatrol.Value) < 2f)
        {
            Vector3 randomPosition = new(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f));
            randomPosition += transform.position;
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                randomPosition = hit.position;
                targetPatrol = randomPosition;
                agent.SetDestination(randomPosition);
                agent.SetPath(agent.path);
                agent.isStopped = false;
            }
            else
            {
                Patrol();
            }
        }
    }

    private Transform FindClosestEnemy()
    {
        AIDetect[] detects = FindObjectsOfType<AIDetect>();
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (AIDetect detect in detects)
        {
            if (detect.IsType(AIDetect.DETECT_TYPE.PAWN))
            {
                if (detect.gameObject == allyPlayer)
                {
                    continue;
                }
                float distance = Vector3.Distance(transform.position, detect.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = detect.transform;
                }
            }
        }
        return closestEnemy;
    }
}
