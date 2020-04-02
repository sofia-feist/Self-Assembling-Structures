using UnityEngine;
using System.Linq;




// AGENT STATE: Specifies the state of an agent
public enum AgentState
{
    Inactive,       // 0
    Active,      // 1 
    Seed,        // 2
    Final        // 3
}




public class Agent
{
    public GameObject Obj;
    public Cell Location;
    public AgentState State;

    public int ScentValue;    // Change Scent from int to Vector3Int for direction and proximity? (e.g., Seed at (10,10,10))
    public int ScentMax;



    // Constructor
    public Agent(GameObject prefab, Material material,  Vector3 center)
    {
        Obj = Object.Instantiate(prefab, center, Quaternion.identity);
        Obj.GetComponent<MeshRenderer>().sharedMaterial = material;
        State = AgentState.Inactive;

        ScentValue = 0;
        ScentMax = 50;
    }



}
