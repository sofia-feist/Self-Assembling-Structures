using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum Action
{
    NoAction  = -1,

    // Linear Actions
    North     = 0,
    South     = 1,
    East      = 2,
    West      = 3,
    Up        = 4,
    Down      = 5,

    // Convex/Concave Actions
    UpNorth     = 6,
    DownNorth   = 7,
    NorthUp     = 8,
    NorthDown   = 9,

    UpSouth     = 10,
    DownSouth   = 11,
    SouthUp     = 12,
    SouthDown   = 13,

    UpEast      = 14,
    DownEast    = 15,
    EastUp      = 16,
    EastDown    = 17,

    UpWest      = 18,
    DownWest    = 19,
    WestUp      = 20,
    WestDown    = 21,

    NorthEast   = 22,
    NorthWest   = 23,
    EastNorth   = 24,
    WestNorth   = 25,

    SouthEast   = 26,
    SouthWest   = 27,
    EastSouth   = 28,
    WestSouth   = 29
}



public class ReconfigurationRules
{
    CellGrid grid;
    CommonMethods CM = new CommonMethods();
    
    float speed;


    // Constructor
    public ReconfigurationRules(CellGrid _grid, float _speed)
    {
        grid = _grid;
        speed = _speed;
    }





    public List<Func<Cell, Action>> RuleList()
    {
        var rules = new List<Func<Cell, Action>>();

        // Linear Mouvements
        rules.Add(North);
        rules.Add(South);
        rules.Add(East);
        rules.Add(West);
        rules.Add(Up);
        rules.Add(Down);

        // Convex/Concave Mouvements
        rules.Add(UpNorth);
        rules.Add(DownNorth);
        rules.Add(NorthUp);
        rules.Add(NorthDown);

        rules.Add(UpSouth);
        rules.Add(DownSouth);
        rules.Add(SouthUp);
        rules.Add(SouthDown);

        rules.Add(UpEast);
        rules.Add(DownEast);
        rules.Add(EastUp);
        rules.Add(EastDown);

        rules.Add(UpWest);
        rules.Add(DownWest);
        rules.Add(WestUp);
        rules.Add(WestDown);

        rules.Add(NorthEast);
        rules.Add(NorthWest);
        rules.Add(EastNorth);
        rules.Add(WestNorth);

        rules.Add(SouthEast);
        rules.Add(SouthWest);
        rules.Add(EastSouth);
        rules.Add(WestSouth);

        return rules;
    }




    public Action CheckRules(Agent agent)
    {
        Action nextAction = Action.NoAction;
        CM.RandomShuffle(RuleList());  //Is this Working properly?

        foreach (var rule in RuleList())  
        {
            Action currentAction = rule(agent.Location);

            if (currentAction != Action.NoAction)
            {
                nextAction = currentAction;
                break;
            }
        }
        return nextAction;
    }


    public void ExecuteAction(MonoBehaviour mono, Action action, Agent agent)
    {
        Cell currentCell = agent.Location;

        switch (action)
        {
            // Linear Mouvements
            case Action.North:
                mono.StartCoroutine(
                    LinearMove(currentCell, grid.MN(currentCell), Vector3.forward, agent, currentCell.CellSize));
                break;
            case Action.South:
                mono.StartCoroutine(
                    LinearMove(currentCell, grid.MS(currentCell), Vector3.back, agent, currentCell.CellSize));
                break;
            case Action.East:
                mono.StartCoroutine(
                    LinearMove(currentCell, grid.ME(currentCell), Vector3.right, agent, currentCell.CellSize));
                break;
            case Action.West:
                mono.StartCoroutine(
                    LinearMove(currentCell, grid.MW(currentCell), Vector3.left, agent, currentCell.CellSize));
                break;
            case Action.Up:
                mono.StartCoroutine(
                    LinearMove(currentCell, grid.U(currentCell), Vector3.up, agent, currentCell.CellSize));
                break;
            case Action.Down:
                mono.StartCoroutine(
                    LinearMove(currentCell, grid.B(currentCell), Vector3.down, agent, currentCell.CellSize));
                break;


            // Convex/Concave Mouvements
            case Action.UpNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.U(currentCell), grid.UN(currentCell), Vector3.up, Vector3.forward, agent, currentCell.CellSize));
                break;
            case Action.DownNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.B(currentCell), grid.BN(currentCell), Vector3.down, Vector3.forward, agent, currentCell.CellSize));
                break;
            case Action.NorthUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MN(currentCell), grid.UN(currentCell), Vector3.forward, Vector3.up, agent, currentCell.CellSize));
                break;
            case Action.NorthDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MN(currentCell), grid.BN(currentCell), Vector3.forward, Vector3.down, agent, currentCell.CellSize));
                break;


            case Action.UpSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.U(currentCell), grid.US(currentCell), Vector3.up, Vector3.back, agent, currentCell.CellSize));
                break;
            case Action.DownSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.B(currentCell), grid.BS(currentCell), Vector3.down, Vector3.back, agent, currentCell.CellSize));
                break;
            case Action.SouthUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MS(currentCell), grid.US(currentCell), Vector3.back, Vector3.up, agent, currentCell.CellSize));
                break;
            case Action.SouthDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MS(currentCell), grid.BS(currentCell), Vector3.back, Vector3.down, agent, currentCell.CellSize));
                break;


            case Action.UpEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.U(currentCell), grid.UE(currentCell), Vector3.up, Vector3.right, agent, currentCell.CellSize));
                break;
            case Action.DownEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.B(currentCell), grid.BE(currentCell), Vector3.down, Vector3.right, agent, currentCell.CellSize));
                break;
            case Action.EastUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.ME(currentCell), grid.UE(currentCell), Vector3.right, Vector3.up, agent, currentCell.CellSize));
                break;
            case Action.EastDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.ME(currentCell), grid.BE(currentCell), Vector3.right, Vector3.down, agent, currentCell.CellSize));
                break;


            case Action.UpWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.U(currentCell), grid.UW(currentCell), Vector3.up, Vector3.left, agent, currentCell.CellSize));
                break;
            case Action.DownWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.B(currentCell), grid.BW(currentCell), Vector3.down, Vector3.left, agent, currentCell.CellSize));
                break;
            case Action.WestUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MW(currentCell), grid.UW(currentCell), Vector3.left, Vector3.up, agent, currentCell.CellSize));
                break;
            case Action.WestDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MW(currentCell), grid.BW(currentCell), Vector3.left, Vector3.down, agent, currentCell.CellSize));
                break;


            case Action.NorthEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MN(currentCell), grid.MNE(currentCell), Vector3.forward, Vector3.right, agent, currentCell.CellSize));
                break;
            case Action.NorthWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MN(currentCell), grid.MNW(currentCell), Vector3.forward, Vector3.left, agent, currentCell.CellSize));
                break;
            case Action.EastNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.ME(currentCell), grid.MNE(currentCell), Vector3.right, Vector3.forward, agent, currentCell.CellSize));
                break;
            case Action.WestNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MW(currentCell), grid.MNW(currentCell), Vector3.left, Vector3.forward, agent, currentCell.CellSize));
                break;


            case Action.SouthEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MS(currentCell), grid.MSE(currentCell), Vector3.back, Vector3.right, agent, currentCell.CellSize));
                break;
            case Action.SouthWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MS(currentCell), grid.MSW(currentCell), Vector3.back, Vector3.left, agent, currentCell.CellSize));
                break;
            case Action.EastSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.ME(currentCell), grid.MSE(currentCell), Vector3.right, Vector3.back, agent, currentCell.CellSize));
                break;
            case Action.WestSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, grid.MW(currentCell), grid.MSW(currentCell), Vector3.left, Vector3.back, agent, currentCell.CellSize));
                break;
        }
    }





    ////////////////////////////   RULES  ////////////////////////////

    // CheckDisconnections: checks a given cell's neighbourhood to predict disconnections in the structure
    public bool PotentialDisconnections(Cell currentCell)
    {
        var Neighbours = currentCell.GetFaceNeighbours().Where(n => n.Alive);

        foreach (var neighbour in Neighbours)
        {
            int nNeighbours = neighbour.GetFaceNeighbours().Count(n => n.Alive);

            if (nNeighbours == 1 ||        // If neighbour only has one face neighbour (aka current cell), DO NOT MOVE
                Neighbours.Count() == 2)   // CORRECT
                return true;
        }    // CHECK ALSO: Neighbours.Count() == 2 (opposite faces) or Neighbours.Count() == 2 (adjacent faces -> check connector block)

        return false;
    }


    // TARGET/TRANSITION CELLS CHECK
    // TargetCellUnoccupied: Check if a given target cell an agent is trying to move into is unoccupied
    public bool TargetCellUnoccupied(Cell TargetCell)
    {
        return (TargetCell?.Alive == false) ? true : false;
    }


    // TransitionCellUnoccupied: Check if a given transition cell an agent has to pass through to get to the target cell is unoccupied
    public bool TransitionCellUnoccupied(Cell TransitionCell)
    {
        return (TransitionCell?.Alive == false) ? true : false;
    }



    // CONNECTOR CELLS CHECKS
    // ConnectorCellsExist_Linear: Checks if the connector cells through which the agent is and will connect to exist; for Linear Mouvement
    public bool ConnectorCellsExist_Linear(Cell InitialConnectorCell, Cell GoalConnectorCell)
    {
        return (InitialConnectorCell?.Alive == true && GoalConnectorCell?.Alive == true) ? true : false;
    }

    // ConnectorCellExists_Convex: Checks if the connector cell which the agent will connect around exists; for Convex Mouvement
    public bool ConnectorCellExists_Convex(Cell ConvexConnectorCell)
    {
        return ConvexConnectorCell?.Alive == true ? true : false;
    }

    // ConnectorCellsExist_Concave: Checks if the connector cells through which the agent is and will connect to exist; for Concave Mouvement
    public bool ConnectorCellsExist_Concave(Cell ConnectCell1, Cell ConnectCell2, Cell ConnectCell3, Cell ConnectCell4)
    {
        if (ConnectCell1?.Alive == true &&
            ConnectCell2?.Alive == true &&
            ConnectCell3?.Alive == true &&
            ConnectCell4?.Alive == true)
            return true;
        else
            return false;
    }







    ////////////////////////////   RULES  ////////////////////////////

    // LINEAR TRANSITIONS
    public Action North(Cell currentCell)
    {
        var middleNorth = grid.MN(currentCell);
        var middleSouth = grid.MS(currentCell);

        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleNorthEast = grid.MNE(currentCell);
        var middleNorthWest = grid.MNW(currentCell);
        var bottomNorth = grid.BN(currentCell);
        var upNorth = grid.UN(currentCell);

        if (TargetCellUnoccupied(middleNorth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellsExist_Linear(middleEast, middleNorthEast) ||      // Travel through the East side
             ConnectorCellsExist_Linear(middleWest, middleNorthWest) ||      // Travel through the West side
             ConnectorCellsExist_Linear(bottom, bottomNorth)         ||      // Travel through the Bottom side
             ConnectorCellsExist_Linear(up, upNorth))  &&                    // Travel through the Upper side
            (middleSouth == null || middleSouth.Alive == false))
        
            return Action.North;
        else
            return Action.NoAction;
    }


    public Action South(Cell currentCell)
    {
        var middleSouth = grid.MS(currentCell);
        var middleNorth = grid.MS(currentCell);

        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleSouthEast = grid.MSE(currentCell);
        var middleSouthWest = grid.MSW(currentCell);
        var upperSouth = grid.US(currentCell);
        var bottomSouth = grid.BS(currentCell);
        
        if (TargetCellUnoccupied(middleSouth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellsExist_Linear(middleEast, middleSouthEast) ||      // Travel through the East side
             ConnectorCellsExist_Linear(middleWest, middleSouthWest) ||      // Travel through the West side
             ConnectorCellsExist_Linear(bottom, bottomSouth) ||              // Travel through the Bottom side
             ConnectorCellsExist_Linear(up, upperSouth)) &&                  // Travel through the Upper side
            (middleNorth == null || middleNorth.Alive == false))

            return Action.South;
        else
            return Action.NoAction;
    }


    public Action East(Cell currentCell)
    {
        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);

        var middleSouth = grid.MS(currentCell);
        var middleNorth = grid.MN(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleSouthEast = grid.MSE(currentCell);
        var middleNorthEast = grid.MNE(currentCell);
        var upperEast = grid.UE(currentCell);
        var bottomEast = grid.BE(currentCell);

        if (TargetCellUnoccupied(middleEast) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellsExist_Linear(middleSouth, middleSouthEast) ||      // Travel through the South side
             ConnectorCellsExist_Linear(middleNorth, middleNorthEast) ||      // Travel through the North side
             ConnectorCellsExist_Linear(bottom, bottomEast) ||                // Travel through the Bottom side
             ConnectorCellsExist_Linear(up, upperEast)) &&                    // Travel through the Upper side
            (middleWest == null || middleWest.Alive == false))

            return Action.East;
        else
            return Action.NoAction;
    }


    public Action West(Cell currentCell)
    {
        var middleWest = grid.MW(currentCell);
        var middleEast = grid.ME(currentCell);

        var middleSouth = grid.MS(currentCell);
        var middleNorth = grid.MN(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleSouthWest = grid.MSW(currentCell);
        var middleNorthWest = grid.MNW(currentCell);
        var upperWest = grid.UW(currentCell);
        var bottomWest = grid.BW(currentCell);

        if (TargetCellUnoccupied(middleWest) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellsExist_Linear(middleSouth, middleSouthWest) ||      // Travel through the South side
             ConnectorCellsExist_Linear(middleNorth, middleNorthWest) ||      // Travel through the North side
             ConnectorCellsExist_Linear(bottom, bottomWest) ||                // Travel through the Bottom side
             ConnectorCellsExist_Linear(up, upperWest)) &&                    // Travel through the Upper side
            (middleEast == null || middleEast.Alive == false))

            return Action.West;
        else
            return Action.NoAction;  
    }


    public Action Up(Cell currentCell)
    {
        var up = grid.U(currentCell);
        var bottom = grid.B(currentCell);

        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleSouth = grid.MS(currentCell);
        var middleNorth = grid.MN(currentCell);

        var upperEast = grid.UE(currentCell);
        var upperWest = grid.UW(currentCell);
        var upperNorth = grid.UN(currentCell);
        var upperSouth = grid.US(currentCell);
        
        if (TargetCellUnoccupied(up) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellsExist_Linear(middleEast, upperEast) ||         // Travel through the East side
             ConnectorCellsExist_Linear(middleWest, upperWest) ||         // Travel through the West side
             ConnectorCellsExist_Linear(middleNorth, upperNorth) ||       // Travel through the North side
             ConnectorCellsExist_Linear(middleSouth, upperSouth)) &&      // Travel through the South side
            (bottom == null || bottom.Alive == false))

            return Action.Up;
        else
            return Action.NoAction;
    }


    public Action Down(Cell currentCell)
    {
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleSouth = grid.MS(currentCell);
        var middleNorth = grid.MN(currentCell);

        var bottomEast = grid.BE(currentCell);
        var bottomWest = grid.BW(currentCell);
        var bottomNorth = grid.BN(currentCell);
        var bottomSouth = grid.BS(currentCell);

        if (TargetCellUnoccupied(bottom) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellsExist_Linear(middleEast, bottomEast) ||         // Travel through the East side
             ConnectorCellsExist_Linear(middleWest, bottomWest) ||         // Travel through the West side
             ConnectorCellsExist_Linear(middleNorth, bottomNorth) ||       // Travel through the North side
             ConnectorCellsExist_Linear(middleSouth, bottomSouth)) &&      // Travel through the South side
            (up == null || up.Alive == false))

            return Action.Down;
        else
            return Action.NoAction;
    }



    // CONVEX/CONCAVE TRANSITIONS
    public Action UpNorth(Cell currentCell)
    {
        var upperNorth = grid.UN(currentCell);
        var up = grid.U(currentCell);
        var middleNorth = grid.MN(currentCell);
        var bottom = grid.B(currentCell);

        var middleSouth = grid.MS(currentCell);
        var upperSouth = grid.US(currentCell);
        var up2 = grid.Cells[currentCell.Location.x, currentCell.Location.y + 2, currentCell.Location.z];
        var up2North = grid.Cells[currentCell.Location.x, currentCell.Location.y + 2, currentCell.Location.z + 1];

        if (TargetCellUnoccupied(upperNorth) &&
            TransitionCellUnoccupied(up) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleNorth) ||
             ConnectorCellsExist_Concave(middleSouth, upperSouth, up2, up2North)) &&
            (bottom == null || bottom.Alive == false))

            return Action.UpNorth;
        else
            return Action.NoAction;
    }


    public Action DownNorth(Cell currentCell)
    {
        var bottomNorth = grid.BN(currentCell);
        var middleNorth = grid.MN(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleSouth = grid.MS(currentCell);
        var bottomSouth = grid.BS(currentCell);
        var bottom2 = grid.Cells[currentCell.Location.x, currentCell.Location.y - 2, currentCell.Location.z];
        var bottom2North = grid.Cells[currentCell.Location.x, currentCell.Location.y - 2, currentCell.Location.z + 1];

        if (TargetCellUnoccupied(bottomNorth) &&
            TransitionCellUnoccupied(bottom) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleNorth) ||
             ConnectorCellsExist_Concave(middleSouth, bottomSouth, bottom2, bottom2North)) &&
            (up == null || up.Alive == false))

            return Action.DownNorth;
        else
            return Action.NoAction;
    }


    public Action NorthUp(Cell currentCell)
    {
        var upperNorth = grid.UN(currentCell);
        var middleNorth = grid.MN(currentCell);
        var middleSouth = grid.MS(currentCell);
        var up = grid.U(currentCell);

        var bottom = grid.B(currentCell);
        var bottomNorth = grid.BN(currentCell);
        var north2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z + 2];
        var upperNorth2 = grid.Cells[currentCell.Location.x, currentCell.Location.y + 1, currentCell.Location.z + 2];

        if (TargetCellUnoccupied(upperNorth) &&
            TransitionCellUnoccupied(middleNorth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(up) ||
             ConnectorCellsExist_Concave(bottom, bottomNorth, north2, upperNorth2)) &&
            (middleSouth == null || middleSouth.Alive == false))

            return Action.NorthUp;
        else
            return Action.NoAction;
    }


    public Action NorthDown(Cell currentCell)
    {
        var bottomNorth = grid.BN(currentCell);
        var middleNorth = grid.MN(currentCell);
        var middleSouth = grid.MS(currentCell);
        var bottom = grid.B(currentCell);

        var up = grid.U(currentCell);
        var upperNorth = grid.UN(currentCell);
        var north2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z + 2];
        var bottomNorth2 = grid.Cells[currentCell.Location.x, currentCell.Location.y - 1, currentCell.Location.z + 2];

        if (TargetCellUnoccupied(bottomNorth) && 
            TransitionCellUnoccupied(middleNorth) && 
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(bottom) ||
             ConnectorCellsExist_Concave(up, upperNorth, north2, bottomNorth2)) &&
            (middleSouth == null || middleSouth.Alive == false))

            return Action.NorthDown; 
        else
            return Action.NoAction;
    }


    public Action UpSouth(Cell currentCell)
    {
        var upperSouth = grid.US(currentCell);
        var up = grid.U(currentCell);
        var middleSouth = grid.MS(currentCell);
        var bottom = grid.B(currentCell);

        var middleNorth = grid.MN(currentCell);
        var upperNorth = grid.UN(currentCell);
        var up2 = grid.Cells[currentCell.Location.x, currentCell.Location.y + 2, currentCell.Location.z];
        var up2South = grid.Cells[currentCell.Location.x, currentCell.Location.y + 2, currentCell.Location.z - 1];

        if (TargetCellUnoccupied(upperSouth) &&
            TransitionCellUnoccupied(up) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleSouth) ||
             ConnectorCellsExist_Concave(middleNorth, upperNorth, up2, up2South)) &&
            (bottom == null || bottom.Alive == false))

            return Action.UpSouth;
        else
            return Action.NoAction;
    }


    public Action DownSouth(Cell currentCell)
    {
        var bottomSouth = grid.BS(currentCell);
        var middleSouth = grid.MS(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleNorth = grid.MN(currentCell);
        var bottomNorth = grid.BN(currentCell);
        var bottom2 = grid.Cells[currentCell.Location.x, currentCell.Location.y - 2, currentCell.Location.z];
        var bottom2South = grid.Cells[currentCell.Location.x, currentCell.Location.y - 2, currentCell.Location.z - 1];

        if (TargetCellUnoccupied(bottomSouth) &&
            TransitionCellUnoccupied(bottom) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleSouth) ||
             ConnectorCellsExist_Concave(middleNorth, bottomNorth, bottom2, bottom2South)) &&
            (up == null || up.Alive == false))

            return Action.DownSouth;
        else
            return Action.NoAction;
    }


    public Action SouthUp(Cell currentCell)
    {
        var upperSouth = grid.US(currentCell);
        var middleNorth = grid.MN(currentCell);
        var middleSouth = grid.MS(currentCell);
        var up = grid.U(currentCell);

        var bottom = grid.B(currentCell);
        var bottomSouth = grid.BS(currentCell);
        var south2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z - 2];
        var upperSouth2 = grid.Cells[currentCell.Location.x, currentCell.Location.y + 1, currentCell.Location.z - 2];

        if (TargetCellUnoccupied(upperSouth) &&
            TransitionCellUnoccupied(middleSouth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(up) ||
             ConnectorCellsExist_Concave(bottom, bottomSouth, south2, upperSouth2)) &&
            (middleNorth == null || middleNorth.Alive == false))

            return Action.SouthUp;
        else
            return Action.NoAction;
    }


    public Action SouthDown(Cell currentCell)
    {
        var bottomSouth = grid.BS(currentCell);
        var middleNorth = grid.MN(currentCell);
        var middleSouth = grid.MS(currentCell);
        var bottom = grid.B(currentCell);

        var up = grid.U(currentCell);
        var upperSouth = grid.US(currentCell);
        var south2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z - 2];
        var bottomSouth2 = grid.Cells[currentCell.Location.x, currentCell.Location.y - 1, currentCell.Location.z - 2];

        if (TargetCellUnoccupied(bottomSouth) &&
            TransitionCellUnoccupied(middleSouth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(bottom) ||
             ConnectorCellsExist_Concave(up, upperSouth, south2, bottomSouth2)) &&
            (middleNorth == null || middleNorth.Alive == false))

            return Action.SouthDown;
        else
            return Action.NoAction;
    }


    public Action UpEast(Cell currentCell)
    {
        var upperEast = grid.UE(currentCell);
        var up = grid.U(currentCell);
        var middleEast = grid.ME(currentCell);
        var bottom = grid.B(currentCell);

        var middleWest = grid.MW(currentCell);
        var upperWest = grid.UW(currentCell);
        var up2 = grid.Cells[currentCell.Location.x, currentCell.Location.y + 2, currentCell.Location.z];
        var up2East = grid.Cells[currentCell.Location.x + 1, currentCell.Location.y + 2, currentCell.Location.z];

        if (TargetCellUnoccupied(upperEast) &&
            TransitionCellUnoccupied(up) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleEast) ||
             ConnectorCellsExist_Concave(middleWest, upperWest, up2, up2East)) &&
            (bottom == null || bottom.Alive == false))

            return Action.UpEast;
        else
            return Action.NoAction;
    }


    public Action DownEast(Cell currentCell)
    {
        var bottomEast = grid.BE(currentCell);
        var middleEast = grid.ME(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleWest = grid.MW(currentCell);
        var bottomWest = grid.BW(currentCell);
        var bottom2 = grid.Cells[currentCell.Location.x, currentCell.Location.y - 2, currentCell.Location.z];
        var bottom2East = grid.Cells[currentCell.Location.x + 1, currentCell.Location.y - 2, currentCell.Location.z];

        if (TargetCellUnoccupied(bottomEast) &&
            TransitionCellUnoccupied(bottom) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleEast) ||
             ConnectorCellsExist_Concave(middleWest, bottomWest, bottom2, bottom2East)) &&
            (up == null || up.Alive == false))

            return Action.DownEast;
        else
            return Action.NoAction;
    }


    public Action EastUp(Cell currentCell)
    {
        var upperEast = grid.UE(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var up = grid.U(currentCell);

        var bottom = grid.B(currentCell);
        var bottomEast = grid.BE(currentCell);
        var east2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y, currentCell.Location.z];
        var upperEast2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y + 1, currentCell.Location.z];

        if (TargetCellUnoccupied(upperEast) &&
            TransitionCellUnoccupied(middleEast) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(up) ||
             ConnectorCellsExist_Concave(bottom, bottomEast, east2, upperEast2)) &&
            (middleWest == null || middleWest.Alive == false))

            return Action.EastUp;
        else
            return Action.NoAction;
    }


    public Action EastDown(Cell currentCell)
    {
        var bottomEast = grid.BE(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var bottom = grid.B(currentCell);

        var up = grid.U(currentCell);
        var upperEast = grid.UE(currentCell);
        var east2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y, currentCell.Location.z];
        var bottomEast2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y - 1, currentCell.Location.z];

        if (TargetCellUnoccupied(bottomEast) &&
            TransitionCellUnoccupied(middleEast) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(bottom) ||
             ConnectorCellsExist_Concave(up, upperEast, east2, bottomEast2)) &&
            (middleWest == null || middleWest.Alive == false))

            return Action.EastDown;
        else
            return Action.NoAction;
    }



    public Action UpWest(Cell currentCell)
    {
        var upperWest = grid.UW(currentCell);
        var up = grid.U(currentCell);
        var middleWest = grid.MW(currentCell);
        var bottom = grid.B(currentCell);

        var middleEast = grid.ME(currentCell);
        var upperEast = grid.UE(currentCell);
        var up2 = grid.Cells[currentCell.Location.x, currentCell.Location.y + 2, currentCell.Location.z];
        var up2West = grid.Cells[currentCell.Location.x - 1, currentCell.Location.y + 2, currentCell.Location.z];

        if (TargetCellUnoccupied(upperWest) &&
            TransitionCellUnoccupied(up) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleWest) ||
             ConnectorCellsExist_Concave(middleEast, upperEast, up2, up2West)) &&
            (bottom == null || bottom.Alive == false))

            return Action.UpWest;
        else
            return Action.NoAction;
    }


    public Action DownWest(Cell currentCell)
    {
        var bottomWest = grid.BW(currentCell);
        var middleWest = grid.MW(currentCell);
        var bottom = grid.B(currentCell);
        var up = grid.U(currentCell);

        var middleEast = grid.ME(currentCell);
        var bottomEast = grid.BE(currentCell);
        var bottom2 = grid.Cells[currentCell.Location.x, currentCell.Location.y - 2, currentCell.Location.z];
        var bottom2West = grid.Cells[currentCell.Location.x - 1, currentCell.Location.y - 2, currentCell.Location.z];

        if (TargetCellUnoccupied(bottomWest) &&
            TransitionCellUnoccupied(bottom) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleWest) ||
             ConnectorCellsExist_Concave(middleEast, bottomEast, bottom2, bottom2West)) &&
            (up == null || up.Alive == false))

            return Action.DownWest;
        else
            return Action.NoAction;
    }


    public Action WestUp(Cell currentCell)
    {
        var upperWest = grid.UE(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleEast = grid.ME(currentCell);
        var up = grid.U(currentCell);

        var bottom = grid.B(currentCell);
        var bottomWest = grid.BW(currentCell);
        var west2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y, currentCell.Location.z];
        var upperWest2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y + 1, currentCell.Location.z];

        if (TargetCellUnoccupied(upperWest) &&
            TransitionCellUnoccupied(middleWest) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(up) ||
             ConnectorCellsExist_Concave(bottom, bottomWest, west2, upperWest2)) &&
            (middleEast == null || middleEast.Alive == false))

            return Action.WestUp;
        else
            return Action.NoAction;
    }


    public Action WestDown(Cell currentCell)
    {
        var bottomWest = grid.BW(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleEast = grid.ME(currentCell);
        var bottom = grid.B(currentCell);

        var up = grid.U(currentCell);
        var upperWest = grid.UW(currentCell);
        var west2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y, currentCell.Location.z];
        var bottomWest2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y - 1, currentCell.Location.z];

        if (TargetCellUnoccupied(bottomWest) &&
            TransitionCellUnoccupied(middleWest) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(bottom) ||
             ConnectorCellsExist_Concave(up, upperWest, west2, bottomWest2)) &&
            (middleEast == null || middleEast.Alive == false))

            return Action.EastDown;
        else
            return Action.NoAction;
    }

   
    public Action NorthEast(Cell currentCell)
    {
        var northEast = grid.MNE(currentCell);
        var middleNorth = grid.MN(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleSouth = grid.MS(currentCell);

        var middleWest = grid.MW(currentCell);
        var northWest = grid.MNW(currentCell);
        var north2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z + 2];
        var north2East = grid.Cells[currentCell.Location.x + 1, currentCell.Location.y, currentCell.Location.z + 2];

        if (TargetCellUnoccupied(northEast) &&
            TransitionCellUnoccupied(middleNorth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleEast) ||
             ConnectorCellsExist_Concave(middleWest, northWest, north2, north2East)) &&
            (middleSouth == null || middleSouth.Alive == false))

            return Action.NorthEast;
        else
            return Action.NoAction;
    }


    public Action NorthWest(Cell currentCell)
    {
        var northWest = grid.MNW(currentCell);
        var middleNorth = grid.MN(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleSouth = grid.MS(currentCell);

        var middleEast = grid.ME(currentCell);
        var northEast = grid.MNE(currentCell);
        var north2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z + 2];
        var north2West = grid.Cells[currentCell.Location.x - 1, currentCell.Location.y, currentCell.Location.z + 2];

        if (TargetCellUnoccupied(northWest) &&
            TransitionCellUnoccupied(middleNorth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleWest) ||
             ConnectorCellsExist_Concave(middleEast, northEast, north2, north2West)) &&
            (middleSouth == null || middleSouth.Alive == false))

            return Action.NorthWest;
        else
            return Action.NoAction;
    }


    public Action EastNorth(Cell currentCell)
    {
        var northEast = grid.MNE(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleNorth = grid.MN(currentCell);

        var middleSouth = grid.MS(currentCell);
        var southEast = grid.MSE(currentCell);
        var east2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y, currentCell.Location.z];
        var northEast2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y, currentCell.Location.z + 1];

        if (TargetCellUnoccupied(northEast) &&
            TransitionCellUnoccupied(middleEast) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleNorth) ||
             ConnectorCellsExist_Concave(middleSouth, southEast, east2, northEast2)) &&
            (middleWest == null || middleWest.Alive == false))

            return Action.EastNorth;
        else
            return Action.NoAction;
    }


    public Action WestNorth(Cell currentCell)
    {
        var northWest = grid.MNW(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleNorth = grid.MN(currentCell);

        var middleSouth = grid.MS(currentCell);
        var southWest = grid.MSW(currentCell);
        var west2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y, currentCell.Location.z];
        var northWest2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y, currentCell.Location.z + 1];

        if (TargetCellUnoccupied(northWest) &&
            TransitionCellUnoccupied(middleWest) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleNorth) ||
             ConnectorCellsExist_Concave(middleSouth, southWest, west2, northWest2)) &&
            (middleEast == null || middleEast.Alive == false))

            return Action.WestNorth;
        else
            return Action.NoAction;
    }


    public Action SouthEast(Cell currentCell)
    {
        var southEast = grid.MNE(currentCell);
        var middleSouth = grid.MS(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleNorth = grid.MN(currentCell);

        var middleWest = grid.MW(currentCell);
        var southWest = grid.MSW(currentCell);
        var south2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z - 2];
        var south2East = grid.Cells[currentCell.Location.x + 1, currentCell.Location.y, currentCell.Location.z - 2];

        if (TargetCellUnoccupied(southEast) &&
            TransitionCellUnoccupied(middleSouth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleEast) ||
             ConnectorCellsExist_Concave(middleWest, southWest, south2, south2East)) &&
            (middleNorth == null || middleNorth.Alive == false))

            return Action.SouthEast;
        else
            return Action.NoAction;
    }


    public Action SouthWest(Cell currentCell)
    {
        var southWest = grid.MSW(currentCell);
        var middleSouth = grid.MS(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleNorth = grid.MN(currentCell);

        var middleEast = grid.ME(currentCell);
        var southEast = grid.MSE(currentCell);
        var south2 = grid.Cells[currentCell.Location.x, currentCell.Location.y, currentCell.Location.z - 2];
        var south2West = grid.Cells[currentCell.Location.x - 1, currentCell.Location.y, currentCell.Location.z - 2];

        if (TargetCellUnoccupied(southWest) &&
            TransitionCellUnoccupied(middleSouth) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleWest) ||
             ConnectorCellsExist_Concave(middleEast, southEast, south2, south2West)) &&
            (middleNorth == null || middleNorth.Alive == false))

            return Action.SouthWest;
        else
            return Action.NoAction;
    }


    public Action EastSouth(Cell currentCell)
    {
        var southEast = grid.MSE(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleSouth = grid.MS(currentCell);

        var middleNorth = grid.MN(currentCell);
        var northEast = grid.MNE(currentCell);
        var east2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y, currentCell.Location.z];
        var southEast2 = grid.Cells[currentCell.Location.x + 2, currentCell.Location.y, currentCell.Location.z - 1];

        if (TargetCellUnoccupied(southEast) &&
            TransitionCellUnoccupied(middleEast) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleSouth) ||
             ConnectorCellsExist_Concave(middleNorth, northEast, east2, southEast2)) &&
            (middleWest == null || middleWest.Alive == false))

            return Action.EastSouth;
        else
            return Action.NoAction;
    }


    public Action WestSouth(Cell currentCell)
    {
        var southWest = grid.MSW(currentCell);
        var middleWest = grid.MW(currentCell);
        var middleEast = grid.ME(currentCell);
        var middleSouth = grid.MS(currentCell);

        var middleNorth = grid.MN(currentCell);
        var northWest = grid.MNW(currentCell);
        var west2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y, currentCell.Location.z];
        var southWest2 = grid.Cells[currentCell.Location.x - 2, currentCell.Location.y, currentCell.Location.z - 1];

        if (TargetCellUnoccupied(southWest) &&
            TransitionCellUnoccupied(middleWest) &&
            PotentialDisconnections(currentCell) == false &&
            (ConnectorCellExists_Convex(middleSouth) ||
             ConnectorCellsExist_Concave(middleNorth, northWest, west2, southWest2)) &&
            (middleEast == null || middleEast.Alive == false))

            return Action.WestSouth;
        else
            return Action.NoAction;
    }






    ////////////////////////////   MOVEMENTS  ////////////////////////////

    // LINEAR MOVEMENT
    // LinearMovement: Executes a Linear Mouvement and updates the Cell grid
    public IEnumerator LinearMove(Cell currentCell, Cell targetCell, Vector3 direction, Agent agent, int CellSize)
    {
        agent.Obj.transform.Translate(direction * CellSize);
        agent.Location = targetCell;

        currentCell.Alive = false;
        targetCell.Alive = true;

        yield return new WaitForSeconds(speed);
    }


    // ConvexConcaveMovement: Executes a Concave OR Convex Movement and updates the Cell grid
    public IEnumerator ConvexConcaveMove(Cell currentCell, Cell transitionCell, Cell targetCell, 
                                             Vector3 direction1, Vector3 direction2, Agent agent, int CellSize)
    {
        agent.Obj.transform.Translate(direction1 * CellSize);
        agent.Location = transitionCell;

        currentCell.Alive = false;
        transitionCell.Alive = true;

        yield return new WaitForSeconds(speed);


        agent.Obj.transform.Translate(direction2 * CellSize);
        agent.Location = targetCell;

        transitionCell.Alive = false;
        targetCell.Alive = true;

        yield return new WaitForSeconds(speed);
    }


}
