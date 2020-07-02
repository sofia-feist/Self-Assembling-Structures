using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



// Action enumerator: Enumerates possible actions
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
    CommonMethods CM = new CommonMethods();


    float speed;
    static List<Rule> ruleList;




    // Constructor
    public ReconfigurationRules(float _speed)
    {
        speed = _speed;
        ruleList = RuleList();
    }






    // RuleList: Compilation of the possible movement rules
    public List<Rule> RuleList()
    {
        var rules = new List<Rule>();

        // Linear Mouvements
        rules.Add(new North());
        rules.Add(new South());
        rules.Add(new East());
        rules.Add(new West());
        rules.Add(new Up());
        rules.Add(new Down());

        // Convex/Concave Mouvements
        rules.Add(new UpNorth());
        rules.Add(new DownNorth());
        rules.Add(new NorthUp());
        rules.Add(new NorthDown());

        rules.Add(new UpSouth());
        rules.Add(new DownSouth());
        rules.Add(new SouthUp());
        rules.Add(new SouthDown());

        rules.Add(new UpEast());
        rules.Add(new DownEast());
        rules.Add(new EastUp());
        rules.Add(new EastDown());

        rules.Add(new UpWest());
        rules.Add(new DownWest());
        rules.Add(new WestUp());
        rules.Add(new WestDown());

        rules.Add(new NorthEast());
        rules.Add(new NorthWest());
        rules.Add(new EastNorth());
        rules.Add(new WestNorth());

        rules.Add(new SouthEast());
        rules.Add(new SouthWest());
        rules.Add(new EastSouth());
        rules.Add(new WestSouth());

        return rules;
    }





    // ChoseAction: Choose an action to take according rule fitness
    public Action ChoseAction(Agent agent)
    {
        Action nextAction = Action.NoAction;
        CM.RandomShuffle(ruleList);

        List<Rule> rulesThatApply = new List<Rule>();

        foreach (var rule in ruleList)  
        {
            Cell currentCell = agent.Cell;
            rule.SetTargetCell(currentCell);
            Action currentAction = rule.CheckAction(currentCell);

            if (currentAction != Action.NoAction)
            {
                rule.CalculateFitness(currentCell);
                rulesThatApply.Add(rule);
                rulesThatApply = rulesThatApply.OrderByDescending(r => r.fitness).ToList();    // Sort by fitness
            }
        }

        if (rulesThatApply.Count != 0) 
            nextAction = rulesThatApply[0].action;

        return nextAction;
    }



    // ExecuteAction: Executes a given Action
    public void ExecuteAction(MonoBehaviour mono, Action action, Agent agent)
    {
        Cell currentCell = agent.Cell;
        int cellSize = currentCell.CellSize;

        switch (action)
        {
            // Linear Mouvements
            case Action.North:
                mono.StartCoroutine(
                    LinearMove(currentCell, currentCell.MiddleNorth(), Vector3.forward, agent, cellSize));
                break;
            case Action.South:
                mono.StartCoroutine(
                    LinearMove(currentCell, currentCell.MiddleSouth(), Vector3.back, agent, cellSize));
                break;
            case Action.East:
                mono.StartCoroutine(
                    LinearMove(currentCell, currentCell.MiddleEast(), Vector3.right, agent, cellSize));
                break;
            case Action.West:
                mono.StartCoroutine(
                    LinearMove(currentCell, currentCell.MiddleWest(), Vector3.left, agent, cellSize));
                break;
            case Action.Up:
                mono.StartCoroutine(
                    LinearMove(currentCell, currentCell.Up(), Vector3.up, agent, cellSize));
                break;
            case Action.Down:
                mono.StartCoroutine(
                    LinearMove(currentCell, currentCell.Bottom(), Vector3.down, agent, cellSize));
                break;


            // Convex/Concave Mouvements
            case Action.UpNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Up(), currentCell.UpperNorth(), Vector3.up, Vector3.forward, agent, cellSize));
                break;
            case Action.DownNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Bottom(), currentCell.BottomNorth(), Vector3.down, Vector3.forward, agent, cellSize));
                break;
            case Action.NorthUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleNorth(), currentCell.UpperNorth(), Vector3.forward, Vector3.up, agent, cellSize));
                break;
            case Action.NorthDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleNorth(), currentCell.BottomNorth(), Vector3.forward, Vector3.down, agent, cellSize));
                break;


            case Action.UpSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Up(), currentCell.UpperSouth(), Vector3.up, Vector3.back, agent, cellSize));
                break;
            case Action.DownSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Bottom(), currentCell.BottomSouth(), Vector3.down, Vector3.back, agent, cellSize));
                break;
            case Action.SouthUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleSouth(), currentCell.UpperSouth(), Vector3.back, Vector3.up, agent, cellSize));
                break;
            case Action.SouthDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleSouth(), currentCell.BottomSouth(), Vector3.back, Vector3.down, agent, cellSize));
                break;


            case Action.UpEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Up(), currentCell.UpperEast(), Vector3.up, Vector3.right, agent, cellSize));
                break;
            case Action.DownEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Bottom(), currentCell.BottomEast(), Vector3.down, Vector3.right, agent, cellSize));
                break;
            case Action.EastUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleEast(), currentCell.UpperEast(), Vector3.right, Vector3.up, agent, cellSize));
                break;
            case Action.EastDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleEast(), currentCell.BottomEast(), Vector3.right, Vector3.down, agent, cellSize));
                break;


            case Action.UpWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Up(), currentCell.UpperWest(), Vector3.up, Vector3.left, agent, cellSize));
                break;
            case Action.DownWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.Bottom(), currentCell.BottomWest(), Vector3.down, Vector3.left, agent, cellSize));
                break;
            case Action.WestUp:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleWest(), currentCell.UpperWest(), Vector3.left, Vector3.up, agent, cellSize));
                break;
            case Action.WestDown:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleWest(), currentCell.BottomWest(), Vector3.left, Vector3.down, agent, cellSize));
                break;


            case Action.NorthEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleNorth(), currentCell.MiddleNorthEast(), Vector3.forward, Vector3.right, agent, cellSize));
                break;
            case Action.NorthWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleNorth(), currentCell.MiddleNorthWest(), Vector3.forward, Vector3.left, agent, cellSize));
                break;
            case Action.EastNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleEast(), currentCell.MiddleNorthEast(), Vector3.right, Vector3.forward, agent, cellSize));
                break;
            case Action.WestNorth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleWest(), currentCell.MiddleNorthWest(), Vector3.left, Vector3.forward, agent, cellSize));
                break;


            case Action.SouthEast:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleSouth(), currentCell.MiddleSouthEast(), Vector3.back, Vector3.right, agent, cellSize));
                break;
            case Action.SouthWest:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleSouth(), currentCell.MiddleSouthWest(), Vector3.back, Vector3.left, agent, cellSize));
                break;
            case Action.EastSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleEast(), currentCell.MiddleSouthEast(), Vector3.right, Vector3.back, agent, cellSize));
                break;
            case Action.WestSouth:
                mono.StartCoroutine(
                    ConvexConcaveMove(currentCell, currentCell.MiddleWest(), currentCell.MiddleSouthWest(), Vector3.left, Vector3.back, agent, cellSize));
                break;
        }
    }







    ////////////////////////////   ACTION RULES  ////////////////////////////

    // ABSTRACT RULE
    public abstract class Rule
    {
        public int fitness { get; set; }
        public Cell targetCell { get; set; }
        public Action action { get; set; }





        // ABSTRACT METHODS:
        public abstract Action CheckAction(Cell currentCell);
        public abstract void SetTargetCell(Cell currentCell);



        // COMMON METHODS: 
        // CheckDisconnections: checks a given cell's neighbourhood to predict potential disconnections in the structure
        public bool PotentialDisconnections(Cell currentCell)
        {
            var FaceNeighbours = currentCell.GetFaceNeighbours().Where(n => n.Alive);
            var AllNeighbours = currentCell.GetAllNeighbours().Where(n => n.Alive);

            foreach (var neighbour in FaceNeighbours)
            {
                var nNeighbours = neighbour.GetFaceNeighbours().Where(n => n.Alive);
                var adjacentNeighbours = FaceNeighbours.Where(n => n.Location != neighbour.Location && n.OppositeNeighbours(neighbour) == false);

                var potentialCommonNeighbours = new List<Cell>();
                foreach (var n in adjacentNeighbours)
                    potentialCommonNeighbours.AddRange(n.GetFaceNeighbours().Where(a => a.Alive));

                var commonNeighbours = nNeighbours.Intersect(potentialCommonNeighbours).ToList();

                if ((FaceNeighbours.Count() != 1 && nNeighbours.Count() == 1) ||                                                                         // If neighbour only has one face neighbour (aka current cell), DO NOT MOVE
                    (FaceNeighbours.Count() == 2 && FaceNeighbours.ElementAt(0).OppositeNeighbours(FaceNeighbours.ElementAt(1))                          // If current cell only has two face neighbours on opposite faces, DO NOT MOVE (unless one of them is a final state agent or seed)
                         && FaceNeighbours.All(n => n.agent.State == AgentState.Active || n.agent.State == AgentState.Inactive) 
                         && Random.Range(0.0f, 1.0f) > 0.05) ||                                                                                      // This random range gives this condition a very small probability(5%) of being disregarded (-> TO AVOID LOCAL OPTIMA, although it also gives the structure a chance to disconnect)
                    (FaceNeighbours.Count() > 1 && commonNeighbours.Count() == 1 && commonNeighbours.ElementAt(0).Location == currentCell.Location       // If current cell is the only cell in common between two face neighbours, DO NOT MOVE (unless one of them is a final state agent or seed)
                         && FaceNeighbours.All(n => n.agent.State == AgentState.Active || n.agent.State == AgentState.Inactive)))
                    return true;    
            }

            return false;
        }


        // TARGET/TRANSITION CELLS CHECK
        // TargetCellUnoccupied: Check if a given target cell an agent is trying to move into is unoccupied
        public bool TargetCellUnoccupied(Cell TargetCell) { return (TargetCell?.Alive == false) ? true : false; }


        // TransitionCellUnoccupied: Check if a given transition cell an agent has to pass through to get to the target cell is unoccupied
        public bool TransitionCellUnoccupied(Cell TransitionCell) { return (TransitionCell?.Alive == false) ? true : false; }



        // CONNECTOR CELLS CHECKS
        // ConnectorCellsExist_Linear: Checks if the connector cells through which the agent is and will connect to exist; for Linear Mouvement
        public bool ConnectorCellsExist_Linear(Cell InitialConnectorCell, Cell GoalConnectorCell) {
            return (InitialConnectorCell?.Alive == true && GoalConnectorCell?.Alive == true) ? true : false; }


        // ConnectorCellExists_Convex: Checks if the connector cell which the agent will rotate around exists; for Convex Mouvement
        public bool ConnectorCellExists_Convex(Cell ConvexConnectorCell) { return ConvexConnectorCell?.Alive == true ? true : false; }


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


        public void CalculateFitness(Cell currentCell)
        {
            int currentScent = currentCell.agent.ScentValue;
            var targetNeighbours = targetCell.GetFaceNeighbours().Where(a => a.Alive == true);
            int newScentValue = (targetNeighbours.Count() != 0) ? targetNeighbours.Select(s => s.agent.ScentValue).Max() - 1 : 0;

            if (currentScent == newScentValue)
                fitness = 0;
            else if (newScentValue > currentScent)
                fitness = 1;
            else
                fitness = -1;
        }
    }







    ////// SPECIFIC RULES
    ///
    // LINEAR TRANSITIONS

    // North
    public class North : Rule
    {
        public North()  { action = Action.North; }
        
        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleNorth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var middleNorth = targetCell;

            var middleEast = currentCell.MiddleEast();
            var middleWest = currentCell.MiddleWest();
            var bottom = currentCell.Bottom();
            var up = currentCell.Up();

            var middleNorthEast = currentCell.MiddleNorthEast();
            var middleNorthWest = currentCell.MiddleNorthWest();
            var bottomNorth = currentCell.BottomNorth();
            var upNorth = currentCell.UpperNorth();

            if (TargetCellUnoccupied(middleNorth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellsExist_Linear(middleEast, middleNorthEast) ||      // Travel through the East side
                 ConnectorCellsExist_Linear(middleWest, middleNorthWest) ||      // Travel through the West side
                 ConnectorCellsExist_Linear(bottom, bottomNorth) ||              // Travel through the Bottom side
                 ConnectorCellsExist_Linear(up, upNorth)))                       // Travel through the Upper side

                return Action.North;
            else
                return Action.NoAction;
        }
    }

    // South
    public class South : Rule
    {
        public South() { action = Action.South; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleSouth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var middleSouth = targetCell;

            var middleEast = currentCell.MiddleEast();
            var middleWest = currentCell.MiddleWest();
            var bottom = currentCell.Bottom();
            var up = currentCell.Up();

            var middleSouthEast = currentCell.MiddleSouthEast();
            var middleSouthWest = currentCell.MiddleSouthWest();
            var upperSouth = currentCell.UpperSouth();
            var bottomSouth = currentCell.BottomSouth();

            if (TargetCellUnoccupied(middleSouth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellsExist_Linear(middleEast, middleSouthEast) ||      // Travel through the East side
                 ConnectorCellsExist_Linear(middleWest, middleSouthWest) ||      // Travel through the West side
                 ConnectorCellsExist_Linear(bottom, bottomSouth) ||              // Travel through the Bottom side
                 ConnectorCellsExist_Linear(up, upperSouth)))                    // Travel through the Upper side

                return Action.South;
            else
                return Action.NoAction;
        }
    }

    // East
    public class East : Rule
    {
        public East() { action = Action.East; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var middleEast = targetCell;

            var middleSouth = currentCell.MiddleSouth();
            var middleNorth = currentCell.MiddleNorth();
            var bottom = currentCell.Bottom();
            var up = currentCell.Up();

            var middleSouthEast = currentCell.MiddleSouthEast();
            var middleNorthEast = currentCell.MiddleNorthEast();
            var upperEast = currentCell.UpperEast();
            var bottomEast = currentCell.BottomEast();

            if (TargetCellUnoccupied(middleEast) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellsExist_Linear(middleSouth, middleSouthEast) ||      // Travel through the South side
                 ConnectorCellsExist_Linear(middleNorth, middleNorthEast) ||      // Travel through the North side
                 ConnectorCellsExist_Linear(bottom, bottomEast) ||                // Travel through the Bottom side
                 ConnectorCellsExist_Linear(up, upperEast)))                      // Travel through the Upper side

                return Action.East;
            else
                return Action.NoAction;
        }
    }

    // West
    public class West : Rule
    {
        public West() { action = Action.West; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var middleWest = targetCell;

            var middleSouth = currentCell.MiddleSouth();
            var middleNorth = currentCell.MiddleNorth();
            var bottom = currentCell.Bottom();
            var up = currentCell.Up();

            var middleSouthWest = currentCell.MiddleSouthWest();
            var middleNorthWest = currentCell.MiddleNorthWest();
            var upperWest = currentCell.UpperWest();
            var bottomWest = currentCell.BottomWest();

            if (TargetCellUnoccupied(middleWest) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellsExist_Linear(middleSouth, middleSouthWest) ||      // Travel through the South side
                 ConnectorCellsExist_Linear(middleNorth, middleNorthWest) ||      // Travel through the North side
                 ConnectorCellsExist_Linear(bottom, bottomWest) ||                // Travel through the Bottom side
                 ConnectorCellsExist_Linear(up, upperWest)))                      // Travel through the Upper side

                return Action.West;
            else
                return Action.NoAction;
        }
    }

    // Up
    public class Up : Rule
    {
        public Up() { action = Action.Up; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.Up(); }


        public override Action CheckAction(Cell currentCell)
        {
            var up = targetCell;

            var middleEast = currentCell.MiddleEast();
            var middleWest = currentCell.MiddleWest();
            var middleSouth = currentCell.MiddleSouth();
            var middleNorth = currentCell.MiddleNorth();

            var upperEast = currentCell.UpperEast();
            var upperWest = currentCell.UpperWest();
            var upperNorth = currentCell.UpperNorth();
            var upperSouth = currentCell.UpperSouth();

            if (TargetCellUnoccupied(up) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellsExist_Linear(middleEast, upperEast) ||         // Travel through the East side
                 ConnectorCellsExist_Linear(middleWest, upperWest) ||         // Travel through the West side
                 ConnectorCellsExist_Linear(middleNorth, upperNorth) ||       // Travel through the North side
                 ConnectorCellsExist_Linear(middleSouth, upperSouth)))        // Travel through the South side

                return Action.Up;
            else
                return Action.NoAction;
        }
    }

    // Down
    public class Down : Rule
    {
        public Down() { action = Action.Down; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.Bottom(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottom = targetCell;

            var middleEast = currentCell.MiddleEast();
            var middleWest = currentCell.MiddleWest();
            var middleSouth = currentCell.MiddleSouth();
            var middleNorth = currentCell.MiddleNorth();

            var bottomEast = currentCell.BottomEast();
            var bottomWest = currentCell.BottomWest();
            var bottomNorth = currentCell.BottomNorth();
            var bottomSouth = currentCell.BottomSouth();

            if (TargetCellUnoccupied(bottom) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellsExist_Linear(middleEast, bottomEast) ||         // Travel through the East side
                 ConnectorCellsExist_Linear(middleWest, bottomWest) ||         // Travel through the West side
                 ConnectorCellsExist_Linear(middleNorth, bottomNorth) ||       // Travel through the North side
                 ConnectorCellsExist_Linear(middleSouth, bottomSouth)))        // Travel through the South side

                return Action.Down;
            else
                return Action.NoAction;
        }
    }





    // CONVEX/CONCAVE TRANSITIONS

    // UpNorth
    public class UpNorth : Rule
    {
        public UpNorth() { action = Action.UpNorth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperNorth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperNorth = targetCell;
            var up = currentCell.Up();
            var middleNorth = currentCell.MiddleNorth();

            var middleSouth = currentCell.MiddleSouth();
            var upperSouth = currentCell.UpperSouth();
            var up2 = currentCell.Up2();
            var up2North = currentCell.Up2North();

            if (TargetCellUnoccupied(upperNorth) &&
                TransitionCellUnoccupied(up) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleNorth) ||
                 ConnectorCellsExist_Concave(middleSouth, upperSouth, up2, up2North)))

                return Action.UpNorth;
            else
                return Action.NoAction;
        }
    }

    // DownNorth
    public class DownNorth : Rule
    {
        public DownNorth() { action = Action.DownNorth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomNorth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomNorth = targetCell;
            var middleNorth = currentCell.MiddleNorth();
            var bottom = currentCell.Bottom();

            var middleSouth = currentCell.MiddleSouth();
            var bottomSouth = currentCell.BottomSouth();
            var bottom2 = currentCell.Bottom2();
            var bottom2North = currentCell.Bottom2North();

            if (TargetCellUnoccupied(bottomNorth) &&
                TransitionCellUnoccupied(bottom) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleNorth) ||
                 ConnectorCellsExist_Concave(middleSouth, bottomSouth, bottom2, bottom2North)))

                return Action.DownNorth;
            else
                return Action.NoAction;
        }
    }

    // NorthUp
    public class NorthUp : Rule
    {
        public NorthUp() { action = Action.NorthUp; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperNorth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperNorth = targetCell;
            var middleNorth = currentCell.MiddleNorth();
            var up = currentCell.Up();

            var bottom = currentCell.Bottom();
            var bottomNorth = currentCell.BottomNorth();
            var north2 = currentCell.North2();
            var upperNorth2 = currentCell.UpperNorth2();

            if (TargetCellUnoccupied(upperNorth) &&
                TransitionCellUnoccupied(middleNorth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(up) ||
                 ConnectorCellsExist_Concave(bottom, bottomNorth, north2, upperNorth2)))

                return Action.NorthUp;
            else
                return Action.NoAction;
        }
    }

    // NorthDown
    public class NorthDown : Rule
    {
        public NorthDown() { action = Action.NorthDown; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomNorth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomNorth = targetCell;
            var middleNorth = currentCell.MiddleNorth();
            var bottom = currentCell.Bottom();

            var up = currentCell.Up();
            var upperNorth = currentCell.UpperNorth();
            var north2 = currentCell.North2();
            var bottomNorth2 = currentCell.BottomNorth2();

            if (TargetCellUnoccupied(bottomNorth) &&
                TransitionCellUnoccupied(middleNorth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(bottom) ||
                 ConnectorCellsExist_Concave(up, upperNorth, north2, bottomNorth2)))

                return Action.NorthDown;
            else
                return Action.NoAction;
        }
    }



    // UpSouth
    public class UpSouth : Rule
    {
        public UpSouth() { action = Action.UpSouth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperSouth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperSouth = targetCell;
            var up = currentCell.Up();
            var middleSouth = currentCell.MiddleSouth();

            var middleNorth = currentCell.MiddleNorth();
            var upperNorth = currentCell.UpperNorth();
            var up2 = currentCell.Up2();
            var up2South = currentCell.Up2South();

            if (TargetCellUnoccupied(upperSouth) &&
                TransitionCellUnoccupied(up) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleSouth) ||
                 ConnectorCellsExist_Concave(middleNorth, upperNorth, up2, up2South)))

                return Action.UpSouth;
            else
                return Action.NoAction;
        }
    }

    // DownSouth
    public class DownSouth : Rule
    {
        public DownSouth() { action = Action.DownSouth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomSouth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomSouth = targetCell;
            var middleSouth = currentCell.MiddleSouth();
            var bottom = currentCell.Bottom();

            var middleNorth = currentCell.MiddleNorth();
            var bottomNorth = currentCell.BottomNorth();
            var bottom2 = currentCell.Bottom2();
            var bottom2South = currentCell.Bottom2South();

            if (TargetCellUnoccupied(bottomSouth) &&
                TransitionCellUnoccupied(bottom) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleSouth) ||
                 ConnectorCellsExist_Concave(middleNorth, bottomNorth, bottom2, bottom2South)))

                return Action.DownSouth;
            else
                return Action.NoAction;
        }
    }

    // SouthUp
    public class SouthUp : Rule
    {
        public SouthUp() { action = Action.SouthUp; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperSouth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperSouth = targetCell;
            var middleSouth = currentCell.MiddleSouth();
            var up = currentCell.Up();

            var bottom = currentCell.Bottom();
            var bottomSouth = currentCell.BottomSouth();
            var south2 = currentCell.South2();
            var upperSouth2 = currentCell.UpperSouth2();

            if (TargetCellUnoccupied(upperSouth) &&
                TransitionCellUnoccupied(middleSouth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(up) ||
                 ConnectorCellsExist_Concave(bottom, bottomSouth, south2, upperSouth2)))

                return Action.SouthUp;
            else
                return Action.NoAction;
        }
    }

    // SouthDown
    public class SouthDown : Rule
    {
        public SouthDown() { action = Action.SouthDown; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomSouth(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomSouth = targetCell;
            var middleSouth = currentCell.MiddleSouth();
            var bottom = currentCell.Bottom();

            var up = currentCell.Up();
            var upperSouth = currentCell.UpperSouth();
            var south2 = currentCell.South2();
            var bottomSouth2 = currentCell.BottomSouth2();

            if (TargetCellUnoccupied(bottomSouth) &&
                TransitionCellUnoccupied(middleSouth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(bottom) ||
                 ConnectorCellsExist_Concave(up, upperSouth, south2, bottomSouth2)))

                return Action.SouthDown;
            else
                return Action.NoAction;
        }
    }



    // UpEast
    public class UpEast : Rule
    {
        public UpEast() { action = Action.UpEast; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperEast = targetCell;
            var up = currentCell.Up();
            var middleEast = currentCell.MiddleEast();

            var middleWest = currentCell.MiddleWest();
            var upperWest = currentCell.UpperWest();
            var up2 = currentCell.Up2();
            var up2East = currentCell.Up2East();

            if (TargetCellUnoccupied(upperEast) &&
                TransitionCellUnoccupied(up) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleEast) ||
                 ConnectorCellsExist_Concave(middleWest, upperWest, up2, up2East)))

                return Action.UpEast;
            else
                return Action.NoAction;
        }
    }

    // DownEast
    public class DownEast : Rule
    {
        public DownEast() { action = Action.DownEast; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomEast = targetCell;
            var middleEast = currentCell.MiddleEast();
            var bottom = currentCell.Bottom();

            var middleWest = currentCell.MiddleWest();
            var bottomWest = currentCell.BottomWest();
            var bottom2 = currentCell.Bottom2();
            var bottom2East = currentCell.Bottom2East();

            if (TargetCellUnoccupied(bottomEast) &&
                TransitionCellUnoccupied(bottom) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleEast) ||
                 ConnectorCellsExist_Concave(middleWest, bottomWest, bottom2, bottom2East)))

                return Action.DownEast;
            else
                return Action.NoAction;
        }
    }

    // EastUp
    public class EastUp : Rule
    {
        public EastUp() { action = Action.EastUp; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperEast = targetCell;
            var middleEast = currentCell.MiddleEast();
            var up = currentCell.Up();

            var bottom = currentCell.Bottom();
            var bottomEast = currentCell.BottomEast();
            var east2 = currentCell.East2();
            var upperEast2 = currentCell.UpperEast2();

            if (TargetCellUnoccupied(upperEast) &&
                TransitionCellUnoccupied(middleEast) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(up) ||
                 ConnectorCellsExist_Concave(bottom, bottomEast, east2, upperEast2)))

                return Action.EastUp;
            else
                return Action.NoAction;
        }
    }

    // EastDown
    public class EastDown : Rule
    {
        public EastDown() { action = Action.EastDown; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomEast = currentCell.BottomEast();
            var middleEast = currentCell.MiddleEast();
            var bottom = currentCell.Bottom();

            var up = currentCell.Up();
            var upperEast = currentCell.UpperEast();
            var east2 = currentCell.East2();
            var bottomEast2 = currentCell.BottomEast2();

            if (TargetCellUnoccupied(bottomEast) &&
                TransitionCellUnoccupied(middleEast) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(bottom) ||
                 ConnectorCellsExist_Concave(up, upperEast, east2, bottomEast2)))

                return Action.EastDown;
            else
                return Action.NoAction;
        }
    }



    // UpWest
    public class UpWest : Rule
    {
        public UpWest() { action = Action.UpWest; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperWest = targetCell;
            var up = currentCell.Up();
            var middleWest = currentCell.MiddleWest();

            var middleEast = currentCell.MiddleEast();
            var upperEast = currentCell.UpperEast();
            var up2 = currentCell.Up2();
            var up2West = currentCell.Up2West();

            if (TargetCellUnoccupied(upperWest) &&
                TransitionCellUnoccupied(up) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleWest) ||
                 ConnectorCellsExist_Concave(middleEast, upperEast, up2, up2West)))

                return Action.UpWest;
            else
                return Action.NoAction;
        }
    }

    // DownWest
    public class DownWest : Rule
    {
        public DownWest() { action = Action.DownWest; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomWest = targetCell;
            var middleWest = currentCell.MiddleWest();
            var bottom = currentCell.Bottom();

            var middleEast = currentCell.MiddleEast();
            var bottomEast = currentCell.BottomEast();
            var bottom2 = currentCell.Bottom2();
            var bottom2West = currentCell.Bottom2West();

            if (TargetCellUnoccupied(bottomWest) &&
                TransitionCellUnoccupied(bottom) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleWest) ||
                 ConnectorCellsExist_Concave(middleEast, bottomEast, bottom2, bottom2West)))

                return Action.DownWest;
            else
                return Action.NoAction;
        }
    }

    // WestUp
    public class WestUp : Rule
    {
        public WestUp() { action = Action.WestUp; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.UpperWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var upperWest = targetCell;
            var middleWest = currentCell.MiddleWest();
            var up = currentCell.Up();

            var bottom = currentCell.Bottom();
            var bottomWest = currentCell.BottomWest();
            var west2 = currentCell.West2();
            var upperWest2 = currentCell.UpperWest2();

            if (TargetCellUnoccupied(upperWest) &&
                TransitionCellUnoccupied(middleWest) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(up) ||
                 ConnectorCellsExist_Concave(bottom, bottomWest, west2, upperWest2)))

                return Action.WestUp;
            else
                return Action.NoAction;
        }
    }

    // WestDown
    public class WestDown : Rule
    {
        public WestDown() { action = Action.WestDown; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.BottomWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var bottomWest = targetCell;
            var middleWest = currentCell.MiddleWest();
            var bottom = currentCell.Bottom();

            var up = currentCell.Up();
            var upperWest = currentCell.UpperWest();
            var west2 = currentCell.West2();
            var bottomWest2 = currentCell.BottomWest2();

            if (TargetCellUnoccupied(bottomWest) &&
                TransitionCellUnoccupied(middleWest) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(bottom) ||
                 ConnectorCellsExist_Concave(up, upperWest, west2, bottomWest2)))

                return Action.EastDown;
            else
                return Action.NoAction;
        }
    }



    // NorthEast
    public class NorthEast : Rule
    {
        public NorthEast() { action = Action.NorthEast; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleNorthEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var northEast = targetCell;
            var middleNorth = currentCell.MiddleNorth();
            var middleEast = currentCell.MiddleEast();

            var middleWest = currentCell.MiddleWest();
            var northWest = currentCell.MiddleNorthWest();
            var north2 = currentCell.North2();
            var north2East = currentCell.North2East();

            if (TargetCellUnoccupied(northEast) &&
                TransitionCellUnoccupied(middleNorth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleEast) ||
                 ConnectorCellsExist_Concave(middleWest, northWest, north2, north2East)))

                return Action.NorthEast;
            else
                return Action.NoAction;
        }
    }

    // NorthWest
    public class NorthWest : Rule
    {
        public NorthWest() { action = Action.NorthWest; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleNorthWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var northWest = targetCell;
            var middleNorth = currentCell.MiddleNorth();
            var middleWest = currentCell.MiddleWest();

            var middleEast = currentCell.MiddleEast();
            var northEast = currentCell.MiddleNorthEast();
            var north2 = currentCell.North2();
            var north2West = currentCell.North2West();

            if (TargetCellUnoccupied(northWest) &&
                TransitionCellUnoccupied(middleNorth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleWest) ||
                 ConnectorCellsExist_Concave(middleEast, northEast, north2, north2West)))

                return Action.NorthWest;
            else
                return Action.NoAction;
        }
    }

    // EastNorth
    public class EastNorth : Rule
    {
        public EastNorth() { action = Action.EastNorth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleNorthEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var northEast = targetCell;
            var middleEast = currentCell.MiddleEast();
            var middleNorth = currentCell.MiddleNorth();

            var middleSouth = currentCell.MiddleSouth();
            var southEast = currentCell.MiddleSouthEast();
            var east2 = currentCell.East2();
            var northEast2 = currentCell.NorthEast2();

            if (TargetCellUnoccupied(northEast) &&
                TransitionCellUnoccupied(middleEast) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleNorth) ||
                 ConnectorCellsExist_Concave(middleSouth, southEast, east2, northEast2)))

                return Action.EastNorth;
            else
                return Action.NoAction;
        }
    }

    // WestNorth
    public class WestNorth : Rule
    {
        public WestNorth() { action = Action.WestNorth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleNorthWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var northWest = targetCell;
            var middleWest = currentCell.MiddleWest();
            var middleNorth = currentCell.MiddleNorth();

            var middleSouth = currentCell.MiddleSouth();
            var southWest = currentCell.MiddleSouthWest();
            var west2 = currentCell.West2();
            var northWest2 = currentCell.NorthWest2();

            if (TargetCellUnoccupied(northWest) &&
                TransitionCellUnoccupied(middleWest) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleNorth) ||
                 ConnectorCellsExist_Concave(middleSouth, southWest, west2, northWest2)))

                return Action.WestNorth;
            else
                return Action.NoAction;
        }
    }



    // SouthEast
    public class SouthEast : Rule
    {
        public SouthEast() { action = Action.SouthEast; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleSouthEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var southEast = targetCell;
            var middleSouth = currentCell.MiddleSouth();
            var middleEast = currentCell.MiddleEast();

            var middleWest = currentCell.MiddleWest();
            var southWest = currentCell.MiddleSouthWest();
            var south2 = currentCell.South2();
            var south2East = currentCell.South2East();

            if (TargetCellUnoccupied(southEast) &&
                TransitionCellUnoccupied(middleSouth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleEast) ||
                 ConnectorCellsExist_Concave(middleWest, southWest, south2, south2East)))

                return Action.SouthEast;
            else
                return Action.NoAction;
        }
    }

    // SouthWest
    public class SouthWest : Rule
    {
        public SouthWest() { action = Action.SouthWest; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleSouthWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var southWest = targetCell;
            var middleSouth = currentCell.MiddleSouth();
            var middleWest = currentCell.MiddleWest();

            var middleEast = currentCell.MiddleEast();
            var southEast = currentCell.MiddleSouthEast();
            var south2 = currentCell.South2();
            var south2West = currentCell.South2West();

            if (TargetCellUnoccupied(southWest) &&
                TransitionCellUnoccupied(middleSouth) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleWest) ||
                 ConnectorCellsExist_Concave(middleEast, southEast, south2, south2West)))

                return Action.SouthWest;
            else
                return Action.NoAction;
        }
    }

    // EastSouth
    public class EastSouth : Rule
    {
        public EastSouth() { action = Action.EastSouth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleSouthEast(); }


        public override Action CheckAction(Cell currentCell)
        {
            var southEast = targetCell;
            var middleEast = currentCell.MiddleEast();
            var middleSouth = currentCell.MiddleSouth();

            var middleNorth = currentCell.MiddleNorth();
            var northEast = currentCell.MiddleNorthEast();
            var east2 = currentCell.East2();
            var southEast2 = currentCell.SouthEast2();

            if (TargetCellUnoccupied(southEast) &&
                TransitionCellUnoccupied(middleEast) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleSouth) ||
                 ConnectorCellsExist_Concave(middleNorth, northEast, east2, southEast2)))

                return Action.EastSouth;
            else
                return Action.NoAction;
        }
    }

    // WestSouth
    public class WestSouth : Rule
    {
        public WestSouth() { action = Action.WestSouth; }

        public override void SetTargetCell(Cell currentCell) { targetCell = currentCell.MiddleSouthWest(); }


        public override Action CheckAction(Cell currentCell)
        {
            var southWest = targetCell;
            var middleWest = currentCell.MiddleWest();
            var middleSouth = currentCell.MiddleSouth();

            var middleNorth = currentCell.MiddleNorth();
            var northWest = currentCell.MiddleNorthWest();
            var west2 = currentCell.West2();
            var southWest2 = currentCell.SouthWest2();

            if (TargetCellUnoccupied(southWest) &&
                TransitionCellUnoccupied(middleWest) &&
                PotentialDisconnections(currentCell) == false &&
                (ConnectorCellExists_Convex(middleSouth) ||
                 ConnectorCellsExist_Concave(middleNorth, northWest, west2, southWest2)))

                return Action.WestSouth;
            else
                return Action.NoAction;
        }
    }










    ////////////////////////////   MOVEMENTS  ////////////////////////////

    // LINEAR MOVEMENT
    // LinearMovement: Executes a Linear Mouvement and updates the Cell grid
    public IEnumerator LinearMove(Cell currentCell, Cell targetCell, Vector3 direction, Agent agent, int CellSize) //Vector3 centerRotation, Vector3 axisRotation
    {
        agent.Obj.transform.Translate(direction * CellSize); //RotateAround(centerRotation, axisRotation, 90); 
        agent.Cell = targetCell;

        currentCell.Alive = false;
        currentCell.agent = null;
        targetCell.Alive = true;
        targetCell.agent = agent;

        yield return new WaitForSeconds(speed);
    }


    // ConvexConcaveMovement: Executes a Concave OR Convex Movement and updates the Cell grid
    public IEnumerator ConvexConcaveMove(Cell currentCell, Cell transitionCell, Cell targetCell, 
                                             Vector3 direction1, Vector3 direction2, Agent agent, int CellSize)
    {
        agent.Obj.transform.Translate(direction1 * CellSize);
        agent.Cell = transitionCell;

        currentCell.Alive = false;
        currentCell.agent = null;
        transitionCell.Alive = true;
        transitionCell.agent = agent;

        yield return null; //new WaitForSeconds(speed);


        agent.Obj.transform.Translate(direction2 * CellSize);
        agent.Cell = targetCell;

        transitionCell.Alive = false;
        transitionCell.agent = null;
        targetCell.Alive = true;
        targetCell.agent = agent;

        yield return new WaitForSeconds(speed);
    }

}
