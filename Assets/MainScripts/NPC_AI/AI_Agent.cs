using System.Collections;
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
    public float fleeSpeed = 6f;
    public float walkSpeed = 3.5f;
    public float idleTime = 3f;
    public float escapeDistance = 100f;
    public float maxEscapeDistance = 200f; 
    public float sampleRadius = 10f;  
    public float calmDownTime = 40f;

    private NavMeshAgent navAgent;
    private Animator animator; 
    private NPCState currentState;     
    private float idleTimer = 10f; 
    private bool isIdle = false; 
    private GameObject player;
    private bool isEscaping;

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
        //HandleStateLogic();
        UpdateAnimations();
    }

    public void OnPlayerAiming(GameObject p)
    {
        if (currentState != NPCState.Fleeing)
        {
            player = p;
            currentState = NPCState.Fleeing;
            StartCoroutine(CalmDown());
            HandleFleeingState();
        }
    }

    private void HandleStateLogic()
    {
        switch (currentState)
        {
            case NPCState.Normal:
                //HandleNormalState();
                break;

            case NPCState.Fleeing:
                HandleFleeingState();
                break;
        }
    }

    /*private void HandleNormalState()
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
                //animator.SetFloat("speed", 0); 
            }
        }
    }*/

    private void HandleFleeingState()
    {
        navAgent.speed = fleeSpeed;
        MoveNPCToRandomPoint();
    }

    private void ChangeState(NPCState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case NPCState.Normal:
                navAgent.speed = walkSpeed;
                //MoveNPCToRandomPoint();
                break;

            case NPCState.Fleeing:
                navAgent.speed = fleeSpeed; 
                break;
        }
    }

    void MoveNPCToRandomPoint()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 escapeDirection = (transform.position - playerPosition).normalized;

        // Генерация случайной точки в радиусе от игрока
        Vector3 randomPosition = playerPosition + escapeDirection * escapeDistance;

        NavMeshHit hit;
        // Пытаемся найти точку на NavMesh в пределах заданного радиуса
        if (FindValidNavMeshPosition(randomPosition, out hit))
        {
            navAgent.SetDestination(hit.position);  // Устанавливаем точку как цель для НПС
            Debug.Log("NPC moving to: " + hit.position);
        }
    }

    bool FindValidNavMeshPosition(Vector3 targetPosition, out NavMeshHit hit)
    {
        // Мы будем постепенно увеличивать радиус поиска, чтобы не допустить выхода за границы
        float searchRadius = 5f;
        while (searchRadius < escapeDistance)
        {
            if (NavMesh.SamplePosition(targetPosition, out hit, searchRadius, NavMesh.AllAreas))
            {
                // Проверка, что точка не находится слишком близко к краю NavMesh
                Vector3 distanceToEdge = hit.position - targetPosition;
                if (distanceToEdge.magnitude < escapeDistance)
                {
                    return true;  // Если точка внутри допустимого диапазона, возвращаем успешный результат
                }
            }
            searchRadius += 5f;  // Увеличиваем радиус поиска
        }

        hit = new NavMeshHit();  // Если точка не найдена, возвращаем невалидный результат
        return false;
    }

    private void UpdateAnimations()
    {
        switch (currentState)
        {
            case NPCState.Normal:
                //animator.SetBool("isFleeing", false);
                //animator.SetFloat("speed", navAgent.velocity.magnitude / navAgent.speed);
                break;

            case NPCState.Fleeing:
                //animator.SetBool("isFleeing", true);
                //animator.SetFloat("speed", navAgent.velocity.magnitude / navAgent.speed);
                break;
        }
    }

    IEnumerator CalmDown()
    {
        yield return new WaitForSeconds(calmDownTime);
        ChangeState(NPCState.Normal);
        Debug.Log("NPC Feeling " );
    }
}

