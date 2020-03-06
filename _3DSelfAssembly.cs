using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;


public class _3DSelfAssembly : MonoBehaviour
{
    CommonMethods CM = new CommonMethods();
    CellGrid grid = new CellGrid(AreaMin, AreaMax);
    ReconfigurationRules reconfiguration = new ReconfigurationRules();



    // Target Geometry
    public GameObject GEO;
    GameObject GEOInstance;

    // Agent Properties
    public GameObject agent;
    Agents agents;

    // Lists/Collections
    Agent[] listAgents = new Agent[NumAgents]; 
    List<Vector3> geometryCoordinates;

    // Environment
    public static int AreaMin = 0;
    public static int AreaMax = 100;  // DO NOT EXCEED 500x500!
    static int NumAgents = 300;

    // Render Effects
    public Material Material;


    // Application Elements
    public float minSpeed = 0.0f;
    public float maxSpeed = 3.0f;
    public float currentSpeed;



    void Start()
    {
        
        GEOInstance = Instantiate(GEO, new Vector3(20, 0, 20), Quaternion.identity);

        geometryCoordinates = PointsFromGeometry(GEOInstance).Where(pt => !CM.OutsideBoundaries(pt, AreaMin, AreaMax)).ToList();
        //print(geometryCoordinates.Count);
        Destroy(GEOInstance);
        


        // INSTANTIATE AGENTS (with different agent placement methods)
        //NumAgents = geometryCoordinates.Count;
        //NumAgents = grid.Cells.Length;
        agents = new Agents(agent, Material, NumAgents);

        //agents.FillCellsWithAgents(grid.Cells);
        agents.PlaceAgentsIn2DGrid(new Vector3Int(45, 0, 45), grid.Cells);
        //agents.PlaceAgentsInGivenGeometry(geometryCoordinates); 
        //agents.PlaceAgentsRandomly(AreaMin, AreaMax);
        //agents.PlaceConnectedAgentsRandomly(CM.RandomPosition(AreaMin, AreaMax));
        //agents.PlaceConnectedAgentsRandomly(new Vector3Int(50,0,50));
        listAgents = agents.listAgents;









        //var list = new List<int>();
        //var orderedListAgents = listAgents.OrderBy(ag => ag.Location.GetFaceNeighbours().Count(n => n.Alive)).ToArray();
        //foreach (var item in orderedListAgents)
        //{
        //    list.Add(item.Location.GetFaceNeighbours().Count(n => n.Alive));
        //}

        //string array = "";
        //for (int i = 0; i < list.Count; i++)
        //{
        //    array += list[i] + " ";
        //}
        //print(array);

        //Task.Run(reconfiguration); <- Implement?

    }

    // Update is called once per frame
    void Update()
    { 
        
    }


    // AGENT CHOICE
    // RandomShuffle: Fisher-Yates Shuffle algorithm; shuffles a list to randomly organize the its elements
    void RandomShuffle<T>(T[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            int j = i + UnityEngine.Random.Range(0, list.Length - i);

            T temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }


    void SortByNeighbours()
    {
        listAgents.OrderBy(agent => agent.Location.GetFaceNeighbours().Count(n => n.Alive));   // ???
    }


    public Agent AgentSelectionByNeighbours()
    {
        Agent agent;
        while (true)
        {
            agent = listAgents[UnityEngine.Random.Range(0, listAgents.Length - 1)];
            int nNeighbours = agent.Location.GetFaceNeighbours().Count(n => n.Alive);

            float probabilityOfChoice = 1.0f - nNeighbours / 6.0f;  //Six possible face neighbours

            if (UnityEngine.Random.Range(0.0f, 1.0f) < probabilityOfChoice)
                break;
        }
        return agent;
    }



    ////////////////////////   RECONFIGURATION  ////////////////////////

    // Reconfiguration: Reconfiguration process
    public IEnumerator Reconfiguration()
    {
        List<ReconfigurationRules.Rules> rules = reconfiguration.RuleList();

        while (true)
        {
            // Instead of shuffling the list and running through the entire list; I can randomly choose an agent and repeat 
            // (same agent has a change to be chosen twice in a row); 
            //RandomShuffle(listAgents);


            //foreach (Agent agent in listAgents)  // asynchronous calculations for each agent require a delay between sequential evaluation
            //{

            if (GUI.Paused == false)
            {
                //int index = (int) Mathf.Floor((float)((float)listAgents.Length - 1e-3) * Mathf.Pow(UnityEngine.Random.Range(0.0f, 1.0f), 3));
                //int index = (int)(Mathf.Pow(UnityEngine.Random.Range(0.0f, 1.0f), 3) * (listAgents.Length - 1));
                //print(index);

                //var orderedListAgents = listAgents.OrderBy(ag => ag.Location.GetFaceNeighbours().Count(n => n.Alive)).ToArray();  //IS THIS A GOOD IDEA?
                //Agent agent = orderedListAgents[index];
                Agent agent = AgentSelectionByNeighbours();

                RandomShuffle(rules.ToArray());
                reconfiguration.Successful = false;

                foreach (var rule in rules)
                {
                    if (reconfiguration.Successful == false)
                        StartCoroutine(rule(grid.Cells, agent.Location, agent));
                    else
                        break;
                }

                yield return new WaitForSeconds(currentSpeed);
            }
            else
            {
                yield return null;
            }
            //}
        }
    }




    // PointsFromGeometry: Calculates the target points for the Self-Assembly Algorithm from an 3D geometric input
    public IEnumerable<Vector3> PointsFromGeometry(GameObject geometry)
    {
        MeshCollider collider = geometry.GetComponent<MeshCollider>();
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;

        for (float y = min.y; y < max.y; y++)
        {
            for (float z = min.z; z < max.z; z++)
            {
                for (float x = min.x; x < max.x; x++)
                {
                    Vector3 point = new Vector3(x, y, z);

                    if (CM.InsideCollider(point, new Vector3(1000, 1000, 1000)))
                        yield return point;
                }
            }
        }
    }
}
