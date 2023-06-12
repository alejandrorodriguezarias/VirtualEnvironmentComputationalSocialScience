using System.Collections.Generic;

[System.Serializable]
public class RelevantPlaceData
{
    public string type;
    public PlaceData placeData;
    public GeometryPlaces geometry;

}

[System.Serializable]
public class PlaceData
{
    public string type;
}

[System.Serializable]
public class GeometryPlaces
{
    public string type;
    public float[] coordinates;


}
[System.Serializable]
public class RelevantPlacesData
{
    public RelevantPlaceData[] relevantPlacesData;
}