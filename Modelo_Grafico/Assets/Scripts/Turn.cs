using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]

public class Turn
{
    public Dictionary<string, List<List<string>>> Map = new Dictionary<string, List<List<string>>>();
    public Dictionary<string, List<object>> POI = new Dictionary<string, List<object>>();
    //public Dictionary<string, List<List<int>>> Fire = new Dictionary<string, List<List<int>>>();
    //public Dictionary<string, List<List<int>>> Smokes = new Dictionary<string, List<List<int>>>();
    public Dictionary<string, List<int[]>> Fire = new Dictionary<string, List<int[]>>();
    public Dictionary<string, List<int[]>> Smokes = new Dictionary<string, List<int[]>>();
}