using UnityEngine;



// AGENT STATE: Specifies the state of an agent
public enum AgentState
{
    Inactive,       // 0
    Active,      // 1 
    Seed,        // 2
    Final        // 3
}

public enum Tasks   // Roles of each module!!
{
    Building,       // 0
    Walking,      // 1 
    TemporarySupport,        // 2
    Stopped
}




public class Agent
{
    public Cell Cell;
    public AgentState State;
    public GameObject Obj;

    internal Rigidbody Rb;
    internal BoxCollider BoxColl;

    public int ScentValue;    // Change Scent from int to Vector3Int for direction and proximity? (e.g., Seed at (70,70,70))
    public int ScentMax;
    public int StepCount;

    internal static float breakForce = 400;  // What values should I give?
    internal static float breakTorque = 400;

    




    // Constructor
    public Agent(GameObject prefab, Material material, PhysicMaterial physicsMaterial, Vector3 center)
    {
        Obj = Object.Instantiate(prefab, center, Quaternion.identity);
        Obj.GetComponent<MeshRenderer>().sharedMaterial = material;

        BoxColl = Obj.AddComponent<BoxCollider>();
        BoxColl.size = new Vector3(0.98f, 0.98f, 0.98f);
        BoxColl.sharedMaterial = physicsMaterial;
        BoxColl.enabled = false;

        Rb = Obj.GetComponent<Rigidbody>();
        Rb.maxDepenetrationVelocity = 5;    // Should this stay?
        Rb.useGravity = false;


        State = AgentState.Inactive;

        ScentValue = 0;
        ScentMax = 70;
        StepCount = 0;
    }


}
