using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;

public class _3DSelfAssembly : MonoBehaviour
{
    CommonMethods CM = new CommonMethods();
    CellGrid grid = new CellGrid(AreaMin, AreaMax);
    ReconfigurationRules reconfiguration = new ReconfigurationRules();

    // Target Geometry
    public GameObject GEO;
    GameObject GEOInstance;

    // Agent Properties
    public GameObject agent;
    Agents agents;
    float velocity = 1.0f;
    float minDistance = 1.0f;       // Minimum Distance between Agents: 1.0 = r*2 of a unit Agent

    // Lists/Collections
    List<GameObject> listAgentsObj = new List<GameObject>(NumAgents);
    List<Agent> listAgents = new List<Agent>(NumAgents); 
    List<Vector3> geometryCoordinates;

    // Environment
    public static int AreaMin = 0;
    public static int AreaMax = 100;
    float AreaInfluence = 2f;                    // Area of Influence of each agent: how far they can "see" and react
    static int NumAgents = 50;

    // Render Effects
    public Material Material;




    // Start is called before the first frame update
    void Start()
    { 
        GEOInstance = Instantiate(GEO, new Vector3(20, 0, 20), Quaternion.identity);

        geometryCoordinates = PointsFromGeometry(GEOInstance).ToList();
        print(geometryCoordinates.Count);

        // INSTANTIATE AGENTS (with different agent placement methods)
        //NumAgents = geometryCoordinates.Count;
        agents = new Agents(agent, Material, NumAgents);
        //agents.PlaceAgentsInRows(new Vector3(-50, 0, 10));
        //agents.FillCellsWithAgents(grid.Cells);
        //agents.PlaceAgentsInRowsCells(new Vector3(0, 0, 0), grid.Cells);
        agents.PlaceAgentsInGivenGeometry(geometryCoordinates, grid.Cells, AreaMin, AreaMax);
        listAgents = agents.listAgents;
        listAgentsObj = agents.listAgentsObj;
        //listAgentsRandom = listAgents.ToList();
        

        //foreach (var p in geometryCoordinates)
        //{
        //    Instantiate(agent, p, Quaternion.identity);
        //}

        Destroy(GEOInstance);

        //StartCoroutine(SequentialAssembly(SeekAssignedPosition));
        StartCoroutine("SequentialReconfiguration");

        //Task.Run(reconfiguration); <- Implement

    }

    // Update is called once per frame
    void Update()
    { 
        
    }


    // RandomShuffle: Fisher-Yates Shuffle algorithm; shuffles a list to randomly organize the its elements
    void RandomShuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = i + Random.Range(0, list.Count - i);

            T temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }


    // PointsFromGeometry: Calculates the target points for the Self-Assembly Algorithm from an 3D geometric input
    public IEnumerable<Vector3> PointsFromGeometry(GameObject geometry)
    {
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
                        yield return point;
                }
            }
        }
    }


    bool InsideCollider(Vector3 point, Vector3 rayPosition)
    {
        //Physics.queriesHitBackfaces = true;  // IMPLEMENT

        int count = 0;
        RaycastHit hit;

        Vector3 rayStart = rayPosition;
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


    // SequentialReconfiguration: 
    IEnumerator SequentialReconfiguration()
    {
        List<ReconfigurationRules.Rules> rules;
        rules = reconfiguration.RuleList();

        while (true)
        {
            // Instead of shuffling the list and running through the entire building; I can randomly choose an agent and repeat 
            // (same agent has a change to be chosen twice in a row);
            RandomShuffle(listAgents);  

            foreach (Agent agent in listAgents)  // asynchronous calculations for each agent require a delay between sequential evaluation
            {
                RandomShuffle(rules);

                reconfiguration.Successful = false;

                foreach (var rule in rules)
                {
                    if (reconfiguration.Successful == false)
                        StartCoroutine(rule(grid.Cells, agent.Location, agent));
                    else
                        break;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    








// SelfAssembly delegate
public delegate IEnumerator SelfAssembly(GameObject agent, Vector3 vector);  // or Func<GameObject, Vector3, IEnumerator> Name

    // SequentialAssembly: Agents move one by one to form an image; next agent starts a given time after the first one, forming a queue  
    IEnumerator SequentialAssembly(SelfAssembly SelfAssemblyMethod)
    {
        for (int i = 0; i < listAgentsObj.Count; i++)
        {
            StartCoroutine(SelfAssemblyMethod(listAgentsObj[i], geometryCoordinates[i]));
            yield return new WaitForSeconds(1.4f);
        }
    }

    
    /// 
    /// REDO !!!!!!!!!!!!!!!!!!!!!!!!
    /// 

    // SeekAssignedPosition: Agents seek their own assigned position from a list of positions
    IEnumerator SeekAssignedPosition(GameObject agent, Vector3 desiredPosition)
    {
        while (true)
        {
            Vector3 agentPosition = agent.transform.position;
            Vector3 seekDirection = (desiredPosition - agentPosition).normalized * velocity;
            Vector3 newPosition = agentPosition + seekDirection;
            List<Vector3> agentPositions = listAgentsObj.Select(a => a.transform.position).ToList();

            Vector3 closestAgent = CM.ClosestAgent(agentPosition, agentPositions);
            Vector3 awayFromAgent = (agentPosition - closestAgent).normalized * velocity;

            if (Vector3.Distance(agentPosition, desiredPosition) <= minDistance)
            {
                agent.transform.Translate(desiredPosition - agentPosition);
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
        List<Vector3> agentPositions = listAgentsObj.Select(a => a.transform.position).ToList();

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
