using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CensusDataSec
{
    public string secRef;
    public int t7_2;
    public int t7_3;
    public int t7_5;
    public int t7_6;
    public int e16_24_h;
    public int e16_24_m;
    public int e25_44_h;
    public int e25_44_m;
    public int e45_64_h;
    public int e45_64_m;
    public int e_64_h;
    public int e_64_m;

}

[System.Serializable]
public class CensusData
{
    public CensusDataSec[] censusData;
}