using System.Collections.Generic;

[System.Serializable]
public class SectionData
{
    public string type;
    public Properties properties;
    public Geometry geometry;

}

[System.Serializable]
public class Properties
{
    public int OBJECTID;
    public string secRef;
    public string CUMUN;
    public string CSEC;
    public string CDIS;
    public string CMUN;
    public string CPRO;
    public string CCA;
    public string CUDIS;
    public float Shape_Leng;
    public float Shape_area;
    public float Shape_len;


}

[System.Serializable]
public class Geometry
{
    public string type;
    public float[][] coordinates;


}
[System.Serializable]
public class SectionsData
{
    public SectionData[] sectionsData;
}