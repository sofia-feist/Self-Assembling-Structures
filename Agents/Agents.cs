using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Agents
{
    CommonMethods CM = new CommonMethods();
    CellGrid _grid;


    private GameObject agent;
    private Material material;
    private PhysicMaterial physicsMaterial;
    private int NumberOfAgents;

    private Agent[] listAgents;





    // Constructor
    public Agents(CellGrid grid, GameObject prefab, Material _material, PhysicMaterial _physicsMaterial, int _NumberOfAgents)
    {
        _grid = grid;
        agent = prefab;
        material = _material;
        physicsMaterial = _physicsMaterial;
        NumberOfAgents = _NumberOfAgents;
        listAgents = new Agent[NumberOfAgents];
    }






    ////////////////////////////   CELL PLACEMENT METHODS  ////////////////////////////

    // FillCellsWithAgents: Fills the entire grid of cells with agents (For a Self-Disassembly Process?)
    public Agent[] FillCellsWithAgents()
    {
        listAgents = new Agent[_grid.Cells.Length];

        for (int x = 0; x < _grid.Cells.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.Cells.GetLength(1); y++)
            {
                for (int z = 0; z < _grid.Cells.GetLength(2); z++)
                {
                    int i = _grid.Cells.GetLength(1) * x + _grid.Cells.GetLength(2) * y + z;

                    Cell currentCell = _grid.Cells[x, y, z];
                    currentCell.Alive = true;
                    currentCell.Occupancy = CellOccupancy.Occupied;
                    
                    Agent newAgent = new Agent(agent, material, physicsMaterial, currentCell.Center, i);
                    currentCell.agent = newAgent;
                    newAgent.Cell = currentCell;
                    
                    listAgents[i] = newAgent;
                }
            }
        }
        return listAgents;
    }


    // PlaceAgentsIn2DRows: Places the agents in a 2D regular grid of rows
    public Agent[] PlaceAgentsIn2DRows(Vector3Int placement) 
    {
        int count = 0;

        int rows = (int)Mathf.Ceil(Mathf.Sqrt(NumberOfAgents));        // Square root of number of Agents
        int columns = (int)Mathf.Ceil(Mathf.Sqrt(NumberOfAgents));

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (count < NumberOfAgents)
                {
                    Cell cell = _grid.Cells[i + placement.x, placement.y, j + placement.z]; 
                    cell.Alive = true;
                    cell.Occupancy = CellOccupancy.Occupied;

                    Agent newAgent = new Agent(agent, material, physicsMaterial, cell.Center, count);
                    cell.agent = newAgent;
                    newAgent.Cell = cell;

                    listAgents[count] = newAgent;
                    count++;
                }
            }
        }
        return listAgents;
    }


    // PlaceAgentsIn3DRows: Places the agents in a 3D regular grid of rows
    public Agent[] PlaceAgentsIn3DRows(Vector3Int placement)
    {
        int count = 0;

        int rows = (int) Mathf.Ceil(Mathf.Pow(NumberOfAgents, 1f / 3f));       // Cubic root of number of Agents
        int columns = (int) Mathf.Ceil(Mathf.Pow(NumberOfAgents, 1f / 3f));
        int height = (int) Mathf.Ceil(Mathf.Pow(NumberOfAgents, 1f / 3f));

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < columns; k++)
                {
                    if (count < NumberOfAgents)
                    {
                        Cell cell = _grid.Cells[j + placement.x, i + placement.y, k + placement.z];
                        cell.Alive = true;
                        cell.Occupancy = CellOccupancy.Occupied;

                        Agent newAgent = new Agent(agent, material, physicsMaterial, cell.Center, count);
                        cell.agent = newAgent;
                        newAgent.Cell = cell;

                        listAgents[count] = newAgent;
                        count++;
                    }
                }
            }
        }
        return listAgents;
    }


    // PlaceAgentsInGivenGeometry: Places the agents in a given list of points that forms a 3D shape (only for points inside the lattice)
    public Agent[] PlaceAgentsInGivenGeometry(IEnumerable<Vector3> listPositions)
    {
        for (int i = 0; i < NumberOfAgents; i++)    // CHECK EXCEPTION: NumAgents != listPositions.Count()
        {
            Vector3Int cellLocation = _grid.GetCellLocation(listPositions.ElementAt(i));

            Cell currentCell = _grid.GetCell(cellLocation);
            currentCell.Alive = true;
            currentCell.Occupancy = CellOccupancy.Occupied;

            Agent newAgent = new Agent(agent, material, physicsMaterial, currentCell.Center, i);
            currentCell.agent = newAgent;
            newAgent.Cell = currentCell;

            listAgents[i] = newAgent;
        }
        return listAgents;
    }


    // PlaceAgentsRandomly: Randomly places the agents in the cell grid with no overlap/intersections
    public Agent[] PlaceAgentsRandomly()
    {
        int tries = 10000;    ///// tries -> loop failsafe
        int count = 0;

        while (count < NumberOfAgents && tries-- > 0)
        {
            Vector3Int cellLocation = CM.RandomPosition(0, _grid.AreaSize - 1);
            Cell currentCell = _grid.GetCell(cellLocation);

            if (currentCell.Alive == false)
            {
                currentCell.Alive = true;
                currentCell.Occupancy = CellOccupancy.Occupied;

                Agent newAgent = new Agent(agent, material, physicsMaterial, currentCell.Center, count);
                currentCell.agent = newAgent;
                newAgent.Cell = currentCell;

                listAgents[count] = newAgent;
                count++;
            }
        }
        return listAgents;
    }


    // PlaceConnectedAgentsRandomly: Randomly places the agents in the cell grid with no overlap/intersections and NO DISCONNECTIONS in the structure
    public Agent[] PlaceConnectedAgentsRandomly(Vector3Int placement)
    {
        int tries = 10000;    ///// tries -> loop failsafe
        int count = 0;


        // Place First Agent
        Cell firstCell = _grid.GetCell(placement);
        firstCell.Alive = true;
        firstCell.Occupancy = CellOccupancy.Occupied;

        Agent firstAgent = new Agent(agent, material, physicsMaterial, firstCell.Center, count);
        firstCell.agent = firstAgent;
        firstAgent.Cell = firstCell;

        listAgents[count] = firstAgent;
        count++;

        //Place remaining agents
        while (count < NumberOfAgents && tries-- > 0)
        {
            Agent randomAgent = listAgents[Random.Range(0, count)];
            Cell agentCell = randomAgent.Cell;

            var faceNeighbours = agentCell.GetFaceNeighbours().Where(cell => cell.Alive == false).ToList();

            if (faceNeighbours.Count != 0)
            {
                Cell randomNeighbour = faceNeighbours[Random.Range(0, faceNeighbours.Count)];
                randomNeighbour.Alive = true;
                randomNeighbour.Occupancy = CellOccupancy.Occupied;

                Agent newAgent = new Agent(agent, material, physicsMaterial, randomNeighbour.Center, count);
                randomNeighbour.agent = newAgent;
                newAgent.Cell = randomNeighbour;

                listAgents[count] = newAgent;
                count++;
            }
        }
        return listAgents;
    }
}