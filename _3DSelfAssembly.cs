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
    MainCamera Camera;
    ReconfigurationRules reconfiguration;
    CommonMethods CM = new CommonMethods();
    



    // Target Geometry
    public GameObject GEO;
    GameObject GEOInstance;

    // Agent Properties
    public GameObject agentPrefab;
    Agents agents;

    // Lists/Collections
    Agent[] listAgents; 
    List<Vector3> geometryCoordinates;
    List<GameObject> alive = new List<GameObject>();  // test list: delete


    // Environment
    public static int AreaMin;
    public static int AreaMax;      
    public static int NumAgents;
    public const int defaultNumAgents = 100;

    // Render Effects
    public Material Material;
    Color lerpedColor;


    // App Elements
    public const float maxSpeed = 0.0f;    // Speed: Time in seconds between actions 
    public const float minSpeed = 1.5f;
    public const float defaultSpeed = 0.2f;
    public float currentSpeed;



    void Start()
    {
        // SET AREA PROPERTIES
        if (GUI.Reset != true)
            NumAgents = defaultNumAgents;   // Set Number of Agents to default Number: 100

        AreaMin = 0;
        if (NumAgents <= 50)
            AreaMax = 10;
        else if (NumAgents > 1000)
            AreaMax = 200;          // DO NOT EXCEED 500x500! 
        else
            AreaMax = NumAgents / 5; //(int) (NumAgents / (5 + Mathf.Pow(1.10f, NumAgents / 50.0f) - 1));                                     




        // SET CAMERA PROPERTIES
        Camera = FindObjectOfType(typeof(MainCamera)) as MainCamera;
        Camera.SetCameraPosition(AreaMin, AreaMax);






        // INSTANTIATE EMPTY GRID OF CELLS
        grid = new CellGrid(AreaMin, AreaMax);


        // INITIALIZE GEOMETRY COORDINATES
        GEOInstance = Instantiate(GEO, new Vector3(AreaMax / 2f, 0, AreaMax / 2f), Quaternion.identity);
        geometryCoordinates = PointsFromGeometry(GEOInstance).ToList();
        print(geometryCoordinates.Count);
        Destroy(GEOInstance);




        // INSTANTIATE AGENTS
        agents = new Agents(grid, agentPrefab, Material, NumAgents);


        // INSTANTIATE LIST OF AGENTS (with different agent placement methods)
        //listAgents = agents.FillCellsWithAgents();  //Not in use
        //listAgents = agents.PlaceAgentsRandomly();  //Not in use

        if ((int)Mathf.Ceil(Mathf.Sqrt(NumAgents)) < grid.AreaSize)
            listAgents = agents.PlaceAgentsIn2DRows(new Vector3Int(0, 0, 0));
        else
            listAgents = agents.PlaceAgentsIn3DRows(grid.GetCellLocation(geometryCoordinates[0]));   //Never actually needed xD
        //listAgents = agents.PlaceAgentsInGivenGeometry(geometryCoordinates); 
        //listAgents = agents.PlaceConnectedAgentsRandomly(CM.RandomPosition(0, grid.AreaSize - 1));
        //listAgents = agents.PlaceConnectedAgentsRandomly(new Vector3Int(5, 0, 5));



        // INSTANTIATE RECONFIGURATION RULES
        reconfiguration = new ReconfigurationRules(currentSpeed);




        //SetGoalShape(geometryCoordinates);
        //CheckCells();


        //InitializeAssembly(listAgents);
        //ChooseSeed(listAgents);
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

        //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //obj.transform.localScale = new Vector3(0.99f, 0.99f, 0.99f);
        //foreach (var item in geometryCoordinates)
        //{
        //    Instantiate(obj, item, Quaternion.identity);
        //}
        //Destroy(obj);
    }


    // Update is called once per frame
    void Update()
    {
        //CheckCells();

        //foreach (var item in listAgents)
        //{
        //    lerpedColor = Color.Lerp(Color.blue, Color.green, item.ScentValue / 10f);
        //    if (item.ScentValue != 0)
        //    {
        //        item.Obj.GetComponent<Renderer>().material.color = lerpedColor;
        //    }
        //    if (item.ScentValue == 10)
        //    {
        //        item.Obj.GetComponent<Renderer>().material.color = Color.yellow;
        //    }
        //    if (item.ScentValue > 10)
        //    {
        //        item.Obj.GetComponent<Renderer>().material.color = Color.red;
        //    }
        //}
    }




    void CheckCells()
    {
        foreach (var ob in alive)
        {
            Destroy(ob);
        }
        alive.Clear();

        //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.99f, 0.99f, 0.99f);

        foreach (var cell in grid.Cells)
        {
            if (cell.GoalCell == true)//cell.agent.State == AgentState.Seed) //(cell.agent?.State == AgentState.Seed) //(cell.agent != null)
            {
                var clone = Instantiate(obj, cell.Center + new Vector3(0, 0, 0), Quaternion.identity); //cell.CellSize / 2f
                alive.Add(clone);
            }
        }
        Destroy(obj);
    }






    // AGENT CHOICE
    //AgentSelectionByNeighbours: The bigger the number of neighbours, the less probability the agent has to be chosen
    public Agent AgentSelectionByNeighbours(Agent[] agents)
    {
        while (true)
        {
            Agent agent = agents[UnityEngine.Random.Range(0, agents.Length)];
            var neighbours = agent.Location.GetFaceNeighbours().Where(n => n.Alive);
            int nNeighbours = neighbours.Count();

            float probabilityOfChoice = 1.0f - nNeighbours / 6.0f + (float)1e-3;
            float thresholdOfAcceptance = 1.0f - Mathf.Pow(UnityEngine.Random.Range(0.0f, 1.0f), nNeighbours);
            //print("Threshold: " + thresholdOfAcceptance + " < Probability: " + probabilityOfChoice + ", Neighbors:" + nNeighbours);

            if (neighbours.Any(n => n.agent.State == AgentState.Final) && neighbours.Any(n => n.agent.State == AgentState.Inactive))    //Leave agents next to goal structure for the end
                continue;

            if (thresholdOfAcceptance < probabilityOfChoice)
                return agent; 
        }
    }

    public Agent AgentSelectionByScent(Agent[] agents)
    {
        while (true)
        {
            Agent agent = agents[UnityEngine.Random.Range(0, agents.Length)];

            float probabilityOfChoice = (float)agent.ScentValue / agent.ScentMax + (float)1e-3;
            float thresholdOfAcceptance = 1.0f - Mathf.Pow(UnityEngine.Random.Range((float)agent.ScentValue, agent.ScentMax) / agent.ScentMax, 2);

            if (thresholdOfAcceptance < probabilityOfChoice)
                //print("Threshold: " + thresholdOfAcceptance + " < Probability: " + probabilityOfChoice + ", ScentValue:" + agent.ScentValue);
                return agent;
        }
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

                Action nextAction = reconfiguration.ChoseAction(agent);

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
        while (!GoalShapeReached())    // What happens if nºGoalCells > NumAgents || nºGoalCells < NumAgents
        {
            if (GUI.Paused == false)
            {
                Agent agent = AgentSelectionByNeighbours(listAgents.Where(a => a.State == AgentState.Active || a.State == AgentState.Inactive).ToArray());
                AgentState state = agent.State;

                if (CheckFinalPosition(Seed, agent))
                {
                    agent.State = AgentState.Final;
                    agent.Obj.GetComponent<Renderer>().material.color = Color.grey;
                    UpdateSeed(ref Seed);
                    continue;
                }

                if (state == AgentState.Active)
                {
                    Action nextAction = reconfiguration.ChoseAction(agent);   

                    if (nextAction != Action.NoAction)
                    {
                        reconfiguration.ExecuteAction(this, nextAction, agent);
                        yield return new WaitForSeconds(currentSpeed);

    

                        // Update Scent
                        var newNeighbours = agent.Location.GetFaceNeighbours().Where(a => a.Alive == true);

                        if (newNeighbours.Count() != 0)    // In case structure disconnects?  -> FIX
                        {
                            int maxNeighbouringScent = newNeighbours.Select(s => s.agent.ScentValue).Max();
                            agent.ScentValue = maxNeighbouringScent != 0 ? maxNeighbouringScent - 1 : 0;  // Or current Scent +1?
                        }



                        // Check Final Position and Update State
                        if (CheckFinalPosition(Seed, agent)) 
                        {
                            agent.State = AgentState.Final;
                            agent.Obj.GetComponent<Renderer>().material.color = Color.grey;
                            UpdateSeed(ref Seed);
                        }
                        yield return null; //new WaitForSeconds(currentSpeed); //Needed here?  <- Probably not

                    }
                    else
                    {
                        //print("No action");
                        yield return null;   // Supposedly not needed if I have no disconnections  -> FIX
                    }
                        
                }
                else if (state == AgentState.Inactive)
                {
                    // Give an inactive agent the possibility to become active (depending on the changing system)
                    if (CanMove(agent))
                    {
                        agent.State = AgentState.Active;
                        agent.Obj.GetComponent<Renderer>().material.color = Color.blue;
                    }
                        
                }
            }
            else
            {
                yield return null;
            }
        }
        print("Goal Shape Reached");
    }



    ////////////////////////   SELF-ASSEMBLY STEPS  ////////////////////////

    // InitializeAssembly: Initializes the assembly process and agent states
    void InitializeAssembly(Agent[] agents)
    {
        // Initializes Goal Shape/Goal Cells
        SetGoalShape(geometryCoordinates);

        foreach (var agent in agents)
        {
            Cell currentCell = agent.Location;

            if (currentCell.GoalCell == true)
            {
                agent.State = AgentState.Final;
                agent.Obj.GetComponent<Renderer>().material.color = Color.grey;
            }

            else if (CanMove(agent))
            {
                agent.State = AgentState.Active;
                agent.Obj.GetComponent<Renderer>().material.color = Color.blue;
            }
            else
                agent.State = AgentState.Inactive;
        }
    }


    bool CanMove(Agent agent)         
    {
        int activeAgents = listAgents.Count(a => a.State == AgentState.Active);
        int nNeighbours = agent.Location.GetFaceNeighbours().Count(n => n.Alive);

        return nNeighbours == 1 || (nNeighbours < 4 && activeAgents < 20)  ? true : false;     
    }


    Agent ChooseSeed(Agent[] agents)  
    {
        Agent Seed = null;
        var staticAgents = agents.Where(a => a.State == AgentState.Final);

        foreach (var agent in staticAgents)
        {
            bool unfilledGoalCells = agent.Location.GetHorizontalFaceNeighbours().Any(n => n.GoalCell == true && (n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final)));

            if (unfilledGoalCells)
            {
                Seed = agent;
                Seed.ScentValue = Seed.ScentMax;
                Seed.State = AgentState.Seed;
                agent.Obj.GetComponent<Renderer>().material.color = Color.green;
                break;
            }
        }

        // Only Executes if all horizontal neighbours are in position
        if (Seed == null)
        {
            foreach (var agent in staticAgents)
            {
                bool unfilledGoalCells = agent.Location.GetVerticalFaceNeighbours().Any(n => n.GoalCell == true && (n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final)));

                if (unfilledGoalCells)
                {
                    Seed = agent;
                    Seed.State = AgentState.Seed;
                    Seed.ScentValue = Seed.ScentMax;
                    agent.Obj.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                }
            }
        }

        return Seed;
    }


    // UpdateSeed: Only updates the seed if all Horizontal Seed neighbours have been filled
    void UpdateSeed(ref Agent Seed)
    {
        var seedHorizontalNeighbours = Seed.Location.GetHorizontalFaceNeighbours().Where(n => n.GoalCell == true);
        bool unfilledHorizontalSeedNeighbours = seedHorizontalNeighbours.Any(n => n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final));

        if (unfilledHorizontalSeedNeighbours == false)
        {
            Seed.State = AgentState.Final;
            Seed.Obj.GetComponent<Renderer>().material.color = Color.grey;

            if (!GoalShapeReached())
            {
                Seed = ChooseSeed(listAgents);
                UpdateScents(Seed, Seed.ScentValue);
            }
            
        }
    }


    void PropagateScents(Agent Seed, int maxScent)
    {
        if (maxScent > 0)
        {
            var neighbours = Seed.Location.GetFaceNeighbours().Where(n => n.Alive == true);
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


    bool CheckFinalPosition(Agent Seed, Agent agent)
    {
        var seedHorizontalNeighbours = Seed.Location.GetHorizontalFaceNeighbours().Where(n => n.GoalCell == true);
        var seedVerticalNeighbours = Seed.Location.GetVerticalFaceNeighbours().Where(n => n.GoalCell == true);

        bool unfilledHorizontalSeedNeighbours = seedHorizontalNeighbours.Any(n => n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final));  // Only Horizontal

        bool isSeedHorizontalNeighbour = seedHorizontalNeighbours.Any(n => n.Location == agent.Location.Location);
        bool isSeedVerticalNeighbour = seedVerticalNeighbours.Any(n => n.Location == agent.Location.Location);


        if (agent.Location.GoalCell == true && isSeedHorizontalNeighbour)  // Forces Horizontal neighbours to be filled first
            return true;
        else if (agent.Location.GoalCell == true && unfilledHorizontalSeedNeighbours == false && isSeedVerticalNeighbour)
            return true;
        else
            return false;
    }


    void UpdateScents(Agent Seed, int maxScent)
    {
        //Reset Scents back to zero
        foreach (Agent agent in listAgents)
            if (agent.State != AgentState.Seed) agent.ScentValue = 0;

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
            AgentState state = listAgents[i].State;

            if (cell.GoalCell != true || state == AgentState.Active || state == AgentState.Inactive)
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

        for (float y = min.y + 0.5f; y < max.y; y++) 
        {
            for (float z = min.z + 0.5f; z < max.z; z++)
            {
                for (float x = min.x + 0.5f; x < max.x; x++)
                {
                    Vector3 point = new Vector3(x, y, z);   

                    if (CM.InsideCollider(point, new Vector3(0, 1000, 0))  &&
                        !CM.OutsideBoundaries(point, AreaMin, AreaMax))                  // CONTROL Nº OF POINTS (aka GOAL CELLS) ACC TO Nº of AGENTS?  -  MORE AGENTS -> MORE RESOLUTION
                        yield return point; 
                }
            }
        }
    }
}
