using System;
using System.Collections.Generic;
using UnityEngine;



// AGENT STATE: Specifies the state of an agent
public enum AgentState
{
    Inactive,       // 0
    Active,      // 1 
    Seed,        // 2
    Final        // 3
}

public enum Role   // Roles/Task of each module!!
{
    Building,      
    Walking,      
    Support,       
    Stopped
}

public enum Connections 
{
    North,
    South,
    East,
    West,
    Down,
    Up
}




public class Agent
{
    public Cell Cell;
    public AgentState State;
    public GameObject Obj;

    internal int Id;

    internal Rigidbody Rb;
    internal BoxCollider BoxColl;

    public int ScentValue;    // Use Scent vectors for direction and proximity? (or Seed at (70,70,70))
    public int ScentMax;
    public int StepCount;

    internal static float breakForce = 400;  // What values should I give?
    internal static float breakTorque = 400;

    internal List<Connections> activeConnectors;
    //Connector info?

    




    // Constructor
    public Agent(GameObject prefab, Material material, PhysicMaterial physicsMaterial, Vector3 center, int _Id)
    {
        Id = _Id;

        Obj = UnityEngine.Object.Instantiate(prefab, center, Quaternion.identity);
        Obj.GetComponent<MeshRenderer>().sharedMaterial = material;

        BoxColl = Obj.AddComponent<BoxCollider>();
        BoxColl.sharedMaterial = physicsMaterial;

        Rb = Obj.GetComponent<Rigidbody>();


        State = AgentState.Inactive;

        ScentValue = 0;
        ScentMax = 20;
        StepCount = 0;

        activeConnectors = new List<Connections>();
    }


    public List<Connections> GetActiveConnectors()
    {
        List<Connections> activeConnectors = new List<Connections>();
        Cell east = Cell.MiddleEast();
        Cell west = Cell.MiddleWest();
        Cell north = Cell.MiddleNorth();
        Cell south = Cell.MiddleSouth();
        Cell down = Cell.Bottom();
        Cell up = Cell.Up();
        

        if (east?.Alive == true) activeConnectors.Add(Connections.East);
        if (west?.Alive == true) activeConnectors.Add(Connections.West);

        if (north?.Alive == true) activeConnectors.Add(Connections.North);
        if (south?.Alive == true) activeConnectors.Add(Connections.South);

        if (up?.Alive == true) activeConnectors.Add(Connections.Up);
        if (down?.Alive == true) activeConnectors.Add(Connections.Down);

        return activeConnectors;
    }


}
