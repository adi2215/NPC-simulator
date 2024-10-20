using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentTest : MonoBehaviour
{

    public NavMeshAgent agent;
    public GameObject target;
    private GameObject blocker = null;
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(agent.destination, target.transform.position) > 0.1)
        {
            agent.destination = target.transform.position;
        }
        if (blocker != null && !blocker.activeSelf)
        {
            agent.isStopped = false;
            blocker = null;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("TrafficLightBlocker"))
        {
            blocker = collision.gameObject;
            agent.isStopped = true;
        }
    }
}
