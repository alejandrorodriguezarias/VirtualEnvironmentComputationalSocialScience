using UnityEngine;
using System.Collections.Generic;
using TriangleNet.Topology;
using SEGAR;
using Newtonsoft.Json.Linq;
using System;
using TriangleNet.Geometry;

public class EnvironmentGeneratorVitoria : EnvironmentGenerator
{

    /// <summary>
    /// Create the agents using the sociodemographic data. These agents are empty. It is only a demo for the population of the environment.ion
    /// </summary>
    protected override void CreateSimulatedAgents()
    {
        censusDataJson = JsonUtility.FromJson<CensusData>(censusData.text);
        int currentNumberAgents = 0;
        foreach (CensusDataSec censusDataSec in censusDataJson.censusData)
        {
            ICollection<Triangle> triangles = polygons[secRef[censusDataSec.secRef]];
            Triangle[] triangleArray = new Triangle[triangles.Count];
            triangles.CopyTo(triangleArray, 0);
            int maleOver65 = censusDataSec.e_64_h / 10;
            CreateMaleOver65(maleOver65, currentNumberAgents, censusDataSec.secRef, triangleArray);
            int maleUnder65 = censusDataSec.e45_64_h / 10;
            currentNumberAgents = currentNumberAgents + maleOver65;
            CreateMaleUnder65(maleUnder65, currentNumberAgents, censusDataSec.secRef, triangleArray);
            int femaleOver65 = censusDataSec.e_64_m / 10;
            currentNumberAgents = currentNumberAgents + maleUnder65;
            CreateFemaleOver65(femaleOver65, currentNumberAgents, censusDataSec.secRef, triangleArray);
            int femaleUnder65 = censusDataSec.e45_64_m / 10;
            currentNumberAgents = currentNumberAgents + femaleOver65;
            CreateFemaleUnder65(femaleUnder65, currentNumberAgents, censusDataSec.secRef, triangleArray);
            currentNumberAgents = currentNumberAgents + femaleUnder65;
        }
    }

    public void CreateMaleOver65(int numberNewAgents, int currentNumberAgents, string cusec, Triangle[] triangles)
    {

        for (int i = 1; i <= numberNewAgents; i++)
        {
            GameObject agent = Instantiate(citizenPrefab);
            agent.transform.position = GenerateRandomPointWithinTriangle(ChooseRandomTriangle(triangles));
            Citizen citizenAgent = agent.GetComponent<Citizen>();
            agent.name = "Citizen " + (i + currentNumberAgents);
            citizenAgent.gender = 0;
            citizenAgent.age = 65 + UnityEngine.Random.Range(1, 36);
        }
    }

    public void CreateFemaleOver65(int numberNewAgents, int currentNumberAgents, string cusec, Triangle[] triangles)
    {

        for (int i = 1; i <= numberNewAgents; i++)
        {
            GameObject agent = Instantiate(citizenPrefab);
            agent.transform.position = GenerateRandomPointWithinTriangle(ChooseRandomTriangle(triangles));
            Citizen citizenAgent = agent.GetComponent<Citizen>();
            agent.name = "Citizen " + (i + currentNumberAgents);
            citizenAgent.gender = 1;
            citizenAgent.age = 65 + UnityEngine.Random.Range(1, 36);
        }
    }

    public void CreateMaleUnder65(int numberNewAgents, int currentNumberAgents, string cusec, Triangle[] triangles)
    {

        for (int i = 1; i <= numberNewAgents; i++)
        {
            GameObject agent = Instantiate(citizenPrefab);
            agent.transform.position = GenerateRandomPointWithinTriangle(ChooseRandomTriangle(triangles));
            Citizen citizenAgent = agent.GetComponent<Citizen>();
            agent.name = "Citizen " + (i + currentNumberAgents);
            citizenAgent.gender = 0;
            citizenAgent.age = 18 + UnityEngine.Random.Range(0, 47);
        }
    }

    public void CreateFemaleUnder65(int numberNewAgents, int currentNumberAgents, string cusec, Triangle[] triangles)
    {

        for (int i = 1; i <= numberNewAgents; i++)
        {
            GameObject agent = Instantiate(citizenPrefab);
            agent.transform.position = GenerateRandomPointWithinTriangle(ChooseRandomTriangle(triangles));
            Citizen citizenAgent = agent.GetComponent<Citizen>();
            agent.name = "Citizen " + (i + currentNumberAgents);
            citizenAgent.gender = 1;
            citizenAgent.age = 18 + UnityEngine.Random.Range(0, 47);
        }
    }

    protected override void FeedBuildingWithData(JObject buildingData, GameObject building)
    {
        Features features = ((JObject)buildingData["properties"]).ToObject<Features>();
        building.GetComponent<Place>().type = features.building;
    }

    protected override void FeedNodeWithData(JObject buildingData, GameObject building)
    {
        throw new NotImplementedException();
    }
}
