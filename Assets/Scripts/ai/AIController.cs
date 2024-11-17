using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private DecisionNode rootNode;
    private NavMeshAgent agent;

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
                decisionCondition = IsMeleeInRange,
                trueNode = new ActionNode(Attack),
                falseNode = new ActionNode(GoTo)
            },
            falseNode = new ActionNode(Patrol)
        };
    }

    private void Attack()
    {
        agent.isStopped = true;
        Debug.Log("Attack");
    }

    private bool IsMeleeInRange()
    {
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, closestEnemy.position) < 3f;
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
        return Vector3.Distance(transform.position, closestEnemy.position) < 25f;
    }

    private void GoTo()
    {
        Transform closestEnemy = FindClosestEnemy();
        agent.SetDestination(closestEnemy.position);
        agent.isStopped = false;
    }

    private void Patrol()
    {
        if (targetPatrol != null)
        {
            Debug.Log("Patrol: " + Vector3.Distance(transform.position, targetPatrol.Value));
        }
        else
        {
            Debug.Log("Patrol: null");
        }
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
