﻿using System;
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
    Agent[] listAgents; 
    public static List<Vector3> shapePoints;


    // Environment
    public static int AreaMin;
    public static int AreaMax;
    public static int NumAgents;
    public const int defaultNumAgents = 120;
    int maxActiveAgents;


    // Visual Effects
    public Material Material;
    Color seed = new Color(155 / 255f, 10 / 255f, 10 / 255f);
    Color finalState = new Color(31 / 255f, 124 / 255f, 231 / 255f);
    Color movingState = new Color(0.75f, 0.75f, 0.75f);



    // Reconfiguration Speed (Time in seconds between actions)
    public const float maxSpeed = 0.0f;
    public const float minSpeed = 1.5f;
    public const float defaultSpeed = 0.75f;
    public float currentSpeed;



    // Performance Diagnostic
    public Stopwatch timer = new Stopwatch();



    void Start()
    {
        // INITIALIZE NUMBER OF AGENTS
        if (GUI.Reset == false && GUI.SuggestedShapeSelected == false)
            NumAgents =  defaultNumAgents;   // Set Number of Agents to default Number: 120

        


        // INITIALIZE DEFAULT GOAL SHAPE
        if (GUI.GoalShape == null)
            GUI.GoalShape = DefaultShape;
        
        GameObject GoalShape = Instantiate(GUI.GoalShape, new Vector3Int(5, 0, 5), Quaternion.identity);  //Pivot: bottom left corner
        shapePoints = CM.PointsFromGeometry(GoalShape).ToList();
        if (GUI.SuggestedShapeSelected) NumAgents = shapePoints.Count;
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
            AreaMax = 10;
        else if (NumAgents > 750)
            AreaMax = 150;          // DO NOT EXCEED 500x500! 
        else
            AreaMax = NumAgents / 5;                                   



        // INSTANTIATE EMPTY GRID OF CELLS
        grid = new CellGrid(AreaMin, AreaMax);



        // INSTANTIATE AGENTS
        agents = new Agents(grid, agentPrefab, Material, NumAgents);



        // INSTANTIATE LIST OF AGENTS (with different agent placement methods)
        //listAgents = agents.FillCellsWithAgents();  //Not in use
        //listAgents = agents.PlaceAgentsRandomly();  //Not in use
        //listAgents = agents.PlaceAgentsInGivenGeometry(geometryCoordinates);         //Not in use   
        //listAgents = agents.PlaceConnectedAgentsRandomly(new Vector3Int(1, 0, 1));   //Not in use

        if ((int)Mathf.Ceil(Mathf.Sqrt(NumAgents)) < grid.AreaSize)
            listAgents = agents.PlaceAgentsIn2DRows(new Vector3Int(1, 0, 1));
        else
            listAgents = agents.PlaceAgentsIn3DRows(new Vector3Int(1, 0, 1));



        // INSTANTIATE RECONFIGURATION RULES
        reconfiguration = new ReconfigurationRules(currentSpeed);
    }


    // Update is called once per frame
    void Update()
    {

    }





    // AGENT SELECTION
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

            if (agent.State == AgentState.Inactive &&                           // Leave inactive agents next to goal structure with a lower probability of being chosen to avoid disconnections 
                neighbours.Any(n => n.agent.State == AgentState.Final) &&
                listAgents.Any(a => a.State == AgentState.Inactive && a.Location.GetFaceNeighbours().Count(n => n.Alive && n.agent.State == AgentState.Final) == 0))     
                continue;

            if (neighbours.Any(n => n.agent.State == AgentState.Seed))
                probabilityOfChoice = 1;

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
            var neighbours = agent.Location.GetFaceNeighbours().Where(n => n.Alive);

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
        while (!GoalShapeReached()) 
        {
            timer.Start();
            if (GUI.Paused == false)
            {
                Agent agent;
                if (listAgents.Count(a => a.State == AgentState.Inactive) > 0 && listAgents.Count(a => a.State == AgentState.Active) < maxActiveAgents)
                    agent = AgentSelectionByNeighbours(listAgents.Where(a => a.State == AgentState.Inactive).ToArray());
                else
                    agent = AgentSelectionByNeighbours(listAgents.Where(a => a.State == AgentState.Active).ToArray());

                AgentState state = agent.State;

                if (CheckFinalPosition(Seed, agent))
                {
                    agent.State = AgentState.Final;
                    agent.Obj.GetComponent<Renderer>().material.color = finalState;
                    UpdateSeed(ref Seed);
                    continue;
                }

                if (state == AgentState.Active)
                {
                    Action nextAction = reconfiguration.ChoseAction(agent);   

                    if (nextAction != Action.NoAction)
                    {
                        reconfiguration.ExecuteAction(this, nextAction, agent);
                        agent.StepCount++;
                        yield return new WaitForSeconds(currentSpeed);


                        // Update Scent
                        var newNeighbours = agent.Location.GetFaceNeighbours().Where(a => a.Alive == true);

                        int maxNeighbouringScent = newNeighbours.Select(s => s.agent.ScentValue).Max();
                        agent.ScentValue = maxNeighbouringScent != 0 ? maxNeighbouringScent - 1 : 0;         


                        // Check Final Position and Update State
                        if (CheckFinalPosition(Seed, agent)) 
                        {
                            agent.State = AgentState.Final;
                            agent.Obj.GetComponent<Renderer>().material.color = finalState;
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

        foreach (Agent agent in listAgents.Where(a => a.Location.GoalCell == true))
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
            Cell currentCell = agent.Location;
            int nNeighbours = currentCell.GetFaceNeighbours().Count(n => n.Alive);

            if (currentCell.GoalCell == true)
            {
                agent.State = AgentState.Final;
                agent.Obj.GetComponent<Renderer>().material.color = finalState;
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
        var neighbours = agent.Location.GetFaceNeighbours().Where(n => n.Alive);
        int inactiveNeighbours = neighbours.Count(a => a.agent.State == AgentState.Inactive);
        int nNeighbours = neighbours.Count();

        return (nNeighbours == 1 || inactiveNeighbours < 4) ? true : false;
    }


    // ChooseSeed: Chooses a Seed from the list of Agents
    Agent ChooseSeed(Agent[] agents)
    {
        var staticAgents = agents.Where(a => a.State == AgentState.Final).OrderBy(a => a.Location.Location.y);
        List<Agent> potentialHorizontalSeeds = new List<Agent>();
        List<Agent> potentialVerticalSeeds = new List<Agent>();

        foreach (var agent in staticAgents)
        {
            bool unfilledHorizontalGoalCells = agent.Location.GetHorizontalFaceNeighbours().Any(n => n.GoalCell == true && (n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final)));
            bool unfilledVerticalGoalCells = agent.Location.GetVerticalFaceNeighbours().Any(n => n.GoalCell == true && (n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final)));

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
            var neighbours = Seed.Location.GetFaceNeighbours().Where(n => n.Alive == true);
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
        var seedHorizontalNeighbours = Seed.Location.GetHorizontalFaceNeighbours().Where(n => n.GoalCell == true);
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
        var seedHorizontalNeighbours = Seed.Location.GetHorizontalFaceNeighbours().Where(n => n.GoalCell == true);
        var seedVerticalNeighbours = Seed.Location.GetVerticalFaceNeighbours().Where(n => n.GoalCell == true);

        bool unfilledHorizontalSeedNeighbours = seedHorizontalNeighbours.Any(n => n.Alive == false || (n.Alive == true && n.agent.State != AgentState.Final));

        bool isSeedHorizontalNeighbour = seedHorizontalNeighbours.Any(n => n.Location == agent.Location.Location);
        bool isSeedVerticalNeighbour = seedVerticalNeighbours.Any(n => n.Location == agent.Location.Location);


        if (agent.Location.GoalCell == true && isSeedHorizontalNeighbour)    // Forces Horizontal neighbours to be filled first
            return true;
        else if (agent.Location.GoalCell == true && unfilledHorizontalSeedNeighbours == false && isSeedVerticalNeighbour)
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
