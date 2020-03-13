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
    CellGrid grid;
    ReconfigurationRules reconfiguration;
    CommonMethods CM = new CommonMethods();
    



    // Target Geometry
    public GameObject GEO;
    GameObject GEOInstance;

    // Agent Properties
    public GameObject agent;
    Agents agents;

    // Lists/Collections
    Agent[] listAgents = new Agent[NumAgents]; 
    List<Vector3> geometryCoordinates;
    List<GameObject> alive = new List<GameObject>();

    // Environment
    public static int AreaMin = 0;
    public static int AreaMax = 50;  // DO NOT EXCEED 500x500!    -> ENDGOAL: Eliminate have space dimensions defined by number of Agents?
    static int NumAgents = 150;

    // Render Effects
    public Material Material;
    Color lerpedColor;


    // Application Elements
    public const float minSpeed = 0.0f;
    public const float maxSpeed = 3.0f;
    public float currentSpeed;



    void Start()
    {
        grid = new CellGrid(AreaMin, AreaMax);
        reconfiguration = new ReconfigurationRules(grid, currentSpeed);


        GEOInstance = Instantiate(GEO, new Vector3(10, 0, 10), Quaternion.identity);

        geometryCoordinates = PointsFromGeometry(GEOInstance).ToList();
        //print(geometryCoordinates.Count);
        
        Destroy(GEOInstance);
        


        // INSTANTIATE AGENTS (with different agent placement methods)
        NumAgents = geometryCoordinates.Count;
        //NumAgents = grid.Cells.Length;
        agents = new Agents(grid, agent, Material, NumAgents);

        //agents.FillCellsWithAgents();
        agents.PlaceAgentsIn2DRows(new Vector3Int(0, 0, 0));
        //agents.PlaceAgentsIn2DRows(grid.GetCellLocation(geometryCoordinates[0]));
        //agents.PlaceAgentsIn3DRows(grid.GetCellLocation(geometryCoordinates[0]));
        //agents.PlaceAgentsInGivenGeometry(geometryCoordinates); 
        //agents.PlaceAgentsRandomly(AreaMin, AreaMax);
        //agents.PlaceConnectedAgentsRandomly(CM.RandomPosition(AreaMin, AreaMax));
        //agents.PlaceConnectedAgentsRandomly(new Vector3(10, 0, 10));
        listAgents = agents.listAgents;


        InitializeAssembly(listAgents);
        ChooseSeed(listAgents);
        //PropagateScents(listAgents[600], 10);


        //foreach (var item in listAgents)
        //{
        //    lerpedColor = Color.Lerp(Color.blue, Color.green, item.ScentValue / 10f);
        //    if (item.ScentValue != 0)
        //    {
        //        item.Obj.GetComponent<Renderer>().material.color = lerpedColor;
        //    }
        //}

        //listAgents[600].Obj.GetComponent<Renderer>().material.color = Color.red;
        //AgentSelectionByScent(listAgents);
        
    }


    // Update is called once per frame
    void Update()
    {
        //CheckAliveCells();
        //AgentSelectionByScent(listAgents);
    }




    void CheckAliveCells()
    {
        foreach (var ob in alive)
        {
            Destroy(ob);
        }
        alive.Clear();

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        foreach (var cell in grid.Cells)
        {
            if (cell.Alive == true)
            {
                var clone = Instantiate(obj, cell.Center, Quaternion.identity);
                alive.Add(clone);
            }
        }
    }






    // AGENT CHOICE
    //AgentSelectionByNeighbours: The bigger the number of neighbours, the less probability the agent has to be chosen
    public Agent AgentSelectionByNeighbours(Agent[] agents)
    {
        while (true)
        {
            Agent agent = agents[UnityEngine.Random.Range(0, agents.Length - 1)];
            int nNeighbours = agent.Location.GetFaceNeighbours().Count(n => n.Alive);

            float probabilityOfChoice = 1.0f - nNeighbours / 6.0f;  //Six possible face neighbours
            float thresholdOfAcceptance = 1.0f - Mathf.Pow(UnityEngine.Random.Range(0.0f, 1.0f), nNeighbours);
            //print("Threshold: " + thresholdOfAcceptance + " < Probability: " + probabilityOfChoice + ", Neighbors:" + nNeighbours);

            if (thresholdOfAcceptance < probabilityOfChoice)
                return agent; 
        }
    }

    public Agent AgentSelectionByScent(Agent[] agents)
    {
        int tries = 10000;

        while (tries-- > 0)
        {
            Agent agent = agents[UnityEngine.Random.Range(0, agents.Length - 1)];

            float probabilityOfChoice = (float)agent.ScentValue / agent.ScentMax + (float)1e-3;
            float thresholdOfAcceptance = 1.0f - Mathf.Pow(UnityEngine.Random.Range((float)agent.ScentValue, agent.ScentMax) / agent.ScentMax, 2);

            if (thresholdOfAcceptance < probabilityOfChoice)
                //print("Threshold: " + thresholdOfAcceptance + " < Probability: " + probabilityOfChoice + ", ScentValue:" + agent.ScentValue);
                return agent;
        }
        return null;
    }









    ////////////////////////   SELF-RECONFIGURATION  ////////////////////////

    // RandomReconfiguration: Rando Reconfiguration process to test the rules
    public IEnumerator RandomReconfiguration()
    {
        while (true)
        {
            if (GUI.Paused == false)
            {
                Agent agent = AgentSelectionByNeighbours(listAgents);

                Action nextAction = reconfiguration.CheckRules(agent);

                if (nextAction != Action.NoAction)
                    reconfiguration.ExecuteAction(this, nextAction, agent);

                yield return new WaitForSeconds(currentSpeed);
            }
            else
            {
                yield return null;
            }
        }
    }






    ////////////////////////   SELF-ASSEMBLY  ////////////////////////

    // SelfAssembly: Self-Assembly process
    public IEnumerator SelfAssembly()
    {
        // Initialize the Assembly Process
        InitializeAssembly(listAgents);

        // Choose an Initial Seed and Propagate Scent
        Agent Seed = ChooseSeed(listAgents);
        PropagateScents(Seed, Seed.ScentValue);

        // Start Assembly
        while (!GoalShapeReached())
        {
            if (GUI.Paused == false)
            {
                Agent agent = AgentSelectionByScent(listAgents.Where(a => a.State != AgentState.Final && a.State != AgentState.Seed).ToArray());
                AgentState state = agent.State;

                if (state == AgentState.Active)
                {
                    Action nextAction = reconfiguration.CheckRules(agent);

                    if (nextAction != Action.NoAction)
                    {
                        reconfiguration.ExecuteAction(this, nextAction, agent);







                        // Update Scent
                        var newNeighbours = agent.Location.GetFaceNeighbours();
                        int maxNeighbouringScent = newNeighbours.Select(s => s.agent.ScentValue).Max();

                        agent.ScentValue = maxNeighbouringScent != 0 ? maxNeighbouringScent - 1 : 0;  // Or current Scent +1?


                        // Check Final Position and Update State
                        if (agent.Location.GoalCell == true & newNeighbours.Any(n => n.agent.State == AgentState.Seed))   // Check Horizontal and Vertical neighbours
                            agent.State = AgentState.Final;
                        else
                        {
                            yield return new WaitForSeconds(currentSpeed);
                            continue;
                        }
                            

                        // Check Rules (rule fitness?)
                        // Decide next Action based on maximizing Scent

                        // If moved:
                        ///////Update Scent (only of moving agent)
                        ///////If next to seed and in goal cell -> State.Final
                        ///////else continue
                        ///
                        //If Seed has no more neighboring goal cells:
                        ////////Former seed -> State.Final
                        ////////Choose next Seed
                        ////////Update Scents


                        // Update Seed
                        int unfilledGoalCells = Seed.Location.GetHorizontalFaceNeighbours().Count(n => n.GoalCell == true && n.Alive == false);

                        if (unfilledGoalCells == 0)
                        {
                            Seed.State = AgentState.Final;
                            Seed = ChooseSeed(listAgents);
                            UpdateScents(Seed, Seed.ScentValue);
                        }

                        yield return new WaitForSeconds(currentSpeed);

                    }
                }
                else if (state == AgentState.Sleep)
                {
                    if (CanMove(agent))
                        agent.State = AgentState.Active;
                }
            }
            else
            {
                yield return null;
            }
        }
    }



    ////////////////////////   SELF-ASSEMBLY STEPS  ////////////////////////

    // InitializeAssembly: Initializes the assembly process and agent states
    void InitializeAssembly(Agent[] agents)
    {
        SetGoalShape(geometryCoordinates);   // Initializes Goal Shape/Goal Cells

        foreach (var agent in agents)
        {
            Cell currentCell = agent.Location;
            int nNeighbours = currentCell.GetFaceNeighbours().Count(n => n.Alive);

            if (currentCell.GoalCell == true)
            {
                agent.State = AgentState.Final;
                agent.Obj.GetComponent<Renderer>().material.color = Color.black;
            }

            else if (CanMove(agent))
            {
                agent.State = AgentState.Active;
                agent.Obj.GetComponent<Renderer>().material.color = Color.blue;
            }
            else
                agent.State = AgentState.Sleep;
        }
    }


    bool CanMove(Agent agent)         // IMPROVE THIS
    {
        int nNeighbours = agent.Location.GetFaceNeighbours().Count(n => n.Alive);

        return nNeighbours < 5 ? true : false;
    }


    Agent ChooseSeed(Agent[] agents)
    {
        Agent Seed = null;
        var staticAgents = agents.Where(a => a.State == AgentState.Final);

        foreach (var agent in staticAgents)
        {
            bool NeighboursToFill = agent.Location.GetHorizontalFaceNeighbours().Any(n => n.GoalCell == true && n.Alive == false);

            if (NeighboursToFill)
            {
                Seed = agent;
                Seed.State = AgentState.Seed;
                Seed.ScentValue = Seed.ScentMax;
                //agent.Obj.GetComponent<Renderer>().material.color = Color.green;
                break;
            }
        }

        // Only Executes if all horizontal neighbours are in position
        if (Seed == null)
        {
            foreach (var agent in staticAgents)
            {
                bool NeighboursToFill = agent.Location.GetVerticalFaceNeighbours().Any(n => n.GoalCell == true && n.Alive == false);

                if (NeighboursToFill)
                {
                    Seed = agent;
                    Seed.State = AgentState.Seed;
                    Seed.ScentValue = Seed.ScentMax;
                    //agent.Obj.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                }
            }
        }

        return Seed;
    }


    void PropagateScents(Agent Seed, int maxScent)
    {
        if (maxScent > 0)
        {
            var neighbours = Seed.Location.GetFaceNeighbours().Where(n => n.Alive == true).ToArray();
            var recursiveNeighbours = new List<Cell>();

            foreach(var neighbour in neighbours)
            {
                if (neighbour.agent.ScentValue == 0)
                {
                    neighbour.agent.ScentValue = maxScent - 1;
                    recursiveNeighbours.Add(neighbour);
                }
            }

            Parallel.ForEach(recursiveNeighbours, neighbour => PropagateScents(neighbour.agent, maxScent - 1));
        }
    }

    void UpdateScents(Agent Seed, int maxScent)
    {
        //Reset Scents back to zero
        foreach (var agent in listAgents)
            agent.ScentValue = 0;

        //Propagate again
        PropagateScents(Seed, maxScent);
    }


    // SetGoalShape: Sets Goal Cells according to the given list of Positions
    public void SetGoalShape(IEnumerable<Vector3> listPositions)
    {
        for (int i = 0; i < listPositions.Count(); i++)
        {
            Vector3Int location = grid.GetCellLocation(listPositions.ElementAt(i));

            Cell currentCell = grid.GetCell(location);
            currentCell.GoalCell = true;
        }
    }


    // ClearGoalCells: Sets all goal cells back to false
    public void ClearGoalCells()
    {
        foreach (var cell in grid.Cells)
        {
            cell.GoalCell = false;
        }
    }


    // GoalShapeReached: Verifies if all Agents have reached their final state; aka are located in a GoalCell
    public bool GoalShapeReached()
    {
        for (int i = 0; i < listAgents.Length; i++)
        {
            Cell cell = listAgents[i].Location;
            // OR
            //AgentState state = listAgents[i].State;      // if (state != AgentState.Final)

            if (cell.GoalCell != true)
                return false;
        }
        return true;
    }




    


    
   


    



    /////////////////////////////////////////////////////////////////
    


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

                    if (CM.InsideCollider(point, new Vector3(1000, 1000, 1000))  &&
                        !CM.OutsideBoundaries(point, AreaMin, AreaMax))
                        yield return point;
                }
            }
        }
    }
}
