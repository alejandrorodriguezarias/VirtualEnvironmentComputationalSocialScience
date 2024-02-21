using System;
using UnityEngine;
using System.Collections.Generic;
using TriangleNet.Topology;
using Newtonsoft.Json;
using TriangleNet.Geometry;
using Newtonsoft.Json.Linq;

namespace SEGAR {

    public abstract class EnvironmentGenerator : MonoBehaviour
    {
        //text data
        public TextAsset censusData;
        public TextAsset placesData;
        //public TextAsset wayPlacesData;
        public TextAsset sectionsData;
        public TextAsset roadsData;
        public TextAsset nodesData;

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
        public Material material;
        public Material roadMaterial;
        public Material road2Material;

        /// <summary>
        /// It layers the census tracts, the relevant places and finally the citizens.
        /// </summary>
        void Start()
        {
            try
            {
                CreateCitySections();

                //Uncomment this line to see the polygons of your geographical divisions. 
                //DebugSections();

                CreateNodes();
                CreateBuildings();
                CreateRoads();
                CreateSimulatedAgents();

            }
            catch (Exception e)
            {
                Debug.LogError("The file could not be read:");
                Debug.LogError(e.Message);
            }
        }


        public Triangle ChooseRandomTriangle(Triangle[] triangles)
        {
            return triangles[UnityEngine.Random.Range(0, triangles.Length)];
        }


        /// <summary>
        /// Reads the JSON file of the buildings and generate the building objects
        /// </summary>
        public void CreateBuildings()
        {

            JObject places = JObject.Parse(placesData.text);
            JArray placesArray = (JArray)places["relevantPlacesData"];

            int i = 0;
            foreach (JObject relevantPlace in placesArray)
            {
                CreateBuilding(relevantPlace, i);
                i++;
            }
        }

        /// <summary>
        /// Reads the JSON file of the divisions and generates the array of polygons.
        /// </summary>
        public void CreateCitySections()
        {
            JObject sections = JObject.Parse(sectionsData.text);
            JArray sectionsArray = (JArray)sections["sectionsData"];
            polygons = GeneratePolygons(sectionsArray);
        }

        /// <summary>
        /// Reads the JSON file of the census tracts and generates the node objects
        /// </summary>
        public void CreateNodes()
        {

            JObject places = JObject.Parse(nodesData.text);
            JArray placesArray = (JArray)places["relevantPlacesData"];

            int i = 0;
            foreach (JObject relevantPlace in placesArray)
            {
                CreateNode(relevantPlace, i);
                i++;
            }
        }
        /// <summary>
        ///  Reads the JSON file of the census tracts and generates the road objects
        /// </summary>
        public void CreateRoads()
        {

            JObject roads = JObject.Parse(roadsData.text);
            JArray roadsArray = (JArray)roads["relevantRoadsData"];

            int i = 0;
            foreach (JObject relevantRoad in roadsArray)
            {
                CreateRoad(relevantRoad, i);
                i++;
            }
        }
        /// <summary>
        /// Shows the shape of the irregular polygons in the scene.
        /// </summary>
        public void DebugSections()
        {

            JObject places = JObject.Parse(sectionsData.text);
            JArray placesArray = (JArray)places["sectionsData"];

            int i = 0;
            foreach (JObject relevantPlace in placesArray)
            {
                DebugSection(relevantPlace, i);
                i++;
            }
        }


        /// <summary>
        /// Generates te city polygons and triangulate then using the Delaunay triangulation
        /// </summary>
        /// <param name="sectionsJson">A censal sections data object</param>
        /// <returns></returns>
        public ICollection<Triangle>[] GeneratePolygons(JArray sectionsJson)
        {
            polygons = new ICollection<Triangle>[sectionsJson.Count];
            int j = 0;
            Vertex[] vertices;
            foreach (JObject section in sectionsJson)
            {
                Geometry geometry = ((JObject)section["geometry"]).ToObject<Geometry>();
                vertices = new Vertex[geometry.coordinates.Length];
                int i = 0;
                foreach (float[] points in geometry.coordinates)
                {
                    vertices[i] = new Vertex(Util.NormalizedMinMax(points[0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * scaleFactorX, Util.NormalizedMinMax(points[1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * scaleFactorZ);
                    //Display flag, only for debug. 
                    if (showVertex)
                    {
                        GameObject agent = Instantiate(delimiter);
                        agent.name = "Delimiter " + i;
                        agent.transform.position = new Vector3(Util.NormalizedMinMax(points[0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * scaleFactorX, 0, Util.NormalizedMinMax(points[1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * scaleFactorZ);
                        
                    }
                    i++;
                }
                Polygon polygon = new Polygon();
                polygon.Add(new Contour(vertices));
                var mesh = polygon.Triangulate();
                polygons[j] = mesh.Triangles;
                secRef.Add(((JObject)section["properties"]).GetValue("secRef").ToString(), j);
                j++;
            }
            return polygons;
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

        /// <summary>
        /// For each relevant building, a building object is generated with a mesh that occupies the entire space marked by its coordinates.
        /// </summary>
        /// <param name="relevantPlace">Json object with the relevant information to create a building</param>
        /// <param name="id">internal id of the building. It only serves to name the building</param>
        protected void CreateBuilding(JObject relevantPlace, int id)
        {
            GameObject polygon = new GameObject("Building");
            MeshFilter meshFilter = polygon.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = polygon.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;
            GeometryWayPlaces geometry = ((JObject)relevantPlace["geometry"]).ToObject<GeometryWayPlaces>();
            Polygon poly = new Polygon();
            for (int i = 0; i < geometry.coordinates[0].Length; i++)
            {
                poly.Add(new Vertex(Util.NormalizedMinMax(geometry.coordinates[0][i][0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * 1000, Util.NormalizedMinMax(geometry.coordinates[0][i][1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * 1000));
            }
            TriangleNet.Mesh tMesh = (TriangleNet.Mesh)poly.Triangulate();

            Vector3[] vertices = new Vector3[tMesh.Vertices.Count];
            int[] triangles = new int[tMesh.Triangles.Count * 3];

            int j = 0;
            foreach (Vertex vertex in tMesh.Vertices)
            {
                vertices[j] = new Vector3((float)vertex.X, 0, (float)vertex.Y);
                j++;
            }
            j = 0;
            foreach (Triangle triangle in tMesh.Triangles)
            {
                triangles[j] = triangle.GetVertexID(0);
                triangles[j + 2] = triangle.GetVertexID(1);
                triangles[j + 1] = triangle.GetVertexID(2);
                j = j + 3;
            }

            if (vertices.Length > 2)
            {
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                meshRenderer.material = material;
                polygon.AddComponent<BoxCollider>();
            }
            polygon.AddComponent<Place>();
            FeedBuildingWithData(relevantPlace, polygon);



        }

        /// <summary>
        /// Creates the shape of each irregular polygon and displays it in the scene.
        /// </summary>
        /// <param name="division">A JSON object representing a political division</param>
        /// <param name="id">internal id of the division. It only serves to name the division</param>
        protected void DebugSection(JObject division, int id)
        {
            GameObject polygon = new GameObject("Division" + id);
            MeshFilter meshFilter = polygon.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = polygon.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;
            Geometry geometry = ((JObject)division["geometry"]).ToObject<Geometry>();
            Polygon poly = new Polygon();
            for (int i = 0; i < geometry.coordinates.Length; i++)
            {
                poly.Add(new Vertex(Util.NormalizedMinMax(geometry.coordinates[i][0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * 1000, Util.NormalizedMinMax(geometry.coordinates[i][1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * 1000));
            }
            TriangleNet.Mesh tMesh = (TriangleNet.Mesh)poly.Triangulate();

            Vector3[] vertices = new Vector3[tMesh.Vertices.Count];
            int[] triangles = new int[tMesh.Triangles.Count * 3];

            int j = 0;
            foreach (Vertex vertex in tMesh.Vertices)
            {
                vertices[j] = new Vector3((float)vertex.X, 4, (float)vertex.Y);
                j++;
            }
            j = 0;
            foreach (Triangle triangle in tMesh.Triangles)
            {
                triangles[j] = triangle.GetVertexID(0);
                triangles[j + 2] = triangle.GetVertexID(1);
                triangles[j + 1] = triangle.GetVertexID(2);
                j = j + 3;
            }

            if (vertices.Length > 2)
            {
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                meshRenderer.material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            }
        }


        /// <summary>
        /// Create a road object whose mesh occupies the given coordinates.
        /// </summary>
        /// <param name="road">Json object with the road info</param>
        /// <param name="id">internal id of the road. It only serves to name the road</param>
        protected void CreateRoad(JObject road, int id)
        {

            Tipo type = ((JObject)road["geometry"]).ToObject<Tipo>();
            if (type.type.Equals("Polygon"))
            {
                GameObject polygon = new GameObject("Road");
                MeshFilter meshFilter = polygon.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = polygon.AddComponent<MeshRenderer>();
                Mesh mesh = new Mesh();
                meshFilter.mesh = mesh;

                GeometryWayPlaces geometry = ((JObject)road["geometry"]).ToObject<GeometryWayPlaces>();
                Polygon poly = new Polygon();
                for (int i = 0; i < geometry.coordinates[0].Length; i++)
                {
                    poly.Add(new Vertex(Util.NormalizedMinMax(geometry.coordinates[0][i][0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * 1000, Util.NormalizedMinMax(geometry.coordinates[0][i][1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * 1000));
                }
                TriangleNet.Mesh tMesh = (TriangleNet.Mesh)poly.Triangulate();

                Vector3[] vertices = new Vector3[tMesh.Vertices.Count];
                int[] triangles = new int[tMesh.Triangles.Count * 3];

                int j = 0;
                foreach (Vertex vertex in tMesh.Vertices)
                {
                    vertices[j] = new Vector3((float)vertex.X, 0, (float)vertex.Y);
                    j++;
                }
                j = 0;
                foreach (Triangle triangle in tMesh.Triangles)
                {
                    triangles[j] = triangle.GetVertexID(0);
                    triangles[j + 2] = triangle.GetVertexID(1);
                    triangles[j + 1] = triangle.GetVertexID(2);
                    j = j + 3;
                }

                if (vertices.Length > 2)
                {
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.RecalculateNormals();
                    meshRenderer.material = roadMaterial;
                }
            }
            else if (type.type.Equals("LineString"))
            {
                GeometryRoads geometry = ((JObject)road["geometry"]).ToObject<GeometryRoads>();
                Polygon poly = new Polygon();
                List<Vector3> vertices = new List<Vector3>();
                for (int i = 0; i < geometry.coordinates.Length; i++)
                {
                    //poly.Add(new Vertex(Util.NormalizedMinMax(geometry.coordinates[i][0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * 1000, Util.NormalizedMinMax(geometry.coordinates[i][1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * 1000));
                    Vector3 position = new Vector3(Util.NormalizedMinMax(geometry.coordinates[i][0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * 1000, 0, Util.NormalizedMinMax(geometry.coordinates[i][1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * 1000);
                    vertices.Add(position);
                }

                if (geometry.coordinates.Length >= 3)
                {

                    GameObject roadgo = new GameObject("Road");
                    roadgo.AddComponent<LineRenderer>();
                    LineRenderer lineRenderer = roadgo.GetComponent<LineRenderer>();
                    lineRenderer.positionCount = vertices.Count;
                    lineRenderer.SetPositions(vertices.ToArray());
                    lineRenderer.material = road2Material;
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;

                }
            }
        }

        /// <summary>
        /// Create a node in the given coordinates
        /// </summary>
        /// <param name="road">Json object with the node info</param>
        /// <param name="id">internal id of the node It only serves to name the node</param>
        public void CreateNode(JObject relevantPlace, int id) {
            GeometryNodes geometry = ((JObject)relevantPlace["geometry"]).ToObject<GeometryNodes>();
            GameObject place = Instantiate(placePrefab);
            place.name = "node " + id; 
            place.transform.position = new Vector3(Util.NormalizedMinMax(geometry.coordinates[0], xCoordMin, xCoordMax, newXCoordMin, newXCoordMax) * 1000, 0, Util.NormalizedMinMax(geometry.coordinates[1], zCoordMin, zCoordMax, newZCoordMin, newZCoordMax) * 1000);
            FeedNodeWithData(relevantPlace, place);
        }

        /// <summary>
        /// This method must create the agents of the model and must be implemented by any model using
        /// this environment.
        /// </summary>
        abstract protected void CreateSimulatedAgents();
        abstract protected void FeedBuildingWithData(JObject buildingData, GameObject building);
        abstract protected void FeedNodeWithData(JObject buildingData, GameObject building);

    }

}

