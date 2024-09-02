using UnityEngine;
using System.Collections.Generic;

public class AgentController : MonoBehaviour
{
    public AgentTurn agentData;
    public GameObject hounter;
    public float moveSpeed = 2.0f;

    // Clips de animación
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;

    private Animation anim; // Referencia al componente Animation
    private Vector3 targetPosition;

    void Start()
    {
        // Inicializar la animación
        anim = hounter.GetComponent<Animation>();

        // Asignar los clips al componente Animation
        anim.AddClip(idleAnimation, "idle");
        anim.AddClip(walkAnimation, "walk");

        // Reproducir la animación de "idle" al inicio
        anim.Play("idle");

        // Inicializar la posición objetivo
        targetPosition = hounter.transform.position;
    }

    void Update()
    {
        updateAgent();
        ApplyAgentData("(0, 0)");

        // Comprobar si el hounter se está moviendo
        if (Vector3.Distance(hounter.transform.position, targetPosition) > 0.001f)
        {
            if (!anim.IsPlaying("walk"))
            {
                Debug.Log("Transición a la animación 'walk'");
                anim.CrossFade("walk"); // Cambia a la animación de caminar
            }
        }
        else
        {
            hounter.transform.position = targetPosition;
            if (!anim.IsPlaying("idle"))
            {
                Debug.Log("Transición a la animación 'idle'");
                anim.CrossFade("idle");
            }
        }
    }

    public void updateAgent()
    {
        agentData = GameObject.Find("TurnManager").GetComponent<WebClient>().Agents;
    }

    // Método para aplicar la posición desde AgentTurn
    public void ApplyAgentData(string key)
    {
        if (agentData != null)
        {
            List<int> positionList = agentData.Pos[key];
            Debug.Log(positionList);
            if (positionList != null && positionList.Count == 2)
            {
                // Convierte los valores [x, y] en un Vector3
                targetPosition = new Vector3(positionList[1] * 3, 0.1f, 14.5f - positionList[0] * 3);
                hounter.transform.position = Vector3.MoveTowards(hounter.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                Debug.LogWarning("La lista de posiciones no tiene suficientes elementos o es nula.");
            }
        }
        else
        {
            Debug.LogWarning("AgentTurn no está asignado.");
        }
    }
}
