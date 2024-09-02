using System.Collections.Generic;
[System.Serializable]

public class Turn
{
    public Dictionary<string, List<string[,]>> Map = new Dictionary<string, List<string[,]>>();
    public Dictionary<string, List<POI>> Points = new Dictionary<string, List<POI>>();
    public Dictionary<string, List<int[]>> Fire = new Dictionary<string, List<int[]>>();
    public Dictionary<string, List<int[]>> Smoke = new Dictionary<string, List<int[]>>();
}