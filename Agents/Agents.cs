using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Agents
{
    CommonMethods CM = new CommonMethods();
    CellGrid _grid;


    GameObject agent;
    Material material;
    int NumberOfAgents;

    public Agent[] listAgents;





    // Constructor
    public Agents(CellGrid grid, GameObject prefab, Material _material, int _NumberOfAgents)
    {
        _grid = grid;
        agent = prefab;
        material = _material;
        NumberOfAgents = _NumberOfAgents;
        listAgents = new Agent[NumberOfAgents];
    }





    ////////////////////////////   CELL PLACEMENT METHODS  ////////////////////////////

    // FillCellsWithAgents: Fills the entire grid of cells with agents
    public Agent[] FillCellsWithAgents()
    {
        for (int x = 0; x < _grid.Cells.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.Cells.GetLength(1); y++)
            {
                for (int z = 0; z < _grid.Cells.GetLength(2); z++)
                {
                    Cell currentCell = _grid.Cells[x, y, z];
                    currentCell.Alive = true;
                    
                    Agent newAgent = new Agent(agent, material, currentCell.Center);
                    newAgent.Location = currentCell;
                    currentCell.agent = newAgent;
                    int i = _grid.Cells.GetLength(1) * x + _grid.Cells.GetLength(2) * y + z;
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
                    Cell cell = _grid.Cells[i + placement.x, placement.y, j + placement.z];    // Check Exception if Outside Area Boundaries? -> Make 3D Rows instead
                    cell.Alive = true;

                    Agent newAgent = new Agent(agent, material, cell.Center);
                    newAgent.Location = cell;
                    cell.agent = newAgent;
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

                        Agent newAgent = new Agent(agent, material, cell.Center);
                        newAgent.Location = cell;
                        cell.agent = newAgent;
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
        for (int i = 0; i < listPositions.Count(); i++)
        {
            Vector3Int location = _grid.GetCellLocation(listPositions.ElementAt(i));

            Cell currentCell = _grid.GetCell(location);
            currentCell.Alive = true;

            Agent newAgent = new Agent(agent, material, currentCell.Center);
            newAgent.Location = currentCell;
            currentCell.agent = newAgent;
            listAgents[i] = newAgent;
        }
        return listAgents;
    }


    // PlaceAgentsRandomly: Randomly places the agents in the cell grid with no overlap/intersections
    public Agent[] PlaceAgentsRandomly(int AreaMin, int AreaMax)
    {
        int tries = 10000;    ///// tries -> loop failsafe
        int index = 0;

        while (index < NumberOfAgents && tries-- > 0)
        {
            Vector3Int cellLocation = CM.RandomPosition(AreaMin, AreaMax);
            Cell currentCell = _grid.GetCell(cellLocation);

            if (currentCell.Alive == false)
            {
                currentCell.Alive = true;

                Agent newAgent = new Agent(agent, material, currentCell.Center);
                newAgent.Location = currentCell;
                currentCell.agent = newAgent;
                listAgents[index] = newAgent;
                index++;
            }
        }
        return listAgents;
    }


    // PlaceConnectedAgentsRandomly: Randomly places the agents in the cell grid with no overlap/intersections and NO DISCONNECTIONS in the structure
    public Agent[] PlaceConnectedAgentsRandomly(Vector3 placement)
    {
        int tries = 10000;    ///// tries -> loop failsafe
        int index = 0;

        Vector3Int firstCellPlacement = _grid.GetCellLocation(placement);
        Cell firstCell = _grid.GetCell(firstCellPlacement);
        firstCell.Alive = true;

        Agent firstAgent = new Agent(agent, material, firstCell.Center);
        firstAgent.Location = firstCell;
        listAgents[index] = firstAgent;

        while (index < NumberOfAgents && tries-- > 0)
        {
            Agent randomAgent = listAgents[Random.Range(0, index)];
            Cell agentCell = randomAgent.Location;

            var faceNeighbours = agentCell.GetFaceNeighbours().Where(cell => cell.Alive == false).ToList();

            if (faceNeighbours.Count != 0)
            {
                Cell randomNeighbour = faceNeighbours[Random.Range(0, faceNeighbours.Count)];
                randomNeighbour.Alive = true;
                
                Agent newAgent = new Agent(agent, material, randomNeighbour.Center);
                newAgent.Location = randomNeighbour;
                randomNeighbour.agent = newAgent;
                listAgents[index] = newAgent;
                index++;
            }
        }
        return listAgents;
    }
}