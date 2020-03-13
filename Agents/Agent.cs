using UnityEngine;
using System.Linq;




// AGENT STATE: Specifies the state of an agent
public enum AgentState
{
    Sleep,       // 0
    Active,      // 1
    //Moving,    
    Seed,        // 2
    Final        // 3
}




public class Agent
{
    public GameObject Obj;
    public Cell Location;
    public AgentState State;

    public int ScentValue;
    public int ScentMax;



    // Constructor
    public Agent(GameObject prefab, Material material,  Vector3 center)
    {
        Obj = Object.Instantiate(prefab, center, Quaternion.identity);
        Obj.GetComponent<MeshRenderer>().sharedMaterial = material;
        State = AgentState.Sleep;

        ScentValue = 0;
        ScentMax = 10;
    }


    



}
