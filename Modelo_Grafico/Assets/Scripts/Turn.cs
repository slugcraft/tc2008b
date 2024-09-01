using System.Collections.Generic;
[System.Serializable]

public class Turn
{
    Dictionary<string, List<string[,]>> Map = new Dictionary<string, List<string[,]>>();
    Dictionary<string, List<POI>> Points = new Dictionary<string, List<POI>>();
    Dictionary<string, List<int[]>> Fire = new Dictionary<string, List<int[]>>();
    Dictionary<string, List<int[]>> Smoke = new Dictionary<string, List<int[]>>();
}