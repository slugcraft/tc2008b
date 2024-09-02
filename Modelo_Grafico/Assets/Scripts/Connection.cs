// TC2008B Modelaci�n de Sistemas Multiagentes con gr�ficas computacionales
// C# client to interact with Python server via POST
// Sergio Ruiz-Loza, Ph.D. March 2021
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    public Turn Board;
    public AgentTurn Agents;
    public bool TurnAdvance = true;
    // IEnumerator - yield return
    IEnumerator SendData(string data)
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
                //Debug.Log(www.downloadHandler.text);    // Answer from Python
                string[] jsonParts = responseText.Split('\n');
                Debug.Log(jsonParts[0]);
                Debug.Log(jsonParts[1]);

                Agents = JsonConvert.DeserializeObject<AgentTurn>(jsonParts[0]);

                // Deserialize the second JSON into a custom object or another type
                // Example: Let's assume it's another Vector3 for simplicity
                Board = JsonConvert.DeserializeObject<Turn>(jsonParts[1]);
                //Debug.Log(Board.Fire["0"]);
                GameObject.Find("BoardManager").GetComponent<Tablero>().BoardTurn = true;
                TurnAdvance = false;
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TurnAdvance)
        {
            Vector3 fakePos = new Vector3(3.44f, 0, -15.707f);
            string json = EditorJsonUtility.ToJson(fakePos);
            //StartCoroutine(SendData(call));
            StartCoroutine(SendData(json));
        }
    }
}