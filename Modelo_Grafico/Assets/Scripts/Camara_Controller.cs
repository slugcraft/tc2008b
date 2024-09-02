using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara_Controller : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0, 13, 0);

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}