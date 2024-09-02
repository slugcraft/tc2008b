using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tablero : MonoBehaviour
{
    public GameObject vamp;
    public GameObject bats;
    public GameObject interest;
    public int turno = 0;
    public bool BoardTurn = false;
    List<List<int>> fuego = new List<List<int>>();
    List<List<int>> humo = new List<List<int>>();
    List<List<int>> points = new List<List<int>>();
    //List<int[]> fuego = new List<int[]>();
    //List<int[]> humo = new List<int[]>(); 
    Turn Board;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(BoardTurn)
        {
            StartCoroutine(GenerateEvents());
        }
    }

    public void UpdateMap()
    {
        Board = GameObject.Find("TurnManager").GetComponent<WebClient>().Board;
    }

    public void CreateFire(int turn)
    {
        string indice = turn.ToString();
        for (int i = 0; i < Board.Fire[indice].Count; i++)
        {
            List<int> firePosition = new List<int>(Board.Fire[indice][i]);
            if(!fuego.Any(f => AreListsEqual(f, firePosition)))
            {
                fuego.Add(Board.Fire[indice][i]);
                Instantiate(vamp, new Vector3(3.5f + (Board.Fire[indice][i][1] - 1) * 3f, 0.1f, 14.5f - (Board.Fire[indice][i][0] - 1) * 3f), Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }

    public void CreateSmoke(int turn)
    {
        string indice = turn.ToString();
        for (int i = 0; i < Board.Smokes[indice].Count; i++)
        {
            List<int> smokePosition = new List<int>(Board.Smokes[indice][i]);
            if(!humo.Any(h => AreListsEqual(h, smokePosition)))
            {
                humo.Add(Board.Smokes[indice][i]);
                Instantiate(bats, new Vector3(-1.5f + (Board.Smokes[indice][i][1] - 1) * 3f, -8f, 17.5f - (Board.Smokes[indice][i][0] - 1) * 3f), Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }

    /*public void CreatePOI(int turn)
    {
        string indice = turn.ToString();
        for (int i = 0; i < Board.POI[indice].Count; i++)
        {
            List<int> POIPosition = new List<int>(Board.POI[indice][i][0].Coordinates);
            if(!points.Any(f => AreListsEqual(f, POIPosition)))
            {
                points.Add(Board.POI[indice][i][0].Coordinates);
                Instantiate(interest, new Vector3(3.5f + (POIPosition[0] - 1) * 3f, 0.1f, 14.5f - (POIPosition[1] - 1) * 3f), Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }*/

    private bool AreListsEqual(List<int> list1, List<int> list2)
    {
        if (list1.Count != list2.Count) return false;
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i]) return false;
        }
        return true;
    }

    public IEnumerator GenerateEvents()
    {
        UpdateMap();
        CreateSmoke(turno);
        CreateFire(turno);
        turno += 1;
        GameObject.Find("TurnManager").GetComponent<WebClient>().TurnAdvance = true;
        BoardTurn = false;
        yield return null;
    }
}
