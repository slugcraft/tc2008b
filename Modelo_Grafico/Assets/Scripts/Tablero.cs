using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero : MonoBehaviour
{
    public GameObject vamp;
    public GameObject bats;
    Turn Board;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMap();
        CreateFire();
        CreateSmoke();
    }

    public void UpdateMap()
    {
        Board = GameObject.Find("TurnManager").GetComponent<WebClient>().Board;
    }

    public void CreateFire()
    {
        for (int i = 0; i < Board.Fire["0"].Count; i++)
        {
            Instantiate(vamp, new Vector3(3.5f + (Board.Fire["0"][i][1] - 1) * 3f, 0.1f, 14.5f - (Board.Fire["0"][i][0] - 1) * 3f), Quaternion.Euler(0f, 0f, 0f));
        }
    }

    public void CreateSmoke()
    {
        for (int i = 0; i < Board.Smokes["1"].Count; i++)
        {
            Instantiate(bats, new Vector3(-1.5f + (Board.Smokes["1"][i][1] - 1) * 3f, -8f, 17.5f - (Board.Smokes["1"][i][0] - 1) * 3f), Quaternion.Euler(0f, 0f, 0f));
        }
    }
}
