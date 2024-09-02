using UnityEngine;
using System.Collections.Generic;

public class AgentController : MonoBehaviour
{
    public AgentTurn agentData;
    public GameObject hounter;
    public float moveSpeed = 2.0f;

    // Clips de animacion
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;

    private Animation anim; // Referencia al componente Animation
    private Vector3 targetPosition;

    void Start()
    {
        // Inicializar la animaci�n
        anim = hounter.GetComponent<Animation>();

        // Asignar los clips al componente Animation
        anim.AddClip(idleAnimation, "idle");
        anim.AddClip(walkAnimation, "walk");

        // Reproducir la animaci�n de "idle" al inicio
        anim.Play("idle");

        // Inicializar la posici�n objetivo
        targetPosition = hounter.transform.position;
    }

    void Update()
    {
        updateAgent();
        ApplyAgentData("(0, 0)");

        // Comprobar si el hounter se est� moviendo
        if (Vector3.Distance(hounter.transform.position, targetPosition) > 0.001f)
        {
            if (!anim.IsPlaying("walk"))
            {
                Debug.Log("Transicion a la animacion 'walk'");
                anim.CrossFade("walk"); // Cambia a la animaci�n de caminar
            }
        }
        else
        {
            hounter.transform.position = targetPosition;
            if (!anim.IsPlaying("idle"))
            {
                Debug.Log("Transicion a la animacion 'idle'");
                anim.CrossFade("idle");
            }
        }
    }

    public void updateAgent()
    {
        agentData = GameObject.Find("TurnManager").GetComponent<WebClient>().Agents;
    }

    // M�todo para aplicar la posici�n desde AgentTurn
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
            Debug.LogWarning("AgentTurn no esta asignado.");
        }
    }
}
