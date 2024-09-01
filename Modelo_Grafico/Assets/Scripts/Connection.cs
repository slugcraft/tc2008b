// TC2008B Modelación de Sistemas Multiagentes con gráficas computacionales
// C# client to interact with Python server via POST
// Sergio Ruiz-Loza, Ph.D. March 2021

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    Turn Board;
    AgentTurn Agents;
    // IEnumerator - yield return
    IEnumerator SendData(string data/*, Turn Board, Agent_Turn Agents*/)
    {
        WWWForm form = new WWWForm();
        form.AddField("bundle", "the data");
        string url = "http://localhost:8585";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            //www.SetRequestHeader("Content-Type", "text/html");
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();          // Talk to Python
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log(www.downloadHandler.text);    // Answer from Python
                string[] jsonParts = responseText.Split('\n');

                Agents = JsonUtility.FromJson<AgentTurn>(jsonParts[0].Replace('\'', '\"'));
                //Debug.Log("Agentes " + Agents);

                // Deserialize the second JSON into a custom object or another type
                // Example: Let's assume it's another Vector3 for simplicity
                Board = JsonUtility.FromJson<Turn>(jsonParts[1].Replace('\'', '\"'));
                //Debug.Log("Tablero " + Tablero);
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        //string call = "What's up?";
        Vector3 fakePos = new Vector3(3.44f, 0, -15.707f);
        string json = EditorJsonUtility.ToJson(fakePos);
        //StartCoroutine(SendData(call));
        StartCoroutine(SendData(json));
    }

    // Update is called once per frame
    void Update()
    {

    }
}