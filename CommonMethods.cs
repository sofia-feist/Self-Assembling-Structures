using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class CommonMethods 
{


    // RandomShuffle: Fisher-Yates Shuffle algorithm; shuffles a list to randomly organize the its elements
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


    // InsideCollider: Checks if a given point is inside a collider
    public bool InsideCollider(Vector3 point, Vector3 rayPosition)
    {
        Physics.queriesHitBackfaces = true;    // IMPLEMENT!!!

        int count = 0;
        RaycastHit hit;

        Vector3 rayEnd = rayPosition;
        Vector3 direction = rayEnd - point;
        Vector3 hitPoint = point;//hitPoint = rayStart;

        //while (Physics.Raycast(hitPoint, direction, out hit, direction.magnitude) && count < 100)
        //{
        //    hitPoint = hit.point + (direction.normalized / 10.0f);
        //    count++;
        //}

        //hitPoint = point;

        while (Physics.Raycast(hitPoint, direction, out hit, direction.magnitude) && count < 100)
        {
            hitPoint = hit.point + (direction.normalized / 1000.0f);
            count++;
        }

        // Check how many intersections there are
        if (count % 2 == 0)
            return false;
        else   // if (count % 2 == 1)
            return true;
    }










    ////////////////////////   VECTORS  ////////////////////////

    // AbsVector: Returns a vector whose elements are the absolute values of each of the specified vector's elements.
    public Vector3 AbsVector(Vector3 vector)
    {
        vector.x = Abs(vector.x);
        vector.y = Abs(vector.y);
        vector.z = Abs(vector.z);
        return vector;
    }


    // PerpendicularVector: Returns a vector perpendicular to the given vector.
    public Vector3 PerpendicularVector(Vector3 vector, float direction)     // direction: either 1 or -1.
    {
        float coordX = vector.z * (- direction);
        float coordZ = vector.x * direction;
        return new Vector3(coordX, 0, coordZ);
    }


    // ConstrainedRandomVector: Random Vector constrained to a given amplitude (-amplitude -> +amplitude)
    public Vector3 ConstrainedRandomVector(Vector3 steeringDirection, float velocity, float amplitude)
    {
        float angle = Atan2(steeringDirection.z, steeringDirection.x);
        float range = Random.Range(angle - amplitude, angle + amplitude);
        Vector3 vector = new Vector3(Cos(range), 0, Sin(range));

        return vector.normalized * velocity;
    }


    // PerlinVector: Random Vector using Perlin Noise
    public Vector3 PerlinVector(Vector3 steeringDirection, Vector3 position, float velocity)
    {
        float noise = PerlinNoise(position.x, position.z);
        float angle = Atan2(steeringDirection.z, steeringDirection.x);

        float range;
        if (noise < 0.5)
            range = angle - noise;
        else
            range = angle + noise * 0.5f;

        Vector3 vector = new Vector3(Cos(range), 0, Sin(range));
        return vector.normalized * velocity;
    }


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