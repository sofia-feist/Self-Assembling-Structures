using UnityEngine;

public class Agent
{
    public GameObject Obj;
    public Cell Location;

    // Constructor
    public Agent(GameObject prefab, Vector3 center)
    {
        Obj = Object.Instantiate(prefab, center, Quaternion.identity);
    }

}
