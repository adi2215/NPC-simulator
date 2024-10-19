using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightsManager : MonoBehaviour
{
    public GameObject end1;
    public GameObject end2;
    public float cycle = 5;
    public Color red;
    public Color green;
    int color;
    float timer;
    Renderer objectRenderer;


    void Start()
    {
        color = Random.Range(0, 1);
        //color = 0;
        timer = Random.Range(0, cycle);
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (color == 0)
        {
            objectRenderer.material.color = red;
            end1.SetActive(true);
            end2.SetActive(true);
        } else
        {
            objectRenderer.material.color = green;
            end1.SetActive(false);
            end2.SetActive(false);
        }
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = cycle;
            color ^= 1;
            
        }
    }
}
