using UnityEngine;
using UnityEngine.AI;

public class AI_Agent : MonoBehaviour
{
    public enum NPCState
    {
        Normal, 
        Fleeing    
    }
    
    [SerializeField] private Transform originalPoint;
    public float fleeDistance = 5f;
    public float fleeSpeed = 6f;
    public float walkSpeed = 3.5f;
    public float idleTime = 3f;

    private NavMeshAgent navAgent;
    private Animator animator; 
    private NPCState currentState;     
    private float idleTimer = 10f; 
    private bool isIdle = false; 

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        navAgent.speed = walkSpeed;
        currentState = NPCState.Normal;
        navAgent.SetDestination(originalPoint.position);
    }

    private void Update()
    {
        HandleStateLogic();
        UpdateAnimations();
    }

    public void OnPlayerAiming(bool isAiming)
    {
        if (isAiming)
        {
            ChangeState(NPCState.Fleeing);
        }
        else if (!isAiming && currentState == NPCState.Fleeing)
        {
            ChangeState(NPCState.Normal);
        }
    }

    private void HandleStateLogic()
    {
        switch (currentState)
        {
            case NPCState.Normal:
                HandleNormalState();
                break;

            case NPCState.Fleeing:
                HandleFleeingState();
                break;
        }
    }

    private void HandleNormalState()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                ChangeState(NPCState.Normal);
            }
        }
        else
        {
            navAgent.SetDestination(originalPoint.position);

            if (Vector3.Distance(transform.position, originalPoint.position) < 1f)
            {
                isIdle = true;
                idleTimer = 10f; 
                navAgent.isStopped = true; 
                animator.SetFloat("speed", 0); 
            }
        }
    }

    private void HandleFleeingState()
    {
        navAgent.speed = fleeSpeed;
        navAgent.SetDestination(ChooseRandomPoint());
    }

    private void ChangeState(NPCState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case NPCState.Normal:
                navAgent.speed = walkSpeed;
                navAgent.isStopped = false; 
                navAgent.SetDestination(ChooseRandomPoint()); 
                isIdle = false; 
                break;

            case NPCState.Fleeing:
                navAgent.speed = fleeSpeed; 
                break;
        }
    }

    private Vector3 ChooseRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 400f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position + randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position; 
        }

        return transform.position; 
    }

    private void UpdateAnimations()
    {
        switch (currentState)
        {
            case NPCState.Normal:
                animator.SetBool("isFleeing", false);
                animator.SetFloat("speed", navAgent.velocity.magnitude / navAgent.speed);
                break;

            case NPCState.Fleeing:
                animator.SetBool("isFleeing", true);
                animator.SetFloat("speed", navAgent.velocity.magnitude / navAgent.speed);
                break;
        }
    }
}

