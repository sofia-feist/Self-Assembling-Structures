using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;

public class _3DSelfAssembly : MonoBehaviour
{
    CommonMethods CM = new CommonMethods();

    // Target Geometry
    public GameObject GEO;
    GameObject GEOInstance;

    // Agent Properties
    public GameObject agent;
    Agents agents;
    float velocity = 1.0f;
    float minDistance = 1.0f;       // Minimum Distance between Agents: 1.0 = r*2 of a unit Agent

    // Lists/Collections
    List<GameObject> listAgents = new List<GameObject>(NumAgents);
    List<Vector3> geometryCoordinates;

    // Environment
    public static float AreaMin = 0f;
    public static float AreaMax = 100f;
    float AreaInfluence = 2f;                    // Area of Influence of each agent: how far they can "see" and react
    static int NumAgents = 100;

    // Render Effects
    public Material whiteGlowMaterial;
    public Material blueGlowMaterial;



    

    // Start is called before the first frame update
    void Start()
    {
        GEOInstance = Instantiate(GEO, Vector3.zero, Quaternion.identity);

        geometryCoordinates = PointsFromGeometry(GEOInstance);
        print(PointsFromGeometry(GEOInstance).Count);

        // INSTANTIATE AGENTS (with different agent placement methods)
        NumAgents = geometryCoordinates.Count;
        agents = new Agents(agent, whiteGlowMaterial, NumAgents);
        //agents.PlaceAgentsInRows(new Vector3(-50, 0, 10));
        listAgents = agents.listAgents;

        //print(InsideCollider2(new Vector3(3, 3, 0)));

        foreach (var p in geometryCoordinates)
        {
            Instantiate(agent, p, Quaternion.identity);
        }

        Destroy(GEOInstance);

        //StartCoroutine(SequentialAssembly(SeekAssignedPosition));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // PointsFromGeometry: Calculates the target points for the Self-Assembly Algorithm from an 3D geometric input
    public List<Vector3> PointsFromGeometry(GameObject geometry)
    {
        List<Vector3> positions = new List<Vector3>();

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

                    if (InsideCollider(point, new Vector3(1000, 1000, 1000)))
                        positions.Add(point);
                }
            }
        }
        return positions;
    }

    bool InsideCollider(Vector3 point)
    {
        int count = 0;
        RaycastHit hit;

        Vector3 rayStart = point + new Vector3(1000, 1000, 1000);
        Vector3 direction = point - rayStart;
        Vector3 hitPoint = rayStart;

        while (hitPoint != point)
        {
            if (Physics.Raycast(hitPoint, direction, out hit))
            {
                hitPoint = hit.point + (direction.normalized / 10.0f);
                count++;
            }
            else 
            {
                hitPoint = point;
            }
        }

        while (hitPoint != rayStart)
        {
            if (Physics.Raycast(hitPoint, -direction, out hit))
            {
                hitPoint = hit.point + (-direction.normalized / 10.0f);
                count++;
            }
            else
            {
                hitPoint = rayStart;
            }
        }

        // Check how many intersections there are
        if (count % 2 == 0)
        {
            return false;
        }
        else // if (count % 2 == 1)
        {
            return true;
        }
    }

    bool InsideCollider(Vector3 point, Vector3 rayPosition)
    {
        int count = 0;
        RaycastHit hit;

        Vector3 rayStart =  rayPosition;
        Vector3 direction = point - rayStart;
        Vector3 hitPoint = rayStart;

        while (Physics.Raycast(hitPoint, direction, out hit, direction.magnitude) && count < 100)
        {
            hitPoint = hit.point + (direction.normalized / 10.0f);
            count++;
        }

        hitPoint = point;

        while (Physics.Raycast(hitPoint, -direction, out hit, direction.magnitude) && count < 100)
        {
            hitPoint = hit.point + (-direction.normalized / 10.0f);
            count++;
        }

        // Check how many intersections there are
        if (count % 2 == 0)
            return false;
        else   // if (count % 2 == 1)
            return true;
    }


    // SelfAssembly delegate
    public delegate IEnumerator SelfAssembly(GameObject agent, Vector3 vector);  // or Func<GameObject, Vector3, IEnumerator> Name

    // SequentialAssembly: Agents move one by one to form an image; next agent starts a given time after the first one, forming a queue  
    IEnumerator SequentialAssembly(SelfAssembly SelfAssemblyMethod)
    {
        for (int i = 0; i < listAgents.Count; i++)
        {
            StartCoroutine(SelfAssemblyMethod(listAgents[i], geometryCoordinates[i]));
            yield return new WaitForSeconds(1.4f);
        }
    }


    // SeekAssignedPosition: Agents seek their own assigned position from a list of positions
    IEnumerator SeekAssignedPosition(GameObject agent, Vector3 desiredPosition)
    {
        while (true)
        {
            Vector3 agentPosition = agent.transform.position;
            Vector3 seekDirection = (desiredPosition - agentPosition).normalized * velocity;
            Vector3 newPosition = agentPosition + seekDirection;
            List<Vector3> agentPositions = listAgents.Select(a => a.transform.position).ToList();

            Vector3 closestAgent = CM.ClosestAgent(agentPosition, agentPositions);
            Vector3 awayFromAgent = (agentPosition - closestAgent).normalized * velocity;

            if (Vector3.Distance(agentPosition, desiredPosition) <= minDistance)
            {
                agent.transform.Translate(desiredPosition - agentPosition);
                agent.GetComponent<Renderer>().material = blueGlowMaterial;
                break;
            }
            else if (!CM.WillCollide(AreaInfluence, agentPosition, newPosition, agentPositions) ||
                     Vector3.Distance(agentPosition, desiredPosition) < AreaInfluence ||
                     !CM.OpposingForces(seekDirection, awayFromAgent))
            {
                agent.transform.Translate(seekDirection);
            }
            else
            {
                EdgeFollowing(agent, minDistance * 2f);
            }
            yield return new WaitForSeconds(0.03f);
        }
    }


    // EdgeFollowing: Makes a given agent follow along the edges of a group of agents, while maintaining a given distance
    public void EdgeFollowing(GameObject agent, float distance)
    {
        Vector3 agentPosition = agent.transform.position;
        List<Vector3> agentPositions = listAgents.Select(a => a.transform.position).ToList();

        Vector3 closestAgent = CM.ClosestAgent(agentPosition, agentPositions);
        float distanceToColidingAgent = Vector3.Distance(agentPosition, closestAgent);
        Vector3 awayFromAgent = (agentPosition - closestAgent).normalized * velocity;
        Vector3 closerToAgent = -awayFromAgent;
        Vector3 perpendicularToAgent = CM.PerpendicularVector(awayFromAgent, 1);

        if (distanceToColidingAgent > distance)
            agent.transform.Translate((perpendicularToAgent + closerToAgent * 0.2f).normalized * velocity);    // To account for small curving displacements
        else if (distanceToColidingAgent < minDistance)
            agent.transform.Translate((perpendicularToAgent + awayFromAgent * 0.2f).normalized * velocity);
        else
            agent.transform.Translate(perpendicularToAgent);
    }
}
