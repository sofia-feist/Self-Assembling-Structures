using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;


public class _3DSelfAssembly : MonoBehaviour
{
    CellGrid grid;
    ReconfigurationRules reconfiguration;
    CommonMethods CM = new CommonMethods();




    // Target Geometry
    public GameObject DefaultShape;


    // Agent Properties
    public GameObject agentPrefab;
    Agents agents;


    // Lists/Collections
    public static Agent[] listAgents; 
    public static List<Vector3> shapePoints;


    // Environment
    public static float AreaMin;
    public static float AreaMax;

    public static int NumAgents;
    public const int defaultNumAgents = 120;
    int maxActiveAgents;


    // Materials
    public PhysicMaterial physicsMaterial;
    public Material Material;

    // Visuals
    Color seed = new Color(155 / 255f, 10 / 255f, 10 / 255f);
    Color finalState = new Color(31 / 255f, 124 / 255f, 231 / 255f);
    Color movingState = new Color(0.75f, 0.75f, 0.75f);



    // Reconfiguration Speed (Time in seconds between actions)
    public const float maxSpeed = 0.0f;
    public const float minSpeed = 1.5f;
    public const float defaultSpeed = 0.75f;
    public float currentSpeed;



    // Diagnostics
    public static Stopwatch timer = new Stopwatch();
    List<GameObject> alive = new List<GameObject>();
    




    void Start()
    {
        // INITIALIZE NUMBER OF AGENTS
        if (GUI.Reset == false && GUI.SuggestedShapeSelected == false)
            NumAgents =  defaultNumAgents;   // Set Number of Agents to default Number: 120

        


        // INITIALIZE DEFAULT GOAL SHAPE
        if (GUI.GoalShape == null)
            GUI.GoalShape = DefaultShape;
        
        GameObject GoalShape = Instantiate(GUI.GoalShape, new Vector3(0.25f, 0, 0.25f), Quaternion.identity);  //Pivot: bottom left corner
        GoalShape.layer = 8;
        shapePoints = CM.PointsFromGeometry(GoalShape, Cell.CellSize).ToList();
        //if (GUI.SuggestedShapeSelected) 
            NumAgents = shapePoints.Count;
        Destroy(GoalShape);


        

        // SET MAX ACTIVE AGENTS
        if (NumAgents < 100)
            maxActiveAgents = 10;
        else if (NumAgents > 1000)
            maxActiveAgents = 100;
        else
            maxActiveAgents = NumAgents / 10;




        // SET AREA PROPERTIES
        AreaMin = 0;
        if (NumAgents <= 50)
            AreaMax = 2.5f;
        else if (NumAgents > 750)
            AreaMax = 7.5f;          // DO NOT EXCEED 500x500! 
        else
            AreaMax = 5.0f;// NumAgents;



        // INSTANTIATE EMPTY GRID OF CELLS
        grid = new CellGrid(AreaMin, AreaMax);

        // INSTANTIATE AGENTS
        agents = new Agents(grid, agentPrefab, Material, physicsMaterial, NumAgents);



        // INSTANTIATE LIST OF AGENTS (with different agent placement methods)
        //listAgents = agents.FillCellsWithAgents();  //Not in use
        //listAgents = agents.PlaceAgentsRandomly();  //Not in use
        //listAgents = agents.PlaceAgentsInGivenGeometry(shapePoints);         //Not in use   
        //listAgents = agents.PlaceConnectedAgentsRandomly(new Vector3Int(1, 0, 1));   //Not in use

        //if ((int)Mathf.Ceil(Mathf.Sqrt(NumAgents)) < grid.AreaSize)
        listAgents = agents.PlaceAgentsIn2DRows(new Vector3Int(1, 0, 1));
        //else
        //    listAgents = agents.PlaceAgentsIn3DRows(new Vector3Int(1, 0, 1));



        // INSTANTIATE RECONFIGURATION RULES
        reconfiguration = new ReconfigurationRules(currentSpeed, grid);

        //foreach (var item in shapePoints)
        //{
        //    Instantiate(agentPrefab, item, Quaternion.identity);
        //}
        //ModulesTest();
        //AddSupport();
        //AddJoints();


        // Send data to GH
        // Implement Events to send message (each time an agent moves)
        //UDPSend.SendData(UDPSend.EncodeMessage(listAgents.Select(a => a.Cell.Center).ToArray()));


        //foo();



        //OptimizationAlgorithms Optimize = new OptimizationAlgorithms(grid, 30, 500);
        ////var l = Optimize.GenerateGoalShape(new List<Vector3Int>() { new Vector3Int(5, 0, 5), new Vector3Int(10, 0, 5), new Vector3Int(5, 0, 10), new Vector3Int(10, 0, 10) }, 90);
        //Optimize.SimulatedAnnealing(new List<Vector3Int>() { new Vector3Int(5, 0, 5), new Vector3Int(10, 0, 5), new Vector3Int(5, 0, 10), new Vector3Int(10, 0, 10) }, 90);
        //foreach (var cell in Optimize.bestCandidate)
        //    cell.GoalCell = true;
    }





    // Update is called once per frame
    void Update()
    { 
        if (UDPReceive.MessageReceived)
        {
            shapePoints = UDPReceive.coordinates;
            SetGoalShape(shapePoints);
            UDPReceive.MessageReceived = false;
        }

        //CheckAliveCells();

    }


    void foo()
    {
        Vector3 winnerVector = new Vector3();
        List<float> disps = new List<float>();

        for (int i = 0; i < 30; i++)
        {
            int tries = 0;
            
            var randomAgent = listAgents[UnityEngine.Random.Range(0, listAgents.Length - 1)];
            var v = randomAgent.Cell.GetFaceNeighbours().Where(x => x.Alive == false).ToArray()[0].Center;
            Vector3[] N = new Vector3[] { v };

            while (tries < 3)
            {
                int count = 0;

                UDPSend.SendData(UDPSend.EncodeMessage(listAgents.Select(a => a.Cell.Center).Union(N).ToArray()));
                UDPReceive.MessageReceived = false;

                while (UDPReceive.MessageReceived == false && count < 3)
                {
                    Thread.Sleep(40);
                    count++;
                }

                if (UDPReceive.MessageReceived == true)
                {
                    print("Count: " + count + ", Tries: " + tries);
                    print("_" + UDPReceive.maxDisplacement.ToString("F10"));

                    disps.Add(UDPReceive.maxDisplacement);
                    if (disps.Min() >= UDPReceive.maxDisplacement)
                        winnerVector = v;

                    break;
                }

                tries++;
            }
        }

        print("Winner: " + winnerVector + "," + disps.Min().ToString("F10"));
        Instantiate(agentPrefab, winnerVector, Quaternion.identity);
    }



    void CheckAliveCells()
    {
        foreach (var ob in alive)
            Destroy(ob);

        alive.Clear();

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(0.048f, 0.048f, 0.048f);
        obj.GetComponent<BoxCollider>().enabled = false;
        obj.GetComponent<Renderer>().material.color = finalState;

        foreach (var cell in grid.Cells)
        {
            if (cell.GoalCell == true)
            {
                var clone = Instantiate(obj, cell.Center, Quaternion.identity);
                alive.Add(clone);
            }
        }

        Destroy(obj);
    }

    void ModulesTest()
    {
        int v = 5;
        int h = 4;
        listAgents = new Agent[v+h];

        for (int i = 0; i < v; i++)
        {
            Cell cell = grid.GetCell(new Vector3Int(0,i,0));
            cell.Alive = true;

            Agent agent = new Agent(agentPrefab, Material, physicsMaterial, cell.Center, i);
            cell.agent = agent;

            listAgents[i] = agent;
            listAgents[i].Cell = cell;
        }

        for (int i = 0; i < h; i++)
        {
            Cell cell = grid.GetCell(new Vector3Int(i+1, v-1, 0));
            cell.Alive = true;

            Agent agent = new Agent(agentPrefab, Material, physicsMaterial, cell.Center, v + i);
            cell.agent = agent;

            listAgents[v+i] = agent;
            listAgents[v+i].Cell = cell;
        }
    }

    void AddSupport()
    {
        for (int i = 0; i < listAgents.Length; i++)
        {
            Agent agent = listAgents[i];

            if (agent.Cell.Location.y == 0)
            {
                //agent.Rb.constraints = RigidbodyConstraints.FreezeAll; // Other way to do it

                FixedJoint joint = agent.Obj.AddComponent<FixedJoint>();
                joint.connectedBody = GameObject.Find("GroundPlane").GetComponent<Rigidbody>();
                joint.breakForce = Agent.breakForce;
                joint.breakTorque = Agent.breakTorque;
            }
        }

    }

    

    void AddJoints()
    {
        for (int i = 0; i < listAgents.Length; i++)
        {
            foreach (var neighbour in listAgents[i].Cell.GetFaceNeighbours().Where(n=>n.Alive))
            {
                FixedJoint joint = listAgents[i].Obj.AddComponent<FixedJoint>();
                joint.connectedBody = neighbour.agent.Obj.GetComponent<Rigidbody>();
                joint.breakForce = Agent.breakForce;
                joint.breakTorque = Agent.breakTorque;
            }
        }
    }

    void UpdateJoints(Agent agent)
    {
        foreach (var neighbour in agent.Cell.GetFaceNeighbours().Where(n => n.Alive && n.agent.State == AgentState.Final))
        {
            FixedJoint joint = agent.Obj.AddComponent<FixedJoint>();
            joint.connectedBody = neighbour.agent.Obj.GetComponent<Rigidbody>();
            joint.breakForce = Agent.breakForce;
            joint.breakTorque = Agent.breakTorque;
        }
    }
    void IsSupport(Agent agent)
    {
        if (agent.Cell.Location.y == 0)
        {
            FixedJoint joint = agent.Obj.AddComponent<FixedJoint>();
            joint.connectedBody = GameObject.Find("GroundPlane").GetComponent<Rigidbody>();
            joint.breakForce = Agent.breakForce;
            joint.breakTorque = Agent.breakTorque;
        }
    }


    






    // AGENT SELECTION
    //AgentSelectionByNeighbours: The bigger the number of neighbours, the less probability the agent has to be chosen
    public Agent AgentSelectionByNeighbours(Agent[] agents)
    {
        while (true)
        {
            Agent agent = agents[UnityEngine.Random.Range(0, agents.Length)];
            var neighbours = agent.Cell.GetFaceNeighbours().Where(n => n.Alive);
            int nNeighbours = neighbours.Count();

            float probabilityOfChoice = 1.0f - nNeighbours / 6.0f + (float)1e-3;
            float thresholdOfAcceptance = 1.0f - Mathf.Pow(UnityEngine.Random.Range(0.0f, 1.0f), nNeighbours);

            if (neighbours.Any(n => n.agent.State == AgentState.Final) &&    // Leave inactive agents next to goal structure with a lower probability of being chosen to avoid disconnections
                agents.Any(a => a.Cell.GetFaceNeighbours().Count(n => n.Alive && n.agent.State == AgentState.Final) == 0))
                continue;

            if (thresholdOfAcceptance < probabilityOfChoice)
                return agent;
        }
    }


    //AgentSelectionByScent: The higher the number of scent value, the more probability the agent has to be chosen (NOT IN USE ANYMORE)
    public Agent AgentSelectionByScent(Agent[] agents)
    {
        while (true)
        {
            Agent agent = agents[UnityEngine.Random.Range(0, agents.Length)];
            var neighbours = agent.Cell.GetFaceNeighbours().Where(n => n.Alive);

            float probabilityOfChoice = (float)agent.ScentValue / agent.ScentMax + (float)1e-3;
            float thresholdOfAcceptance = 1.0f - Mathf.Pow(UnityEngine.Random.Range((float)agent.ScentValue, agent.ScentMax) / agent.ScentMax, 2);

            if (neighbours.Any(n => n.agent.State == AgentState.Seed))
                probabilityOfChoice = 1;

            if (thresholdOfAcceptance < probabilityOfChoice)
                return agent;
        }
    }









    ////////////////////////   SELF-RECONFIGURATION  ////////////////////////

    // RandomReconfiguration: Random Reconfiguration process to test the rules
    public IEnumerator RandomReconfiguration()
    {
        while (true)
        {
            if (GUI.Paused == false)
            {
                Agent agent = AgentSelectionByNeighbours(listAgents);

                Action nextAction = reconfiguration.ChoseAction(listAgents, agent);

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
            timer.Start();
            if (GUI.Paused == false)
            {
                Agent agent;

                if (listAgents.Count(a => a.State == AgentState.Inactive) > 0 && listAgents.Count(a => a.State == AgentState.Active) < maxActiveAgents)
                    agent = AgentSelectionByNeighbours(listAgents.Where(a => a.State == AgentState.Inactive).ToArray());
                else
                    agent = AgentSelectionByScent(listAgents.Where(a => a.State == AgentState.Active).ToArray());

                AgentState state = agent.State;

                if (CheckFinalPosition(Seed, agent))
                {
                    agent.State = AgentState.Final;
                    agent.Obj.GetComponent<Renderer>().material.color = finalState;

                    //IsSupport(agent);
                    //UpdateJoints(agent);
                    UpdateSeed(ref Seed);
                    continue;
                }

                if (state == AgentState.Active)
                {
                    Action nextAction = reconfiguration.ChoseAction(listAgents, agent);

                    if (nextAction != Action.NoAction)
                    {
                        reconfiguration.ExecuteAction(this, nextAction, agent);
                        agent.StepCount++;
                        yield return new WaitForSeconds(currentSpeed);


                        // Update Scent
                        var newNeighbours = agent.Cell.GetFaceNeighbours().Where(a => a.Alive == true);

                        int maxNeighbouringScent = newNeighbours.Select(s => s.agent.ScentValue).Max();
                        agent.ScentValue = maxNeighbouringScent != 0 ? maxNeighbouringScent - 1 : 0;


                        // Check Final Position and Update State
                        if (CheckFinalPosition(Seed, agent))
                        {
                            agent.State = AgentState.Final;
                            agent.Obj.GetComponent<Renderer>().material.color = finalState;

                            //IsSupport(agent);
                            //UpdateJoints(agent);
                            UpdateSeed(ref Seed);
                        }
                        yield return null; //new WaitForSeconds(currentSpeed);    <-  Probably not needed?  

                    }
                    else
                    {
                        yield return null;
                    }

                }
                else if (state == AgentState.Inactive)
                {
                    // Give an inactive agent the possibility to become active (depending on the changing system)
                    if (CanMove(agent))
                    {
                        agent.State = AgentState.Active;
                        agent.Obj.GetComponent<Renderer>().material.color = movingState;
                    }

                }
            }
            else
            {
                yield return null;
            }
        }

        foreach (Agent agent in listAgents.Where(a => a.Cell.GoalCell == true))
            agent.Obj.GetComponent<Renderer>().material.color = finalState;

        // Diagnostics
        //print(timer.Elapsed);
        //print("Goal Shape Reached");
        //print(listAgents.Average(a => a.StepCount));
        //print(listAgents.Where(a => a.StepCount != 0).Min(a => a.StepCount));
        //print(listAgents.Max(a => a.StepCount));
    }







    ////////////////////////   SELF-ASSEMBLY STEPS  ////////////////////////

    // InitializeAssembly: Initializes the assembly process and agent states
    void InitializeAssembly(Agent[] agents)
    {
        // Initializes Goal Shape/Goal Cells
        SetGoalShape(shapePoints);

        CM.RandomShuffle(agents);

        foreach (var agent in agents)
        {
            Cell currentCell = agent.Cell;
            int nNeighbours = currentCell.GetFaceNeighbours().Count(n => n.Alive);

            if (currentCell.GoalCell == true)
            {
                agent.State = AgentState.Final;
                agent.Obj.GetComponent<Renderer>().material.color = finalState;
                IsSupport(agent);
            }
            else if (nNeighbours < 4 && listAgents.Count(a => a.State == AgentState.Active) < maxActiveAgents)
            {
                agent.State = AgentState.Active;
                agent.Obj.GetComponent<Renderer>().material.color = movingState;
            }
            else
                agent.State = AgentState.Inactive;
        }
    }


    // CanMove: Determines whether an inactive agent can become active or not
    bool CanMove(Agent agent)         
    {
        var neighbours = agent.Cell.GetFaceNeighbours().Where(n => n.Alive);
        int inactiveNeighbours = neighbours.Count(a => a.agent.State == AgentState.Inactive);
        int nNeighbours = neighbours.Count();

        return (nNeighbours == 1 || inactiveNeighbours < 4) ? true : false;
    }


    // ChooseSeed: Chooses a Seed from the list of Agents
    Agent ChooseSeed(Agent[] agents)
    {
        var staticAgents = agents.Where(a => a.State == AgentState.Final).OrderBy(a => a.Cell.Location.y);
        List<Agent> potentialHorizontalSeeds = new List<Agent>();
        List<Agent> potentialVerticalSeeds = new List<Agent>();

        foreach (var agent in staticAgents)
        {
            bool unfilledHorizontalGoalCells = agent.Cell.GetHorizontalFaceNeighbours().Any(n => n.GoalCell == true && (n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final)));
            bool unfilledVerticalGoalCells = agent.Cell.GetVerticalFaceNeighbours().Any(n => n.GoalCell == true && (n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final)));

            if (unfilledHorizontalGoalCells)
                potentialHorizontalSeeds.Add(agent);

            if (unfilledVerticalGoalCells)
                potentialVerticalSeeds.Add(agent);
        }

        Agent Seed;

        if (potentialHorizontalSeeds.Count != 0)     // <- GIVE PRIORITY TO HORIZONTAL SEEDS (Assembly order: horizontal layers first)
            Seed = potentialHorizontalSeeds[0];
        else
            Seed = potentialVerticalSeeds[0];

        Seed.State = AgentState.Seed;
        Seed.ScentValue = Seed.ScentMax;
        Seed.Obj.GetComponent<Renderer>().material.color = seed;

        return Seed;
    }


    // PropagateScents: Propagates Scent emitted from the Seed
    void PropagateScents(Agent Seed, int maxScent)
    {
        if (maxScent > 0)
        {
            var neighbours = Seed.Cell.GetFaceNeighbours().Where(n => n.Alive == true);
            var recursiveNeighbours = new List<Cell>();

            foreach (var neighbour in neighbours)
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


    // UpdateSeed: Only updates the seed if all Horizontal Seed neighbours have been filled
    void UpdateSeed(ref Agent Seed)
    {
        var seedHorizontalNeighbours = Seed.Cell.GetHorizontalFaceNeighbours().Where(n => n.GoalCell == true);
        bool unfilledHorizontalSeedNeighbours = seedHorizontalNeighbours.Any(n => n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final));

        if (unfilledHorizontalSeedNeighbours == false)
        {
            Seed.State = AgentState.Final;
            Seed.Obj.GetComponent<Renderer>().material.color = finalState;

            if (!GoalShapeReached())
            {
                Seed = ChooseSeed(listAgents);
                UpdateScents(Seed, Seed.ScentValue);
            }
            
        }
    }


    // CheckFinalPosition: Checks if an agent has reached a final position or not
    bool CheckFinalPosition(Agent Seed, Agent agent)
    {
        var seedHorizontalNeighbours = Seed.Cell.GetHorizontalFaceNeighbours().Where(n => n.GoalCell == true);
        var seedVerticalNeighbours = Seed.Cell.GetVerticalFaceNeighbours().Where(n => n.GoalCell == true);

        bool unfilledHorizontalSeedNeighbours = seedHorizontalNeighbours.Any(n => n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final));

        bool isSeedHorizontalNeighbour = seedHorizontalNeighbours.Any(n => n.Location == agent.Cell.Location);
        bool isSeedVerticalNeighbour = seedVerticalNeighbours.Any(n => n.Location == agent.Cell.Location);


        if (agent.Cell.GoalCell == true && isSeedHorizontalNeighbour)    // Forces Horizontal neighbours to be filled first
            return true;
        else if (agent.Cell.GoalCell == true && unfilledHorizontalSeedNeighbours == false && isSeedVerticalNeighbour)
            return true;
        else
            return false;
    }


    // UpdateScents: Updates Scent Propagation
    void UpdateScents(Agent Seed, int maxScent)
    {
        //Reset Scents back to zero
        foreach (Agent agent in listAgents)
            if (agent.State != AgentState.Seed) agent.ScentValue = 0;

        //Propagate again
        PropagateScents(Seed, maxScent);
    }





    //////////////////////////// GOAL SHAPE //////////////////////////

    // SetGoalShape: Sets Goal Cells according to the given list of Positions
    public void SetGoalShape(IEnumerable<Vector3> listPositions)
    {
        for (int i = 0; i < listPositions.Count(); i++)
        {
            Vector3 point = listPositions.ElementAt(i);

            if (!CM.OutsideBoundaries(point, AreaMin, AreaMax))
            {
                Vector3Int location = grid.GetCellLocation(point);

                Cell currentCell = grid.GetCell(location);
                currentCell.GoalCell = true;
            }
        }
    }

    

    


    // ClearGoalCells: Sets all goal cells back to false
    public void ResetSystem()
    {
        foreach (Agent agent in listAgents)
        {
            agent.Obj.GetComponent<Renderer>().material.color = Color.white;
            agent.State = AgentState.Inactive;
        } 

        foreach (var cell in grid.Cells)
            cell.GoalCell = false;
    }


    // GoalShapeReached: Verifies if all Agents have reached their final state; aka are located in a GoalCell
    public bool GoalShapeReached()
    {
        if (listAgents.Where(a => a.State != AgentState.Seed).All(a => a.State == AgentState.Final) ||    // || a.Location.GoalCell == true
            grid.Cells.Cast<Cell>().Where(cell => cell.GoalCell == true).All(cell => cell.Alive == true))
            return true;
        else
            return false;
    }
}
