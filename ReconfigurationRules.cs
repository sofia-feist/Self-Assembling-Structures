using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;



// Action enumerator: Enumerates possible actions
public enum Action
{
    NoAction,

    X90,
    X_90,
    X180,
    X_180,
    X270,
    X_270,

    Y90,
    Y_90,
    Y180,
    Y_180,
    Y270,
    Y_270,

    Z90,
    Z_90,
    Z180,
    Z_180,
    Z270,
    Z_270,
}




public class ReconfigurationRules
{
    CellGrid grid;

    public ReconfigurationRules(CellGrid _grid)  { grid = _grid; }





   

    // ChoseRule: Choose a rule/action to execute according to rule fitness
    public Rule ChoseRule(Agent[] listAgents, Agent agent)
    {
        Rule chosenRule = null;
        var rules = LocalRules(agent);
        CommonMethods.RandomShuffle(rules);
        var otherAgents = listAgents.Where(a => a.Id != agent.Id).Select(a => a.Cell.Center).ToArray();

        foreach (var rule in rules)  // Collect Fitnesses
        {
            var sendList = otherAgents.Union(new Vector3[] { rule.targetCell.Center }).ToArray();
            rule.CalculateConvergence(agent);
            //rule.CollectDisplacement(sendList);
        }
            
        rules = rules.OrderByDescending(r => r.convergence - r.displacement).ToList();    // CONVERGENCE + DISPLACEMENT??

        if (rules.Count != 0)
            chosenRule = rules[0];

        return chosenRule;
    }


    // LocalRules: Selects applicable rules according to local neighbourhood 
    public List<Rule> LocalRules(Agent agent)
    {
        Cell currentCell = agent.Cell;
        var connections = agent.GetActiveConnectors();
        List<Rule> rules = new List<Rule>();

        if (connections.Any(c => c == Connections.Up))
        {
            Cell pivot = currentCell.Up();

            if (pivot.GetXFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var x90 = new Rule(pivot, currentCell.UpperSouth(), currentCell.MiddleSouth(), Action.X90);
                if (x90.CheckApplicability(currentCell)) rules.Add(x90);

                var x_90 = new Rule(pivot, currentCell.UpperNorth(), currentCell.MiddleNorth(), Action.X_90);
                if (x_90.CheckApplicability(currentCell)) rules.Add(x_90);
            }
            if (pivot.GetZFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var y90 = new Rule(pivot, currentCell.UpperEast(), currentCell.MiddleEast(), Action.Y90);
                if (y90.CheckApplicability(currentCell)) rules.Add(y90);

                var y_90 = new Rule(pivot, currentCell.UpperWest(), currentCell.MiddleWest(), Action.Y_90);
                if (y_90.CheckApplicability(currentCell)) rules.Add(y_90);
            }
        }

        if (connections.Any(c => c == Connections.Down))
        {
            Cell pivot = currentCell.Bottom();

            if (pivot.GetXFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var x90 = new Rule(pivot, currentCell.BottomNorth(), currentCell.MiddleNorth(), Action.X90);
                if (x90.CheckApplicability(currentCell)) rules.Add(x90);

                var x_90 = new Rule(pivot, currentCell.BottomSouth(), currentCell.MiddleSouth(), Action.X_90);
                if (x_90.CheckApplicability(currentCell)) rules.Add(x_90);
            }
            if (pivot.GetZFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var y90 = new Rule(pivot, currentCell.BottomWest(), currentCell.MiddleWest(), Action.Y90);
                if (y90.CheckApplicability(currentCell)) rules.Add(y90);

                var y_90 = new Rule(pivot, currentCell.BottomEast(), currentCell.MiddleEast(), Action.Y_90);
                if (y_90.CheckApplicability(currentCell)) rules.Add(y_90);
            }

        }

        if (connections.Any(c => c == Connections.North))
        {
            Cell pivot = currentCell.MiddleNorth();

            if (pivot.GetXFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var x90 = new Rule(pivot, currentCell.UpperNorth(), currentCell.Up(), Action.X90);
                if (x90.CheckApplicability(currentCell)) rules.Add(x90);

                var x_90 = new Rule(pivot, currentCell.BottomNorth(), currentCell.Bottom(), Action.X_90);
                if (x_90.CheckApplicability(currentCell)) rules.Add(x_90);
            }
            if (pivot.GetYFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var z90 = new Rule(pivot, currentCell.MiddleNorthWest(), currentCell.MiddleWest(), Action.Z90);
                if (z90.CheckApplicability(currentCell)) rules.Add(z90);

                var z_90 = new Rule(pivot, currentCell.MiddleNorthEast(), currentCell.MiddleEast(), Action.Z_90);
                if (z_90.CheckApplicability(currentCell)) rules.Add(z_90);
            }
        }

        if (connections.Any(c => c == Connections.South))
        {
            Cell pivot = currentCell.MiddleSouth();

            if (pivot.GetXFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var x90 = new Rule(pivot, currentCell.BottomSouth(), currentCell.Bottom(), Action.X90);
                if (x90.CheckApplicability(currentCell)) rules.Add(x90);

                var x_90 = new Rule(pivot, currentCell.UpperSouth(), currentCell.Up(), Action.X_90);
                if (x_90.CheckApplicability(currentCell)) rules.Add(x_90);
            }
            if (pivot.GetYFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var z90 = new Rule(pivot, currentCell.MiddleSouthEast(), currentCell.MiddleEast(), Action.Z90);
                if (z90.CheckApplicability(currentCell)) rules.Add(z90);

                var z_90 = new Rule(pivot, currentCell.MiddleSouthWest(), currentCell.MiddleWest(), Action.Z_90);
                if (z_90.CheckApplicability(currentCell)) rules.Add(z_90);
            }
        }

        if (connections.Any(c => c == Connections.East))
        {
            Cell pivot = currentCell.MiddleEast();

            if (pivot.GetZFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var y90 = new Rule(pivot, currentCell.BottomEast(), currentCell.Bottom(), Action.Y90);
                if (y90.CheckApplicability(currentCell)) rules.Add(y90);

                var y_90 = new Rule(pivot, currentCell.UpperEast(), currentCell.Up(), Action.Y_90);
                if (y_90.CheckApplicability(currentCell)) rules.Add(y_90);
            }
            if (pivot.GetYFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var z90 = new Rule(pivot, currentCell.MiddleNorthEast(), currentCell.MiddleNorth(), Action.Z90);
                if (z90.CheckApplicability(currentCell)) rules.Add(z90);

                var z_90 = new Rule(pivot, currentCell.MiddleSouthEast(), currentCell.MiddleSouth(), Action.Z_90);
                if (z_90.CheckApplicability(currentCell)) rules.Add(z_90);
            }
        }

        if (connections.Any(c => c == Connections.West))
        {
            Cell pivot = currentCell.MiddleWest();

            if (pivot.GetZFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var y90 = new Rule(pivot, currentCell.UpperWest(), currentCell.Up(), Action.Y90);
                if (y90.CheckApplicability(currentCell)) rules.Add(y90);

                var y_90 = new Rule(pivot, currentCell.BottomWest(), currentCell.Bottom(), Action.Y_90);
                if (y_90.CheckApplicability(currentCell)) rules.Add(y_90);
            }
            if (pivot.GetYFaceNeighbours().Count(n => n.Alive) > 0)
            {
                var z90 = new Rule(pivot, currentCell.MiddleSouthWest(), currentCell.MiddleSouth(), Action.Z90);
                if (z90.CheckApplicability(currentCell)) rules.Add(z90);

                var z_90 = new Rule(pivot, currentCell.MiddleNorthWest(), currentCell.MiddleNorth(), Action.Z_90);
                if (z_90.CheckApplicability(currentCell)) rules.Add(z_90);
            }
        }

        return rules;
    }


    // ExecuteAction: Executes the given Action
    public void ExecuteAction(Agent agent, Rule rule)
    {
        Action action = rule.action;

        switch (action)
        {
            case Action.X90:
                ModuleRotation(agent, rule.pivot.agent, Vector3.right, 90);
                break;
            case Action.X_90:
                ModuleRotation(agent, rule.pivot.agent, Vector3.right, -90);
                break;
            case Action.X180:
                ModuleRotation(agent, rule.pivot.agent, Vector3.right, 180);
                break;
            case Action.X_180:
                ModuleRotation(agent, rule.pivot.agent, Vector3.right, -180);
                break;
            case Action.X270:
                ModuleRotation(agent, rule.pivot.agent, Vector3.right, 270);
                break;
            case Action.X_270:
                ModuleRotation(agent, rule.pivot.agent, Vector3.right, -270);
                break;

            case Action.Y90:
                ModuleRotation(agent, rule.pivot.agent, Vector3.forward, 90);
                break;
            case Action.Y_90:
                ModuleRotation(agent, rule.pivot.agent, Vector3.forward, -90);
                break;
            case Action.Y180:
                ModuleRotation(agent, rule.pivot.agent, Vector3.forward, 180);
                break;
            case Action.Y_180:
                ModuleRotation(agent, rule.pivot.agent, Vector3.forward, -180);
                break;
            case Action.Y270:
                ModuleRotation(agent, rule.pivot.agent, Vector3.forward, 270);
                break;
            case Action.Y_270:
                ModuleRotation(agent, rule.pivot.agent, Vector3.forward, -270);
                break;

            case Action.Z90:
                ModuleRotation(agent, rule.pivot.agent, Vector3.up, 90);
                break;
            case Action.Z_90:
                ModuleRotation(agent, rule.pivot.agent, Vector3.up, -90);
                break;
            case Action.Z180:
                ModuleRotation(agent, rule.pivot.agent, Vector3.up, 180);
                break;
            case Action.Z_180:
                ModuleRotation(agent, rule.pivot.agent, Vector3.up, -180);
                break;
            case Action.Z270:
                ModuleRotation(agent, rule.pivot.agent, Vector3.up, 270);
                break;
            case Action.Z_270:
                ModuleRotation(agent, rule.pivot.agent, Vector3.up, -270);
                break;
        }
    }


    // Rule Class
    public class Rule
    { 
        public Cell pivot;
        public Cell targetCell; 
        public Cell transitionCell;
        public Action action;

        public float convergence;
        public float displacement;


        public Rule(Cell _pivot, Cell _targetCell, Cell _transitionCell, Action _action)
        {
            pivot = _pivot;
            action = _action;
            targetCell = _targetCell;
            transitionCell = _transitionCell;

            convergence = 0;
            displacement = 0;
        }


        public bool CheckApplicability(Cell currentCell)
        {
            if (targetCell?.Alive == false &&
                transitionCell?.Alive == false &&
                PotentialDisconnections(currentCell) == false)

                return true;
            else
                return false;
        }

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

                if (nNeighbours.Count() == 1 ||                                                                         // If neighbour only has one face neighbour (aka current cell), DO NOT MOVE
                    (FaceNeighbours.Count() == 2 && FaceNeighbours.ElementAt(0).OppositeNeighbours(FaceNeighbours.ElementAt(1))                          // If current cell only has two face neighbours on opposite faces, DO NOT MOVE (unless one of them is a final state agent or seed)
                         && FaceNeighbours.All(n => n.agent.State == AgentState.Active || n.agent.State == AgentState.Inactive)
                         && Random.Range(0.0f, 1.0f) > 0.05) ||                                                                                      // This random range gives this condition a very small probability(5%) of being disregarded (-> TO AVOID LOCAL OPTIMA, although it also gives the structure a chance to disconnect)
                    (FaceNeighbours.Count() > 1 && commonNeighbours.Count() == 1 && commonNeighbours.ElementAt(0).Location == currentCell.Location       // If current cell is the only cell in common between two face neighbours, DO NOT MOVE (unless one of them is a final state agent or seed)
                         && FaceNeighbours.All(n => n.agent.State == AgentState.Active || n.agent.State == AgentState.Inactive)))
                    return true;
            }

            return false;
        }

        // CalculateConvergence: Calculates the rule's convergence fitness
        internal void CalculateConvergence(Agent agent)
        {
            int currentScent = agent.ScentValue;
            var targetNeighbours = targetCell.GetFaceNeighbours().Where(a => a.Alive == true);
            int newScent = targetNeighbours.Select(s => s.agent.ScentValue).Max() - 1;

            convergence = newScent - currentScent;
        }

        // CalculateConvergence: Calculates the rule's convergence fitness
        internal void CollectDisplacement(Vector3[] sendList)
        {
            int tries = 0;

            while (tries < 3)
            {
                int count = 0;

                UDPSend.SendData(UDPSend.EncodeMessage(sendList));
                UDPReceive.MessageReceived = false;

                while (UDPReceive.MessageReceived == false && count < 3)
                {
                    Thread.Sleep(70);
                    count++;
                }

                if (UDPReceive.MessageReceived == true)
                {
                    displacement = UDPReceive.maxDisplacement;
                    break;
                }

                tries++;
                if (tries == 3) Debug.Log("Action Lost");
            }
        }
    }









    


    ////////////////////////////   MOVEMENTS  ////////////////////////////

    // INDIVIDUAL MOVE 
    // ModuleRotation: Rotate a module around a pivot module
    private void ModuleRotation(Agent agent, Agent pivot, Vector3 rotationAxis, float angle)
    {
        Cell currentCell = agent.Cell;
        agent.Obj.transform.RotateAround(pivot.Obj.transform.position, rotationAxis, angle);
        pivot.Obj.transform.RotateAround(pivot.Obj.transform.position, rotationAxis, angle);

        Cell newCell = grid.GetCell(grid.GetCellLocation(agent.Obj.transform.position));
        agent.Cell = newCell;

        currentCell.Alive = false;
        currentCell.agent = null;

        newCell.Alive = true;
        newCell.agent = agent;
    }


    // NOT CORRECT...
    public void LinearMove(Agent agent1, Agent agent2, Vector3 rotationAxis)
    {
        ModuleRotation(agent2, agent1, rotationAxis, 180);
        ModuleRotation(agent1, agent2, rotationAxis, 180);
    }
}
