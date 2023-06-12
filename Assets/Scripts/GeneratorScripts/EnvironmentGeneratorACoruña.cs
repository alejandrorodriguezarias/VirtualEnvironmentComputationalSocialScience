using System;
using UnityEngine;
using System.Collections.Generic;
using TriangleNet.Topology;
using Newtonsoft.Json;
using TriangleNet.Geometry;

public class EnvironmentGeneratorACoruña : EnvironmentGenerator
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
            int maleOver65 = censusDataSec.t7_3 / 10;
            CreateMaleOver65(maleOver65, currentNumberAgents, censusDataSec.secRef, triangleArray);
            int maleUnder65 = censusDataSec.t7_2 / 10;
            currentNumberAgents = currentNumberAgents + maleOver65;
            CreateMaleUnder65(maleUnder65, currentNumberAgents, censusDataSec.secRef, triangleArray);
            int femaleOver65 = censusDataSec.t7_6 / 10;
            currentNumberAgents = currentNumberAgents + maleUnder65;
            CreateFemaleOver65(femaleOver65, currentNumberAgents, censusDataSec.secRef, triangleArray);
            int femaleUnder65 = censusDataSec.t7_5 / 10;
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

    protected override void CreateRelevantPlace(RelevantPlaceData relevantPlace, int id)
    {
        GameObject agent =  Instantiate(placePrefab);
        agent.name = "relevantPlace " + id;
        Place place = agent.GetComponent<Place>();
        place.type = relevantPlace.placeData.type;
        agent.transform.position = new Vector3(Util.NormalizedMinMax(relevantPlace.geometry.coordinates[0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * 1000, 0, Util.NormalizedMinMax(relevantPlace.geometry.coordinates[1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * 1000);
    }
}
