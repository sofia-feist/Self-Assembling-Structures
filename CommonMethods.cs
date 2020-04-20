using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public class CommonMethods 
{
    
    ////////////////////////   RANDOM SHUFFLE  ////////////////////////


    // RandomShuffle(list): Fisher-Yates Shuffle algorithm; shuffles a list to randomly organize the its elements
    public void RandomShuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = i + Random.Range(0, list.Count - i);

            T temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }

    // RandomShuffle(array): Fisher-Yates Shuffle algorithm; shuffles an array to randomly organize the its elements
    public void RandomShuffle<T>(T[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            int j = i + Random.Range(0, list.Length - i);

            T temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }





    ////////////////////////   AREA CHECKS  ////////////////////////


    // OutsideBoundaries: Checks if a given vector is located outside of the given boundaries (square)
    public bool OutsideBoundaries(Vector3 position, int AreaMin, int AreaMax)
    {
        if (position.x > AreaMax ||
            position.y > AreaMax ||
            position.z > AreaMax ||
            position.x < AreaMin ||
            position.y < AreaMin ||
            position.z < AreaMin)
            return true;
        else
            return false;
    }




    ////////////////////////   GOAL SHAPE SETUP  ////////////////////////


    // PointsFromGeometry: Calculates the target points for the Self-Assembly Algorithm from an 3D geometric input
    public IEnumerable<Vector3> PointsFromGeometry(GameObject geometry)
    {
        SetupColliders(geometry);

        Bounds size = GeometrySize(geometry);
        Vector3 min = size.min;
        Vector3 max = size.max;

        for (float y = min.y + 0.5f; y < max.y; y++)
        {
            for (float z = min.z + 0.5f; z < max.z; z++)
            {
                for (float x = min.x + 0.5f; x < max.x; x++)
                {
                    Vector3 point = new Vector3(x, y, z);

                    if (InsideCollider(point, Vector3.down))                  // CONTROL Nº OF POINTS (aka GOAL CELLS) ACC TO Nº of AGENTS?  -  MORE AGENTS -> MORE RESOLUTION
                        yield return point;
                }
            }
        }
    }


    // SetupColliders: Adds Mesh Colliders to geometry and all children with a Mesh Filter
    public void SetupColliders(GameObject geometry)
    {
        // Parent Geometry
        MeshFilter parentMeshFilter = geometry.GetComponent<MeshFilter>();
        MeshCollider parentCollider = geometry.GetComponent<MeshCollider>();

        if (parentMeshFilter != null && parentCollider == null)
        {
            parentCollider = geometry.AddComponent<MeshCollider>();
            Mesh parentMesh = parentMeshFilter.sharedMesh;
            parentCollider.sharedMesh = parentMesh;
        }
        else if (parentMeshFilter != null && parentCollider != null)
        {
            Mesh parentMesh = parentMeshFilter.sharedMesh;
            parentCollider.sharedMesh = parentMesh;
        }


        // Children
        int nChildren = geometry.transform.childCount;

        if (nChildren > 0)
        {
            foreach (Transform childObject in geometry.transform)
            {
                MeshFilter childMeshFilter = childObject.GetComponent<MeshFilter>();
                MeshCollider childMeshCollider = childObject.GetComponent<MeshCollider>();

                if (childMeshFilter != null && childMeshCollider == null)
                {
                    childMeshCollider = childObject.gameObject.AddComponent<MeshCollider>();
                    Mesh mesh = childMeshFilter.sharedMesh;
                    childMeshCollider.sharedMesh = mesh;
                }
                else if (childMeshFilter != null && childMeshCollider != null)
                {
                    Mesh mesh = childMeshFilter.sharedMesh;
                    childMeshCollider.sharedMesh = mesh;
                }
            }
        }
    }


    // GeometrySize: Returns the (largest) size of a Game Object using Collider Bounds
    public Bounds GeometrySize(GameObject geometry)
    {
        int nChildren = geometry.transform.childCount;

        if (nChildren == 0)
        {
            MeshCollider collider = geometry.GetComponent<MeshCollider>();
            return collider.bounds;
        }
        else
        {
            List<Bounds> Sizes = new List<Bounds>();

            // Parent  (Parent geometry of Game Objects isn't always the main geometry)
            MeshCollider parentcollider = geometry.GetComponent<MeshCollider>(); 
            if (parentcollider != null)
                Sizes.Add(parentcollider.bounds);
            

            // Children
            foreach (Transform childObject in geometry.transform)
            {
                MeshCollider childcollider = geometry.GetComponent<MeshCollider>();
                if (childcollider != null) 
                    Sizes.Add(childcollider.bounds);
            }
            
            return Sizes.OrderByDescending(s => s.size.magnitude).ElementAt(0);
        }
    }


    // InsideCollider: Checks if a given point is inside a collider
    bool InsideCollider(Vector3 point, Vector3 direction)
    {
        Physics.queriesHitBackfaces = true;

        int count = 0;
        RaycastHit hit;
        Vector3 hitPoint = point;

        while (Physics.Raycast(hitPoint, direction, out hit, float.PositiveInfinity))   //RayCastAll has a bug and does not detect all colliders
        {
            hitPoint = hit.point + (direction.normalized / 100.0f);
            count++;
        }

        // Check how many intersections there are
        if (count % 2 == 0)
            return false;
        else   // if (count % 2 == 1)
            return true;
    }





    ////////////////////////   VECTORS  ////////////////////////


    // RandomVector: Random Direction Vector in Polar coordinates: infinite possible directions (0º -> 360º float)
    public Vector3 RandomVector(float velocity)
    {
        float pi = PI;
        float angle = Random.Range(-pi, pi);
        Vector3 vector = new Vector3(Cos(angle), 0, Sin(angle));

        return vector.normalized * velocity;
    }


    // RandomVectorXZ: Random Direction Vector in Cartesian coordinates: 4 possible directions (+x, -x, +z, -z)
    public Vector3 RandomVectorXZ(float velocity)
    {
        Vector3 vector;
        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                vector = new Vector3(velocity, 0, 0);
                break;
            case 1:
                vector = new Vector3(-velocity, 0, 0);
                break;
            case 2:
                vector = new Vector3(0, 0, velocity);
                break;
            default:
                vector = new Vector3(0, 0, -velocity);
                break;
        }
        return vector;
    }


    // Random Position (= Vector), with the X, Y and Z coordinates placed randomly within the given boundaries
    public Vector3Int RandomPosition(int AreaMin, int AreaMax)
    {
        return new Vector3Int(Random.Range(AreaMin, AreaMax), Random.Range(AreaMin, AreaMax), Random.Range(AreaMin, AreaMax));
    }
}