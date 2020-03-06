using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReconfigurationRules
{
    CellGrid grid = new CellGrid(_3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMax);

    public bool Successful = false;
    float speed = 0;



    public delegate IEnumerator Rules(Cell[,,] cells, Cell currentCell, Agent agent);

    public List<Rules> RuleList()
    {
        List<Rules> rules = new List<Rules>();
        // Linear Mouvements
        rules.Add(MoveNorth);
        rules.Add(MoveSouth);
        rules.Add(MoveEast);
        rules.Add(MoveWest);
        rules.Add(MoveUp);
        rules.Add(MoveDown);
        // Convex Mouvements
        rules.Add(MoveUpNorth);
        rules.Add(MoveNorthDown);

        return rules;
    }






    ////////////////////////////   RULES  ////////////////////////////
    
    // CheckDisconnections: checks neighbourhood to avoid disconnections in the structure
    public bool PotentialDisconnection(Cell currentCell)
    {
        var Neighbours = currentCell.GetFaceNeighbours().Where(n => n.Alive);

        if (Neighbours.Count() == 2)  // If current cell only has two face neighbour, DO NOT MOVE 
            return true;

        foreach (var neighbour in Neighbours)
        {
            int nNeighbours = neighbour.GetFaceNeighbours().Count(n => n.Alive);

            if (nNeighbours == 1)   // If neighbour only has one face neighbour (aka current cell), DO NOT MOVE
                return true;
        }
       
        return false;
    }


    // LINEAR TRANSITIONS
    public IEnumerator MoveNorth(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var middleNorth = grid.MN(cells, currentCell);
        var middleSouth = grid.MS(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (middleNorth?.Alive == false && (middleSouth == null || middleSouth.Alive == false))
            {
                if (grid.MNE(cells, currentCell)?.Alive == true && grid.ME(cells, currentCell).Alive == true ||   // Travel through the East side
                    grid.MNW(cells, currentCell)?.Alive == true && grid.MW(cells, currentCell).Alive == true ||   // Travel through the West side
                    grid.BN(cells, currentCell)?.Alive == true && grid.B(cells, currentCell).Alive == true ||     // Travel through the Bottom side
                    grid.UN(cells, currentCell)?.Alive == true && grid.U(cells, currentCell).Alive == true)       // Travel through the Upper side
                {
                    agent.Obj.transform.Translate(Vector3.forward);
                    agent.Location = middleNorth;

                    currentCell.Alive = false;
                    middleNorth.Alive = true;

                    Successful = true;

                    yield return new WaitForSeconds(speed);
                }
            }
        }
    }


    public IEnumerator MoveSouth(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var middleSouth = grid.MS(cells, currentCell);
        var middleSouthEast = grid.MSE(cells, currentCell);
        var middleSouthWest = grid.MSW(cells, currentCell);
        var upperSouth = grid.US(cells, currentCell);
        var bottomSouth = grid.BS(cells, currentCell);
        var middleNorth = grid.MS(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (middleSouth?.Alive == false && (middleNorth == null || middleNorth.Alive == false))
            {
                if (middleSouthEast?.Alive == true ||
                    middleSouthWest?.Alive == true ||
                    bottomSouth?.Alive == true ||
                    upperSouth?.Alive == true)
                {
                    agent.Obj.transform.Translate(Vector3.back);
                    agent.Location = middleSouth;

                    currentCell.Alive = false;
                    middleSouth.Alive = true;

                    Successful = true;

                    yield return new WaitForSeconds(speed);
                }
            }
        }
    }


    public IEnumerator MoveEast(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var middleEast = grid.ME(cells, currentCell);
        var middleSouthEast = grid.MSE(cells, currentCell);
        var middleNorthEast = grid.MNE(cells, currentCell);
        var upperEast = grid.UE(cells, currentCell);
        var bottomEast = grid.BE(cells, currentCell);
        var middleWest = grid.MW(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (middleEast?.Alive == false && (middleWest == null || middleWest.Alive == false))
            {
                if (middleSouthEast?.Alive == true ||
                    middleNorthEast?.Alive == true ||
                    bottomEast?.Alive == true ||
                    upperEast?.Alive == true)
                {
                    agent.Obj.transform.Translate(Vector3.right);
                    agent.Location = middleEast;

                    currentCell.Alive = false;
                    middleEast.Alive = true;

                    Successful = true;

                    yield return new WaitForSeconds(speed);
                }
            }
        }
    }


    public IEnumerator MoveWest(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var middleWest = grid.MW(cells, currentCell);
        var middleSouthWest = grid.MSW(cells, currentCell);
        var middleNorthWest = grid.MNW(cells, currentCell);
        var upperWest = grid.UW(cells, currentCell);
        var bottomWest = grid.BW(cells, currentCell);
        var middleEast = grid.ME(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (middleWest?.Alive == false && (middleEast == null || middleEast.Alive == false))
            {
                if (middleSouthWest?.Alive == true ||
                    middleNorthWest?.Alive == true ||
                    bottomWest?.Alive == true ||
                    upperWest?.Alive == true)
                {
                    agent.Obj.transform.Translate(Vector3.left);
                    agent.Location = middleWest;

                    currentCell.Alive = false;
                    middleWest.Alive = true;

                    Successful = true;

                    yield return new WaitForSeconds(speed);
                }
            }
        }
    }


    public IEnumerator MoveUp(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var up = grid.U(cells, currentCell);
        var upperEast = grid.UE(cells, currentCell);
        var upperWest = grid.UW(cells, currentCell);
        var upperNorth = grid.UN(cells, currentCell);
        var upperSouth = grid.US(cells, currentCell);
        var bottom = grid.B(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (up?.Alive == false && (bottom == null || bottom.Alive == false))
            {
                if (upperEast?.Alive == true ||
                    upperWest?.Alive == true ||
                    upperNorth?.Alive == true ||
                    upperSouth?.Alive == true)
                {
                    agent.Obj.transform.Translate(Vector3.up);
                    agent.Location = up;

                    currentCell.Alive = false;
                    up.Alive = true;

                    Successful = true;

                    yield return new WaitForSeconds(speed);
                }
            }
        }
    }


    public IEnumerator MoveDown(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var bottom = grid.B(cells, currentCell);
        var upperEast = grid.UE(cells, currentCell);
        var upperWest = grid.UW(cells, currentCell);
        var upperNorth = grid.UN(cells, currentCell);
        var upperSouth = grid.US(cells, currentCell);
        var up = grid.U(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (bottom?.Alive == false && (up == null || up.Alive == false))
            {
                if (upperEast?.Alive == true ||
                    upperWest?.Alive == true ||
                    upperNorth?.Alive == true ||
                    upperSouth?.Alive == true)
                {
                    agent.Obj.transform.Translate(Vector3.down);
                    agent.Location = bottom;

                    currentCell.Alive = false;
                    bottom.Alive = true;

                    Successful = true;

                    yield return new WaitForSeconds(speed);
                }
            }
        }
    }



    // CONVEX TRANSITIONS
    public IEnumerator MoveUpNorth(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var upperNorth = grid.UN(cells, currentCell);
        var middleNorth = grid.MN(cells, currentCell);
        var middleSouth = grid.MS(cells, currentCell);
        var up = grid.U(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (upperNorth?.Alive == false && up.Alive == false && middleNorth.Alive == true &&
            (middleSouth == null || middleSouth.Alive == false))
            {
                agent.Obj.transform.Translate(Vector3.up);

                currentCell.Alive = false;
                up.Alive = true;

                yield return new WaitForSeconds(speed);


                agent.Obj.transform.Translate(Vector3.forward);

                up.Alive = false;
                upperNorth.Alive = true;

                agent.Location = upperNorth;
                Successful = true;

                yield return new WaitForSeconds(speed);
            }
        }
    }


    public IEnumerator MoveNorthDown(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var bottomNorth = grid.BN(cells, currentCell);
        var middleNorth = grid.MN(cells, currentCell);
        var middleSouth = grid.MS(cells, currentCell);
        var bottom = grid.B(cells, currentCell);

        if (PotentialDisconnection(currentCell) == false)
        {
            if (bottomNorth?.Alive == false && bottom.Alive == true && middleNorth.Alive == false &&
            (middleSouth == null || middleSouth.Alive == false))
            {
                agent.Obj.transform.Translate(Vector3.forward);

                currentCell.Alive = false;
                middleNorth.Alive = true;

                yield return new WaitForSeconds(speed);


                agent.Obj.transform.Translate(Vector3.down);

                middleNorth.Alive = false;
                bottomNorth.Alive = true;

                agent.Location = bottomNorth;
                Successful = true;

                yield return new WaitForSeconds(speed);
            }
        }
    }


    // CONCAVE TRANSITIONS





    ////////////////////////////   MOVEMENTS  ////////////////////////////

    // LINEAR MOVEMENT
    // LinearMovement: Executes a Linear Mouvement and updates the Cell grid
    public IEnumerator LinearMovement(Cell currentCell, Cell targetCell, Vector3 direction, Agent agent, int CellSize)
    {
        agent.Obj.transform.Translate(direction * CellSize);
        agent.Location = targetCell;

        currentCell.Alive = false;
        targetCell.Alive = true;     // Successful?

        yield return new WaitForSeconds(speed);
    }


    // ConvexConcaveMovement: Executes a Concave OR Convex Movement and updates the Cell grid
    public IEnumerator ConvexConcaveMovement(Cell currentCell, Cell transitionCell, Cell targetCell, 
                                             Vector3 direction1, Vector3 direction2, Agent agent, int CellSize)
    {
        agent.Obj.transform.Translate(direction1 * CellSize);
        agent.Location = transitionCell;

        currentCell.Alive = false;
        transitionCell.Alive = true;

        yield return new WaitForSeconds(speed);


        agent.Obj.transform.Translate(direction2 * CellSize);
        agent.Location = targetCell;

        transitionCell.Alive = false;    // Successful?
        targetCell.Alive = true;

        yield return new WaitForSeconds(speed);
    }


}
