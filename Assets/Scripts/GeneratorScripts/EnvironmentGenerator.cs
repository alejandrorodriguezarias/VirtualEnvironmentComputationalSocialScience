using System;
using UnityEngine;
using System.Collections.Generic;
using TriangleNet.Topology;
using Newtonsoft.Json;
using TriangleNet.Geometry;

namespace SEGAR {

    public abstract class EnvironmentGenerator : MonoBehaviour
    {
        //text data
        public TextAsset censusData;
        public TextAsset placesData;
        public TextAsset sectionsData;

        //json objects
        public CensusData censusDataJson;

        //geometry
        protected ICollection<Triangle>[] polygons;
        protected Dictionary<String, int> secRef = new Dictionary<string, int>();

        public float xCoordMin;
        public float xCoordMax;
        public float zCoordMin;
        public float zCoordMax;

        //normalize
        public float newXCoordMin;
        public float newXCoordMax;
        public float newZCoordMin;
        public float newZCoordMax;
        public float scaleFactorX;
        public float scaleFactorZ;

        //gameobjects
        public GameObject delimiter;
        public GameObject citizenPrefab;
        public GameObject placePrefab;

        //visualization
        public bool showVertex;

        /// <summary>
        /// It layers the census tracts, the relevant places and finally the citizens.
        /// </summary>
        void Start()
        {
            try
            {
                CreateCitySections();
                //CreateRelevantPlaces();
                CreateSimulatedAgents();

            }
            catch (Exception e)
            {
                Debug.LogError("The file could not be read:");
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Reads the JSON file of the census tracts and generates the array of polygons.
        /// </summary>
        public void CreateCitySections()
        {
            SectionsData sectionsJson = JsonConvert.DeserializeObject<SectionsData>(sectionsData.text);
            polygons = GeneratePolygons(sectionsJson);

        }

        /// <summary>
        /// Generates te city polygons and triangulate then using the Delaunay triangulation
        /// </summary>
        /// <param name="sectionsJson">A censal sections data object</param>
        /// <returns></returns>
        public ICollection<Triangle>[] GeneratePolygons(SectionsData sectionsJson)
        {
            polygons = new ICollection<Triangle>[sectionsJson.sectionsData.Length];
            int j = 0;
            Vertex[] vertices;
            foreach (SectionData section in sectionsJson.sectionsData)
            {
                vertices = new Vertex[section.geometry.coordinates.Length];
                int i = 0;
                foreach (float[] points in section.geometry.coordinates)
                {
                    vertices[i] = new Vertex(Util.NormalizedMinMax(points[0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * scaleFactorX, Util.NormalizedMinMax(points[1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * scaleFactorZ);
                    //Display flag, only for debug. 
                    if (showVertex)
                    {
                        GameObject agent = Instantiate(delimiter);
                        agent.name = "Delimiter " + i;
                        agent.transform.position = new Vector3(Util.NormalizedMinMax(points[0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * scaleFactorX, 0, Util.NormalizedMinMax(points[1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * scaleFactorZ);
                        i++;
                    }
                }
                Polygon polygon = new Polygon();
                polygon.Add(new Contour(vertices));
                var mesh = polygon.Triangulate();
                polygons[j] = mesh.Triangles;
                secRef.Add(section.properties.secRef, j);
                j++;
            }
            return polygons;
        }

        /// <summary>
        /// Read JSON file of relevant locations and generate them in the virtual environment
        /// </summary>
        public void CreateRelevantPlaces()
        {
            int i = 0;
            RelevantPlacesData relevantPlacesJson = JsonConvert.DeserializeObject<RelevantPlacesData>(placesData.text);
            foreach (RelevantPlaceData relevantPlace in relevantPlacesJson.relevantPlacesData)
            {
                CreateRelevantPlace(relevantPlace, i);
                i++;
            }
        }


        /// <summary>
        /// Calculates a point in the Unity virtual environment that lies within the given triangle.
        /// </summary>
        /// <param name="triangle">Triangle object representing a piece of a census section.</param>
        /// <returns>Vector 3 with the coordinates of a point inside the given triangle</returns>
        public Vector3 GenerateRandomPointWithinTriangle(Triangle triangle)
        {
            float r1 = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
            float r2 = UnityEngine.Random.Range(0f, 1f);
            float w1 = 1 - r1;
            float w2 = r1 * (1 - r2);
            float w3 = r2 * r1;

            Vector2 v1 = new Vector2((float)triangle.GetVertex(0).X, (float)triangle.GetVertex(0).Y);
            Vector2 v2 = new Vector2((float)triangle.GetVertex(1).X, (float)triangle.GetVertex(1).Y);
            Vector2 v3 = new Vector2((float)triangle.GetVertex(2).X, (float)triangle.GetVertex(2).Y);
            Vector2 point = w1 * v1 + w2 * v2 + w3 * v3;
            return new Vector3(point.x, 0, point.y);

        }

        public Triangle ChooseRandomTriangle(Triangle[] triangles)
        {
            return triangles[UnityEngine.Random.Range(0, triangles.Length)];
        }

        /// <summary>
        /// This method must create the agents of the model and must be implemented by any model using
        /// this environment.
        /// </summary>
        abstract protected void CreateSimulatedAgents();
        /// <summary>
        /// This method must create the places of the model and must be implemented by any model using
        /// this environment.
        /// </summary>
        /// <param name="relevantPlace"></param>
        abstract protected void CreateRelevantPlace(RelevantPlaceData relevantPlace, int id);

    }

}

