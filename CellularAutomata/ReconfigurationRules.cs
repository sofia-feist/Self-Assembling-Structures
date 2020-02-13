using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconfigurationRules
{
    CellGrid grid = new CellGrid(_3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMax);


    public bool Successful = false;





    public delegate IEnumerator Rules(Cell[,,] cells, Cell currentCell, Agent agent);

    public List<Rules> RuleList()
    {
        List<Rules> rules = new List<Rules>();
        // Linear
        rules.Add(MoveNorth);
        rules.Add(MoveSouth);
        rules.Add(MoveEast);
        rules.Add(MoveWest);
        rules.Add(MoveUp);
        rules.Add(MoveDown);
        // Convex
        rules.Add(MoveUpNorth);
        rules.Add(MoveNorthDown);

        return rules;
    }






    ////////////////////////////   RULES  ////////////////////////////
    
    // LINEAR TRANSITIONS
    public IEnumerator MoveNorth(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var middleNorth = grid.MN(cells, currentCell);
        var middleNorthEast = grid.MNE(cells, currentCell);
        var middleNorthWest = grid.MNW(cells, currentCell);
        var upperNorth = grid.UN(cells, currentCell);
        var bottomNorth = grid.BN(cells, currentCell);
        var middleSouth = grid.MS(cells, currentCell);

        //bool? boole = redrawTask?.IsComplete;
        //if (redrawTask?.IsCompleted != false)    because redraw can be null, true or false

        //if (middleNorth?.Alive == false && (middleSouth == null || middleSouth.Alive == false))
        if (middleNorth != null && middleNorth.Alive == false && (middleSouth == null || middleSouth.Alive == false))
        {
            if (middleNorthEast != null && middleNorthEast.Alive == true ||
                middleNorthWest != null && middleNorthWest.Alive == true ||
                bottomNorth != null && bottomNorth.Alive == true ||
                upperNorth != null && upperNorth.Alive == true)
            {
                agent.Obj.transform.Translate(Vector3.forward);
                agent.Location = middleNorth;

                currentCell.Alive = false;
                middleNorth.Alive = true;

                Successful = true;

                yield return null;
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

        if (middleSouth != null && middleSouth.Alive == false && (middleNorth == null || middleNorth.Alive == false))
        {
            if (middleSouthEast != null && middleSouthEast.Alive == true ||
                middleSouthWest != null && middleSouthWest.Alive == true ||
                bottomSouth != null && bottomSouth.Alive == true ||
                upperSouth != null && upperSouth.Alive == true)
            {
                agent.Obj.transform.Translate(Vector3.back);
                agent.Location = middleSouth;

                currentCell.Alive = false;
                middleSouth.Alive = true;

                Successful = true;

                yield return null;
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

        if (middleEast != null && middleEast.Alive == false && (middleWest == null || middleWest.Alive == false))
        {
            if (middleSouthEast != null && middleSouthEast.Alive == true ||
                middleNorthEast != null && middleNorthEast.Alive == true ||
                bottomEast != null && bottomEast.Alive == true ||
                upperEast != null && upperEast.Alive == true)
            {
                agent.Obj.transform.Translate(Vector3.right);
                agent.Location = middleEast;

                currentCell.Alive = false;
                middleEast.Alive = true;

                Successful = true;

                yield return null;
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

        if (middleWest != null && middleWest.Alive == false && (middleEast == null || middleEast.Alive == false))
        {
            if (middleSouthWest != null && middleSouthWest.Alive == true ||
                middleNorthWest != null && middleNorthWest.Alive == true ||
                bottomWest != null && bottomWest.Alive == true ||
                upperWest != null && upperWest.Alive == true)
            {
                agent.Obj.transform.Translate(Vector3.left);
                agent.Location = middleWest;

                currentCell.Alive = false;
                middleWest.Alive = true;

                Successful = true;

                yield return null;
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
        
        if (up != null && up.Alive == false && (bottom == null || bottom.Alive == false))
        {
            if (upperEast != null && upperEast.Alive == true ||
                upperWest != null && upperWest.Alive == true ||
                upperNorth != null && upperNorth.Alive == true ||
                upperSouth != null && upperSouth.Alive == true)
            {
                agent.Obj.transform.Translate(Vector3.up);
                agent.Location = up;

                currentCell.Alive = false;
                up.Alive = true;

                Successful = true;

                yield return null;
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

        if (bottom != null && bottom.Alive == false && (up == null || up.Alive == false))
        {
            if (upperEast != null && upperEast.Alive == true ||
                upperWest != null && upperWest.Alive == true ||
                upperNorth != null && upperNorth.Alive == true ||
                upperSouth != null && upperSouth.Alive == true)
            {
                agent.Obj.transform.Translate(Vector3.down);
                agent.Location = bottom;

                currentCell.Alive = false;
                bottom.Alive = true;

                Successful = true;

                yield return null;
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

        if (upperNorth != null && upperNorth.Alive == false && up.Alive == false && middleNorth.Alive == true && 
            (middleSouth == null || middleSouth.Alive == false))
        {
            agent.Obj.transform.Translate(Vector3.up);
            agent.Obj.transform.Translate(Vector3.forward);
            agent.Location = upperNorth;

            currentCell.Alive = false;
            upperNorth.Alive = true;

            Successful = true;

            yield return null;
        }
    }


    public IEnumerator MoveNorthDown(Cell[,,] cells, Cell currentCell, Agent agent)
    {
        var bottomNorth = grid.BN(cells, currentCell);
        var middleNorth = grid.MN(cells, currentCell);
        var middleSouth = grid.MS(cells, currentCell);
        var bottom = grid.B(cells, currentCell);

        if (bottomNorth != null && bottomNorth.Alive == false && bottom.Alive == true && middleNorth.Alive == false &&
            (middleSouth == null || middleSouth.Alive == false)
            )
        {
            agent.Obj.transform.Translate(Vector3.forward);  //couroutines to see both mouvements?
            agent.Obj.transform.Translate(Vector3.down);
            agent.Location = bottomNorth;

            currentCell.Alive = false;
            bottomNorth.Alive = true;

            Successful = true;

            yield return null;
        }
    }

}
