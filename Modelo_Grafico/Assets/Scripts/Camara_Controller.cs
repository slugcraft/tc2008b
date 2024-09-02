using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara_Controller : MonoBehaviour
{
    public float speed = 5.0f;
    public float turnSpeed = 0.0f;
    public float horizontalInput;
    public float forwardInput;

    // Update is called once per frame
    void Update()
    {
        //Mover vehículo hacia adelante
        transform.Translate(0,0,1);
        //transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
