using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTest : MonoBehaviour
{
    //Objeto que se spawnea
    [SerializeField]
    private GameObject temporal = null;

    //Distancia en X
    float x = 0.0f;

    private void Start()
    {
        InvokeRepeating("RandomValue", 0.5f, 0.01f);
    }

    //Spawneo del objeto
    void RandomValue()
    {
        GameObject element = GameObject.Instantiate(temporal);

        //Comprobaciónd de aleatoriedad
        element.transform.position = new Vector3(x, Random.Range(0.0f, 101.0f), 0.0f);

        x += 0.1f;

    }
}
