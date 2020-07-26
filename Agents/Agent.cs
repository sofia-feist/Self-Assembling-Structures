using System;
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
        ScentMax = 70;
        StepCount = 0;
    }


}
